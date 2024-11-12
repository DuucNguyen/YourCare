using YourCare_Application.Models;

namespace YourCare_Application.Repository.Interfaces
{
    public interface IUserRepository
    {
        public Task<bool> Add(ApplicationUser request);
        public Task<bool> Update(ApplicationUser request);
        public Task<bool> Deactivate(ApplicationUser request);
        public List<ApplicationUser> GetAll();
        public List<Doctor> GetAllDoctor();
        public List<ApplicationUser> GetAllUser();
        public Task<ApplicationUser> GetById(string id);
        public Task<Doctor> GetDoctorById(string id);
        public Task<ApplicationUser> GetByEmail(string email);
    }
}
