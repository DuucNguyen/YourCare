using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Data.SqlTypes;
using YourCare_Application.Models;
using YourCare_Application.Repository.Interfaces;

namespace YourCare_Application.Repository
{
    public class PatientProfileRepository : IPatientProfileRepository
    {
        private ApplicationDbContext _context;
        private IUserRepository _userRepo;

        public PatientProfileRepository(
            ApplicationDbContext context,
            IUserRepository userRepo
            )
        {
            _context = context;
            _userRepo = userRepo;
        }

        public async Task Add(PatientProfile request)
        {
            try
            {
                await _context.PatientProfiles.AddAsync(request);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task Delete(PatientProfile request)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<PatientProfile>> GetAllByUserId(string userID)
        {
            try
            {
                var result = new List<PatientProfile>();
                result = await _context.PatientProfiles
                    .Where(x => x.ApplicationUserID.Equals(userID))
                    .ToListAsync();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PatientProfile> GetById(string id)
        {

            try
            {
                return await _context.PatientProfiles.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task Update(PatientProfile request)
        {
            try
            {
                var find = await GetById(request.Id);
                if (find != null)
                {
                    find = request;

                    _context.PatientProfiles.Update(find);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Update failed: Profile not found.");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
