using YourCare_Application.Models;

namespace YourCare_Application.Repository.Interfaces
{
    public interface ISpecializationRepository
    {
        public Task<bool> Add(Specialization request);
        public Task<bool> Update(Specialization request);
        public Task<bool> Delete(Specialization request);
        public List<Specialization> GetAll();
        public Task<Specialization> GetById(int id);
    }
}
