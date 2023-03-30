using InstrumentHiringSystem.Models;
using InstrumentHiringSystem.Models.Constants;
using InstrumentHiringSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentHiringSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Constant.Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Razor Pages
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? ID)
        {
            Company company = new Company();

            if (ID == null)
            {
                return View(company);
            }

            company = _unitOfWork.Company.GetFirstOrDefault(u => u.ID == ID);

            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        public IActionResult Delete(int ID)
        {
            Company company = _unitOfWork.Company.GetFirstOrDefault(u => u.ID == ID);

            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }
        #endregion

        #region API Calls
        public IActionResult GetAll()
        {
            var categories = _unitOfWork.Company.GetAll(null, null, null);
            return Json(new { data = categories });
        }

        [HttpPost, ActionName("Upsert")]
        public IActionResult UpsertPost(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.ID == 0)
                {
                    _unitOfWork.Company.Add(company);
                    TempData["Success"] = "Company added successfully";
                }
                else
                {
                    _unitOfWork.Company.Update(company);
                    TempData["Info"] = "Company altered successfully";
                }
                _unitOfWork.Save();

                return RedirectToAction("Index");
            }
            return View(company);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int id)
        {
            var company = _unitOfWork.Company.GetFirstOrDefault(u => u.ID == id);

            if (company != null)
            {
                _unitOfWork.Company.Delete(company);
                TempData["Delete"] = "Company deleted successfully";
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }
        #endregion
    }
}
