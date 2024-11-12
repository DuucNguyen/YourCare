using Microsoft.EntityFrameworkCore;
using YourCare_Application.Models;
using YourCare_Application.Repository.Interfaces;

namespace YourCare_Application.Repository
{
    public class DoctorSpecializationRepository : IDoctorSpecializationRepository
    {
        private ApplicationDbContext _context;

        public DoctorSpecializationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Add(DoctorSpecialization request)
        {
            try
            {
                var isExist = _context.DoctorSpecializations.AnyAsync(x => x.DoctorID == request.DoctorID
                                && x.SpecializationID == request.SpecializationID);
                if (isExist.Result) return false;

                await _context.DoctorSpecializations.AddAsync(request);
                await _context.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> Delete(DoctorSpecialization request)
        {
            try
            {
                var isExist = await _context.DoctorSpecializations.FirstOrDefaultAsync(x => x.DoctorID == request.DoctorID
                && x.SpecializationID == request.SpecializationID);
                if (isExist==null) return false;

                _context.DoctorSpecializations.Remove(isExist);
                await _context.SaveChangesAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message + " -> " + ex.StackTrace);
                return false;
            }
        }

        public List<DoctorSpecialization> GetAll()
        {
            return _context.DoctorSpecializations.ToList();
        }

        public List<DoctorSpecialization> GetAllBySpeId(int speId)
        {
            var result = new List<DoctorSpecialization>();
            try
            {
                result = _context.DoctorSpecializations.Where(x => x.SpecializationID == speId).ToList();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message + " -> " + ex.StackTrace);
                return result;
            }
        }

        public List<Specialization> GetAllSpeByDoctorId(string doctorId)
        {
            return _context.DoctorSpecializations
                .Include(x => x.Specialization)
                .Where(x=>x.DoctorID == doctorId)
                .Select(x=> new Specialization
                {
                    Id = x.SpecializationID,
                    Name = x.Specialization.Name
                }).ToList();
        }
    }
}
