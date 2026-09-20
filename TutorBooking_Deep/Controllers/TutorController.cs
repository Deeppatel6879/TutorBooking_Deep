using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TutorBooking.DataAccess.Repository.IRepository;
using TutorBooking.Models;
using Microsoft.AspNetCore.Authorization;
namespace TutorBooking_Deep.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TutorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TutorController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Tutor> tutorList =
                _unitOfWork.Tutor.GetAll(
                    includeProperties: "Subject");

            return View(tutorList);
        }

        public IActionResult Create()
        {
            LoadSubjectList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Tutor tutor)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Tutor.Add(tutor);
                _unitOfWork.Save();

                TempData["success"] =
                    "Tutor created successfully.";

                return RedirectToAction("Index");
            }

            LoadSubjectList();

            return View(tutor);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Tutor? tutor = _unitOfWork.Tutor.Get(
                u => u.TutorId == id);

            if (tutor == null)
            {
                return NotFound();
            }

            LoadSubjectList();

            return View(tutor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Tutor tutor)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Tutor.Update(tutor);
                _unitOfWork.Save();

                TempData["success"] =
                    "Tutor updated successfully.";

                return RedirectToAction("Index");
            }

            LoadSubjectList();

            return View(tutor);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Tutor? tutor = _unitOfWork.Tutor.Get(
                u => u.TutorId == id,
                includeProperties: "Subject");

            if (tutor == null)
            {
                return NotFound();
            }

            return View(tutor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            Tutor? tutor = _unitOfWork.Tutor.Get(
                u => u.TutorId == id);

            if (tutor == null)
            {
                return NotFound();
            }

            _unitOfWork.Tutor.Remove(tutor);
            _unitOfWork.Save();

            TempData["success"] =
                "Tutor deleted successfully.";

            return RedirectToAction("Index");
        }

        private void LoadSubjectList()
        {
            IEnumerable<SelectListItem> subjectList =
                _unitOfWork.Subject.GetAll()
                .Select(subject => new SelectListItem
                {
                    Text = subject.Name,
                    Value = subject.SubjectId.ToString()
                });

            ViewBag.SubjectList = subjectList;
        }
    }
}