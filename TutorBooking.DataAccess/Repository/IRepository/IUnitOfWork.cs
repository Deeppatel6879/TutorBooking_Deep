namespace TutorBooking.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ISubjectRepository Subject { get; }

        ITutorRepository Tutor { get; }

        IBookingRepository Booking { get; }

        void Save();
    }
}