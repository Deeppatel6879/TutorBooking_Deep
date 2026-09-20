using TutorBooking.DataAccess.Data;
using TutorBooking.DataAccess.Repository.IRepository;
using TutorBooking.Models;

namespace TutorBooking.DataAccess.Repository
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        private readonly ApplicationDbContext _db;

        public BookingRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Booking booking)
        {
            _db.Bookings.Update(booking);
        }
    }
}