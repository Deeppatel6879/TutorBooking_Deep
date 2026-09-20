using TutorBooking.DataAccess.Data;
using TutorBooking.DataAccess.Repository.IRepository;
using TutorBooking.Models;

namespace TutorBooking.DataAccess.Repository
{
    public class TutorRepository : Repository<Tutor>, ITutorRepository
    {
        private readonly ApplicationDbContext _db;

        public TutorRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Tutor tutor)
        {
            _db.Tutors.Update(tutor);
        }
    }
}