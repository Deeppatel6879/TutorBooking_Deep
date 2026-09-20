using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TutorBooking.Models;

namespace TutorBooking.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Subject> Subjects => Set<Subject>();

        public DbSet<Tutor> Tutors => Set<Tutor>();

        public DbSet<Booking> Bookings => Set<Booking>();
    }
}