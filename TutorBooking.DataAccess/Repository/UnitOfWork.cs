using TutorBooking.DataAccess.Data;
using TutorBooking.DataAccess.Repository.IRepository;

namespace TutorBooking.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public ISubjectRepository Subject { get; private set; }

        public ITutorRepository Tutor { get; private set; }

        public IBookingRepository Booking { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;

            Subject = new SubjectRepository(_db);
            Tutor = new TutorRepository(_db);
            Booking = new BookingRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}