using YourCare_Application.Models;

namespace YourCare_Application.Repository.Interfaces
{
    public interface IDoctorSpecializationRepository
    {
        public Task<bool> Add(DoctorSpecialization request);
        public Task<bool> Delete(DoctorSpecialization request);
        public List<DoctorSpecialization> GetAll();
        public List<Specialization> GetAllSpeByDoctorId(string doctorId);
        public List<DoctorSpecialization> GetAllBySpeId(int speId);
    }
}
