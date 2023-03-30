using InstrumentHiringSystem.Models.ViewModels;
using InstrumentHiringSystem.Models;
using InstrumentHiringSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;
using System.Security.Claims;
using InstrumentHiringSystem.Models.Constants;

namespace InstrumentHiringSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Razor Pages
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(int ID)
        {
            OrderViewModel orderViewModel = new OrderViewModel()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.ID == ID, includeProperties: "AppUser"),
                OrderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderID == ID, includeProperties: "Instrument")
            };
            return View(orderViewModel);
        }
        #endregion

        #region API Calls
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeadersList;

            if (User.IsInRole(Constant.Admin) || User.IsInRole(Constant.Employee))
            {
                orderHeadersList = _unitOfWork.OrderHeader.GetAll(includeProperties: "AppUser");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeadersList = _unitOfWork.OrderHeader.GetAll(u => u.UserID == claim.Value, includeProperties: "AppUser");
            }

            switch (status)
            {
                case "pending":
                    orderHeadersList = orderHeadersList.Where(u => u.OrderStatus == Constant.PaymentPending);
                    break;
                case "inprocess":
                    orderHeadersList = orderHeadersList.Where(u => u.OrderStatus == Constant.OrderProcessing);
                    break;
                case "completed":
                    orderHeadersList = orderHeadersList.Where(u => u.OrderStatus == Constant.OrderShipped);
                    break;
                default:
                    break;
            }

            return Json(new { data = orderHeadersList });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constant.Admin + ", " + Constant.Employee)]
        public IActionResult UpdateOrderDetails(OrderViewModel orderViewModel)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.ID == orderViewModel.OrderHeader.ID);
            orderHeader.Name = orderViewModel.OrderHeader.Name;
            orderHeader.PhoneNumber = orderViewModel.OrderHeader.PhoneNumber;
            orderHeader.City = orderViewModel.OrderHeader.City;
            orderHeader.Region = orderViewModel.OrderHeader.Region;

            if (orderViewModel.OrderHeader.Carrier != null)
            {
                orderHeader.Carrier = orderViewModel.OrderHeader.Carrier;
            }
            if (orderViewModel.OrderHeader.TrackingNumber != null)
            {
                orderHeader.TrackingNumber = orderViewModel.OrderHeader.TrackingNumber;
            }

            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["Success"] = "Order Details updated successfully";
            return RedirectToAction("Detail", "Order", new { ID = orderHeader.ID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constant.Admin + ", " + Constant.Employee)]
        public IActionResult StartProcessing(OrderViewModel orderViewModel)
        {
            _unitOfWork.OrderHeader.UpdateStatus(orderViewModel.OrderHeader.ID, Constant.OrderProcessing);
            _unitOfWork.Save();
            TempData["Success"] = "Order Status updated successfully";
            return RedirectToAction("Detail", "Order", new { ID = orderViewModel.OrderHeader.ID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constant.Admin + ", " + Constant.Employee)]
        public IActionResult ShipOrder(OrderViewModel orderViewModel)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.ID == orderViewModel.OrderHeader.ID);
            orderHeader.TrackingNumber = orderViewModel.OrderHeader.TrackingNumber;
            orderHeader.Carrier = orderViewModel.OrderHeader.Carrier;
            orderHeader.OrderStatus = Constant.OrderShipped;
            orderHeader.ShippedDate = DateTime.Now;

            if (orderHeader.PaymentStatus == Constant.PaymentDelayed)
            {
                orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
            }

            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["Success"] = "Order shipped successfully";
            return RedirectToAction("Detail", "Order", new { ID = orderViewModel.OrderHeader.ID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constant.Admin + ", " + Constant.Employee)]
        public IActionResult CancelOrder(OrderViewModel orderViewModel)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.ID == orderViewModel.OrderHeader.ID);

            if (orderHeader.PaymentStatus == Constant.PaymentApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentID
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.ID, Constant.OrderCancelled, Constant.OrderRefunded);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.ID, Constant.OrderCancelled, Constant.OrderCancelled);
            }

            _unitOfWork.Save();
            TempData["Success"] = "Order cancelled successfully";
            return RedirectToAction("Detail", "Order", new { ID = orderViewModel.OrderHeader.ID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constant.Admin + ", " + Constant.Employee)]
        public IActionResult DelayedPayment(OrderViewModel orderViewModel)
        {
            orderViewModel.OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.ID == orderViewModel.OrderHeader.ID, includeProperties: "AppUser");

            orderViewModel.OrderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderID == orderViewModel.OrderHeader.ID, includeProperties: "Instrument");

            var domain = "https://localhost:44389/";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>()
                    {
                        "card"
                    },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"Admin/Order/PaymentConfirmation?orderHeaderID={orderViewModel.OrderHeader.ID}",
                CancelUrl = domain + $"Admin/Order/Detail?ID={orderViewModel.OrderHeader.ID}",
            };

            foreach (var item in orderViewModel.OrderDetails)
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
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.UpdatePaymentStatus(orderViewModel.OrderHeader.ID, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public IActionResult PaymentConfirmation(int orderHeaderID)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(i => i.ID == orderHeaderID);

            if (orderHeader.PaymentStatus == Constant.PaymentDelayed)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionID);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdatePaymentStatus(orderHeader.ID, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(orderHeader.ID, orderHeader.OrderStatus, Constant.PaymentApproved);
                    _unitOfWork.Save();
                }
            }

            return View(orderHeaderID);
        }

        #endregion
    }
}
