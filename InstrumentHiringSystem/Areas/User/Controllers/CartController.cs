using InstrumentHiringSystem.Models.ViewModels;
using InstrumentHiringSystem.Models;
using InstrumentHiringSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using InstrumentHiringSystem.Models.Constants;
using Stripe.Checkout;

namespace InstrumentHiringSystem.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IEmailSender _emailSender;

        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }

        #region Razor Pages
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cartViewModel = new ShoppingCartViewModel()
            {
                CartList = _unitOfWork.ShoppingCart.GetAll(filter: u => u.UserID == claim.Value, includeProperties: "Instrument"),
                OrderHeader = new OrderHeader()
            };

            foreach (var item in cartViewModel.CartList)
            {
                item.Price = GetPriceByQuantity(item.Count, item.Instrument.Price, item.Instrument.Price20, item.Instrument.Price50);
                cartViewModel.OrderHeader.OrderTotal += (item.Price * item.Count);
            }

            return View(cartViewModel);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartViewModel cartViewModel = new ShoppingCartViewModel()
            {
                CartList = _unitOfWork.ShoppingCart.GetAll(filter: u => u.UserID == claim.Value, includeProperties: "Instrument"),
                OrderHeader = new OrderHeader()
            };

            cartViewModel.OrderHeader.ApplicationUser = _unitOfWork.AppUser.GetFirstOrDefault(u => u.Id == claim.Value);
            cartViewModel.OrderHeader.Name = cartViewModel.OrderHeader.ApplicationUser.FullName;
            cartViewModel.OrderHeader.PhoneNumber = cartViewModel.OrderHeader.ApplicationUser.PhoneNumber;
            cartViewModel.OrderHeader.City = cartViewModel.OrderHeader.ApplicationUser.CityAddress;
            cartViewModel.OrderHeader.Region = cartViewModel.OrderHeader.ApplicationUser.RegionName;

            foreach (var item in cartViewModel.CartList)
            {
                item.Price = GetPriceByQuantity(item.Count, item.Instrument.Price, item.Instrument.Price20, item.Instrument.Price50);
                cartViewModel.OrderHeader.OrderTotal += (item.Price * item.Count);
            }

            return View(cartViewModel);
        }
        #endregion

        #region API Calls
        public IActionResult Add(int cartID)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.ID == cartID);
            _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Substract(int cartID)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.ID == cartID);

            if (cart.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cart);
                _unitOfWork.Save();
            }
            else
            {
                _unitOfWork.ShoppingCart.DecrementCount(cart, 1);
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartID)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.ID == cartID);
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult SummaryPost(ShoppingCartViewModel cartViewModel)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            cartViewModel.OrderHeader.UserID = claim.Value;
            cartViewModel.OrderHeader.OrderedDate = DateTime.Now;
            cartViewModel.CartList = _unitOfWork.ShoppingCart.GetAll(filter: u => u.UserID == claim.Value, includeProperties: "Instrument");

            foreach (var item in cartViewModel.CartList)
            {
                item.Price = GetPriceByQuantity(item.Count, item.Instrument.Price, item.Instrument.Price20, item.Instrument.Price50);
                cartViewModel.OrderHeader.OrderTotal += (item.Price * item.Count);
            }

            var applicationUser = _unitOfWork.AppUser.GetFirstOrDefault(u => u.Id == claim.Value);

            if (applicationUser.CompanyID.GetValueOrDefault() == 0)
            {
                cartViewModel.OrderHeader.OrderStatus = Constant.OrderPending;
                cartViewModel.OrderHeader.PaymentStatus = Constant.PaymentPending;
            }
            else
            {
                cartViewModel.OrderHeader.PaymentStatus = Constant.PaymentDelayed;
                cartViewModel.OrderHeader.OrderStatus = Constant.OrderApproved;
            }

            _unitOfWork.OrderHeader.Add(cartViewModel.OrderHeader);

            _unitOfWork.Save();

            foreach (var item in cartViewModel.CartList)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderID = cartViewModel.OrderHeader.ID,
                    InstrumentID = item.InstrumentID,
                    Price = item.Price,
                    Count = item.Count
                };

                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            if (applicationUser.CompanyID.GetValueOrDefault() == 0)
            {
                var domain = "https://localhost:44389/";

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string>()
                    {
                        "card"
                    },
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    SuccessUrl = domain + $"User/Cart/OrderConfirmation?id={cartViewModel.OrderHeader.ID}",
                    CancelUrl = domain + $"User/Cart/Index",
                };

                foreach (var item in cartViewModel.CartList)
                {
                    var sessionLine = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Instrument.Title
                            },
                        },
                        Quantity = item.Count,
                    };
                    options.LineItems.Add(sessionLine);
                }

                var service = new SessionService();

                var session = service.Create(options);
                
                _unitOfWork.OrderHeader.UpdatePaymentStatus(cartViewModel.OrderHeader.ID, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();

                Response.Headers.Add("Location", session.Url);

                return new StatusCodeResult(303);
            }
            else
            {
                return RedirectToAction("OrderConfirmation", "Cart", new { id = cartViewModel.OrderHeader.ID });
            }
        }

        public IActionResult OrderConfirmation(int ID)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(i => i.ID == ID);

            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var applicationUser = _unitOfWork.AppUser.GetFirstOrDefault(u => u.Id == claim.Value);

            orderHeader.ApplicationUser.Email = applicationUser.Email;

            if (orderHeader.PaymentStatus != Constant.PaymentDelayed)
            {
                var service = new SessionService();

                var session = service.Get(orderHeader.SessionID);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdatePaymentStatus(orderHeader.ID, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(orderHeader.ID, Constant.OrderApproved, Constant.PaymentApproved);
                    _unitOfWork.Save();
                }
            }

            _emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "Order Confirmation", "New Order Created");

            var cartList = _unitOfWork.ShoppingCart.GetAll(filter: u => u.UserID == orderHeader.UserID).ToList();

            _unitOfWork.ShoppingCart.RemoveRange(cartList);
            
            _unitOfWork.Save();

            return View(ID);
        }

        public double GetPriceByQuantity(int quantity, double price, double price50, double price100)
        {
            if (quantity <= 50)
            {
                return price;
            }
            else if (quantity <= 100)
            {
                return price50;
            }
            else
            {
                return price100;
            }
        }
        #endregion
    }
}
