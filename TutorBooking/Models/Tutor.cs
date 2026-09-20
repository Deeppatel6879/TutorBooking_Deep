using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TutorBooking.Models
{
    public class Tutor
    {
        [Key]
        public int TutorId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Bio { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a subject.")]
        public int SubjectId { get; set; }

        public bool IsAvailable { get; set; }

        [ForeignKey("SubjectId")]
        public Subject? Subject { get; set; }
    }
}