using YourCare_Application.Models;

namespace YourCare_Application.Repository.Interfaces
{
    public interface IPatientProfileRepository
    {

        public Task Add(PatientProfile request);
        public Task Update(PatientProfile request);
        public Task Delete(PatientProfile request);
        public Task<List<PatientProfile>> GetAllByUserId(string userID);
        public Task<PatientProfile> GetById(string id);


    }
}
