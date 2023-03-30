using InstrumentHiringSystem.Models;
using InstrumentHiringSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Security.Claims;

namespace InstrumentHiringSystem.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        #region Razor Pages
        public IActionResult Index()
        {
            var instruments = _unitOfWork.Instrument.GetAll(filter: x => !x.IsDeleted, includeProperties: "Category, CoverType");
            
            return View(instruments);
        }

        public IActionResult Details(int instrumentID)
        {
            ShoppingCart cart = new ShoppingCart()
            {
                Count = 1,
                InstrumentID = instrumentID,
                Instrument = _unitOfWork.Instrument.GetFirstOrDefault(filter: x => x.ID == instrumentID, includeProperties: "Category, CoverType")
            };

            return View(cart);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion

        #region API Calls
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCart.UserID = claim.Value;

            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u =>u.UserID == claim.Value && u.InstrumentID == shoppingCart.InstrumentID);

            if (cart == null)
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
            }
            else
            {
                _unitOfWork.ShoppingCart.IncrementCount(cart, shoppingCart.Count);
            }

            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}