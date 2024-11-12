using YourCare_Application.Models;

namespace YourCare_Application.Repository.Interfaces
{
    public interface IAppointmentRepository
    {
        public Task Add(Appointment appointment);
        public Task Update(Appointment appointment);
        public Task Delete(Appointment appointment);
        public Task CancelAppointment(Appointment appointment);
        public Task<Appointment> GetById(int id);
        public Task<List<Appointment>> GetAllByDoctorId(string doctorId);
        public Task<List<Appointment>> GetAll();
        public Task<List<Appointment>> GetAllByUserId(string userId);


    }
}
