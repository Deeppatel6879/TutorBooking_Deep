using TutorBooking.DataAccess.Data;
using TutorBooking.DataAccess.Repository.IRepository;
using TutorBooking.Models;

namespace TutorBooking.DataAccess.Repository
{
    public class SubjectRepository : Repository<Subject>, ISubjectRepository
    {
        private readonly ApplicationDbContext _db;

        public SubjectRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Subject subject)
        {
            _db.Subjects.Update(subject);
        }
    }
}