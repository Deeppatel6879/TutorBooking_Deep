using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TutorBooking.DataAccess.Repository.IRepository;
using TutorBooking.Models;

namespace TutorBooking_Deep.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // -------------------------------------------------
        // BOOKING LIST
        // -------------------------------------------------

        public IActionResult Index()
        {
            IEnumerable<Booking> bookingList =
                _unitOfWork.Booking.GetAll(
                    includeProperties: "Tutor");

            string? currentEmail = User.Identity?.Name;

            // Students only see their own bookings
            if (User.IsInRole("Student"))
            {
                bookingList = bookingList.Where(
                    b => b.StudentEmail == currentEmail);
            }

            // Tutors only see bookings assigned to them
            else if (User.IsInRole("Tutor"))
            {
                Tutor? tutor = _unitOfWork.Tutor.Get(
                    t => t.Email == currentEmail);

                if (tutor == null)
                {
                    bookingList = new List<Booking>();
                }
                else
                {
                    bookingList = bookingList.Where(
                        b => b.TutorId == tutor.TutorId);
                }
            }

            // Admin sees all bookings

            return View(bookingList);
        }

        // -------------------------------------------------
        // CREATE BOOKING
        // -------------------------------------------------

        [Authorize(Roles = "Student")]
        public IActionResult Create()
        {
            LoadTutorList();

            return View();
        }

        [Authorize(Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Booking booking)
        {
            // Automatically use the student's logged-in email
            booking.StudentEmail =
                User.Identity?.Name ?? string.Empty;

            // StudentEmail is not entered in the form,
            // so remove the original model-binding validation result
            ModelState.Remove(nameof(Booking.StudentEmail));

            // Prevent bookings in the past
            if (booking.BookingDate.Date < DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(Booking.BookingDate),
                    "Booking date cannot be in the past.");
            }

            if (ModelState.IsValid)
            {
                booking.Status = "Pending";

                _unitOfWork.Booking.Add(booking);
                _unitOfWork.Save();

                TempData["success"] =
                    "Booking created successfully.";

                return RedirectToAction(nameof(Index));
            }

            LoadTutorList();

            return View(booking);
        }

        // -------------------------------------------------
        // EDIT BOOKING - GET
        // -------------------------------------------------

        [Authorize(Roles = "Student,Admin")]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Booking? booking =
                _unitOfWork.Booking.Get(
                    b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            // Students cannot edit somebody else's booking
            if (User.IsInRole("Student") &&
                booking.StudentEmail != User.Identity?.Name)
            {
                return Forbid();
            }

            LoadTutorList();

            return View(booking);
        }

        // -------------------------------------------------
        // EDIT BOOKING - POST
        // -------------------------------------------------

        [Authorize(Roles = "Student,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Booking booking)
        {
            Booking? existingBooking =
                _unitOfWork.Booking.Get(
                    b => b.BookingId == booking.BookingId);

            if (existingBooking == null)
            {
                return NotFound();
            }

            // Students cannot edit another student's booking
            if (User.IsInRole("Student") &&
                existingBooking.StudentEmail != User.Identity?.Name)
            {
                return Forbid();
            }

            // StudentEmail is not edited through the form
            ModelState.Remove(nameof(Booking.StudentEmail));

            // Status is controlled separately, not through
            // the student Edit Booking form
            ModelState.Remove(nameof(Booking.Status));

            if (booking.BookingDate.Date < DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(Booking.BookingDate),
                    "Booking date cannot be in the past.");
            }

            if (ModelState.IsValid)
            {
                // Only update fields the student/admin
                // should be able to change here
                existingBooking.StudentName =
                    booking.StudentName;

                existingBooking.TutorId =
                    booking.TutorId;

                existingBooking.BookingDate =
                    booking.BookingDate;

                existingBooking.BookingTime =
                    booking.BookingTime;

                existingBooking.Notes =
                    booking.Notes;

                // StudentEmail and Status stay unchanged

                _unitOfWork.Booking.Update(
                    existingBooking);

                _unitOfWork.Save();

                TempData["success"] =
                    "Booking updated successfully.";

                return RedirectToAction(nameof(Index));
            }

            LoadTutorList();

            return View(booking);
        }

        // -------------------------------------------------
        // DELETE BOOKING - GET
        // -------------------------------------------------

        [Authorize(Roles = "Student,Admin")]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Booking? booking =
                _unitOfWork.Booking.Get(
                    b => b.BookingId == id,
                    includeProperties: "Tutor");

            if (booking == null)
            {
                return NotFound();
            }

            // Students cannot delete another student's booking
            if (User.IsInRole("Student") &&
                booking.StudentEmail != User.Identity?.Name)
            {
                return Forbid();
            }

            return View(booking);
        }

        // -------------------------------------------------
        // DELETE BOOKING - POST
        // -------------------------------------------------

        [Authorize(Roles = "Student,Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Booking? booking =
                _unitOfWork.Booking.Get(
                    b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            // Students cannot delete another student's booking
            if (User.IsInRole("Student") &&
                booking.StudentEmail != User.Identity?.Name)
            {
                return Forbid();
            }

            _unitOfWork.Booking.Remove(booking);
            _unitOfWork.Save();

            TempData["success"] =
                "Booking deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        // -------------------------------------------------
        // LOAD AVAILABLE TUTORS
        // -------------------------------------------------

        private void LoadTutorList()
        {
            IEnumerable<SelectListItem> tutorList =
                _unitOfWork.Tutor
                    .GetAll(includeProperties: "Subject")
                    .Where(t => t.IsAvailable)
                    .Select(t => new SelectListItem
                    {
                        Text = t.Name + " - " +
                               (t.Subject?.Name ?? "No Subject"),

                        Value = t.TutorId.ToString()
                    });

            ViewBag.TutorList = tutorList;
        }
        // -------------------------------------------------
        // ACCEPT BOOKING
        // -------------------------------------------------

        [Authorize(Roles = "Tutor,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Accept(int id)
        {
            Booking? booking = _unitOfWork.Booking.Get(
                b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Tutor"))
            {
                string? currentEmail = User.Identity?.Name;

                Tutor? tutor = _unitOfWork.Tutor.Get(
                    t => t.Email == currentEmail);

                if (tutor == null ||
                    booking.TutorId != tutor.TutorId)
                {
                    return Forbid();
                }
            }

            booking.Status = "Accepted";

            _unitOfWork.Booking.Update(booking);
            _unitOfWork.Save();

            TempData["success"] =
                "Booking accepted successfully.";

            return RedirectToAction(nameof(Index));
        }


        // -------------------------------------------------
        // REJECT / CANCEL BOOKING
        // -------------------------------------------------

        [Authorize(Roles = "Tutor,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancel(int id)
        {
            Booking? booking = _unitOfWork.Booking.Get(
                b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Tutor"))
            {
                string? currentEmail = User.Identity?.Name;

                Tutor? tutor = _unitOfWork.Tutor.Get(
                    t => t.Email == currentEmail);

                if (tutor == null ||
                    booking.TutorId != tutor.TutorId)
                {
                    return Forbid();
                }
            }

            booking.Status = "Cancelled";

            _unitOfWork.Booking.Update(booking);
            _unitOfWork.Save();

            TempData["success"] =
                "Booking cancelled.";

            return RedirectToAction(nameof(Index));
        }


        // -------------------------------------------------
        // COMPLETE BOOKING
        // -------------------------------------------------

        [Authorize(Roles = "Tutor,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Complete(int id)
        {
            Booking? booking = _unitOfWork.Booking.Get(
                b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Tutor"))
            {
                string? currentEmail = User.Identity?.Name;

                Tutor? tutor = _unitOfWork.Tutor.Get(
                    t => t.Email == currentEmail);

                if (tutor == null ||
                    booking.TutorId != tutor.TutorId)
                {
                    return Forbid();
                }
            }

            booking.Status = "Completed";

            _unitOfWork.Booking.Update(booking);
            _unitOfWork.Save();

            TempData["success"] =
                "Booking marked as completed.";

            return RedirectToAction(nameof(Index));
        }
    }
}