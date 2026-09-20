using Microsoft.AspNetCore.Mvc;
using TutorBooking.DataAccess.Repository.IRepository;
using TutorBooking.Models;
using Microsoft.AspNetCore.Authorization;

namespace TutorBooking_Deep.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SubjectController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubjectController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Subject> subjectList = _unitOfWork.Subject.GetAll();

            return View(subjectList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Subject subject)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Subject.Add(subject);
                _unitOfWork.Save();

                TempData["success"] = "Subject created successfully.";

                return RedirectToAction("Index");
            }

            return View(subject);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Subject? subject = _unitOfWork.Subject.Get(
                u => u.SubjectId == id);

            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Subject subject)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Subject.Update(subject);
                _unitOfWork.Save();

                TempData["success"] = "Subject updated successfully.";

                return RedirectToAction("Index");
            }

            return View(subject);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Subject? subject = _unitOfWork.Subject.Get(
                u => u.SubjectId == id);

            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            Subject? subject = _unitOfWork.Subject.Get(
                u => u.SubjectId == id);

            if (subject == null)
            {
                return NotFound();
            }

            _unitOfWork.Subject.Remove(subject);
            _unitOfWork.Save();

            TempData["success"] = "Subject deleted successfully.";

            return RedirectToAction("Index");
        }
    }
}