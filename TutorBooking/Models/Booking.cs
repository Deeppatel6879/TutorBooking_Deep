using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TutorBooking.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Student Name")]
        public string StudentName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Student Email")]
        public string StudentEmail { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Please select a tutor.")]
        [Display(Name = "Tutor")]
        public int TutorId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Booking Time")]
        public TimeSpan BookingTime { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        [MaxLength(500)]
        public string? Notes { get; set; }

        [ForeignKey("TutorId")]
        public Tutor? Tutor { get; set; }
    }
}