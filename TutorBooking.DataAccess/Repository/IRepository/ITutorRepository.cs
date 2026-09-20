using TutorBooking.Models;

namespace TutorBooking.DataAccess.Repository.IRepository
{
    public interface ITutorRepository : IRepository<Tutor>
    {
        void Update(Tutor tutor);
    }
}