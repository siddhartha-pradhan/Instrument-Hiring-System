using InstrumentHiringSystem.Models.Constants;
using InstrumentHiringSystem.Models.ViewModels;
using InstrumentHiringSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InstrumentHiringSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Constant.Admin)]
    public class InstrumentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public InstrumentController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }


        #region Razor Calls
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? ID)
        {
            var instrumentViewModel = new InstrumentViewModel()
            {
                Instrument = new(),
                CategoryList = _unitOfWork.Category.GetAll(filter: x => !x.IsDeleted)
                .Select(u => new SelectListItem
                {
                    Text = u.Title,
                    Value = u.ID.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll(filter: x => !x.IsDeleted)
                .Select(u => new SelectListItem
                {
                    Text = u.Title,
                    Value = u.ID.ToString()
                })
            };

            if (ID == null)
            {
                return View(instrumentViewModel);
            }

            instrumentViewModel.Instrument = _unitOfWork.Instrument.GetFirstOrDefault(x => x.ID == ID);

            if (instrumentViewModel.Instrument == null)
            {
                return NotFound();
            }

            return View(instrumentViewModel);
        }

        public IActionResult Delete(int ID)
        {
            var instrumentViewModel = new InstrumentViewModel();
            
            instrumentViewModel.Instrument = _unitOfWork.Instrument.GetFirstOrDefault(x => x.ID == ID);

            if (instrumentViewModel.Instrument != null)
            {
                return View(instrumentViewModel);
            }

            return NotFound();
        }
        #endregion

        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var instruments = _unitOfWork.Instrument.GetAll(filter: x => !x.IsDeleted, includeProperties: "Category, CoverType");

            return Json(new { data = instruments });
        }

        [HttpPost, ActionName("Upsert")]
        public IActionResult UpsertPost(InstrumentViewModel instrumentViewModel, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var wwwRootPath = _webHostEnvironment.WebRootPath;

                if (imageFile != null)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\instruments");
                    var extension = Path.GetExtension(imageFile.FileName);

                    if (instrumentViewModel.Instrument.ImageURL != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, instrumentViewModel.Instrument.ImageURL.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Exists(oldImagePath);
                        }

                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        imageFile.CopyTo(fileStreams);
                    }

                    instrumentViewModel.Instrument.ImageURL = @"\images\instruments\" + fileName + extension;
                }

                if (instrumentViewModel.Instrument.ID == 0)
                {
                    _unitOfWork.Instrument.Add(instrumentViewModel.Instrument);

                    TempData["Success"] = "Instrument added successfully";
                }
                else
                {
                    _unitOfWork.Instrument.Update(instrumentViewModel.Instrument);

                    TempData["Info"] = "Instrument altered successfully";
                }

                _unitOfWork.Save();

                return RedirectToAction("Index");
            }
            return View(instrumentViewModel);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int id)
        {
            var instrument = _unitOfWork.Instrument.GetFirstOrDefault(u => u.ID == id);

            if (instrument != null)
            {
                _unitOfWork.Instrument.Delete(instrument);

                TempData["Delete"] = "Instrument deleted successfully";
                
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
