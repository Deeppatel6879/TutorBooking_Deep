using TutorBooking.Models;

namespace TutorBooking.DataAccess.Repository.IRepository
{
    public interface IBookingRepository : IRepository<Booking>
    {
        void Update(Booking booking);
    }
}