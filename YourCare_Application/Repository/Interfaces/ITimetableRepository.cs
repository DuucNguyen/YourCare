using YourCare_Application.Models;

namespace YourCare_Application.Repository.Interfaces
{
    public interface ITimetableRepository
    {
        public Task Add(Timetable timetable);
        public Task AddRange(List<Timetable> timetables);
        public Task Update(Timetable timetable);
        public Task Delete(Timetable timetable);
        public Task Deactivate (int timeTaleId);
        public Task<Timetable> GetById(int id);
        public Task<List<Timetable>> GetAll();
        public Task<List<Timetable>> GetInRange(string doctorID, DateTime startDate, DateTime endDate);


    }
}
