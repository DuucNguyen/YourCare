using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using YourCare_Application.Models;
using YourCare_Application.Repository.Interfaces;

namespace YourCare_Application.Repository
{
    public class SpecializationRepository : ISpecializationRepository
    {
        private ApplicationDbContext _context;
        private IDoctorSpecializationRepository _docSpeRepo;

        public SpecializationRepository(
            ApplicationDbContext context,
            IDoctorSpecializationRepository docSpeRepo
            )
        {
            _context = context;
            _docSpeRepo = docSpeRepo;
        }


        public async Task<bool> Add(Specialization request)
        {
            try
            {
                var find = await GetById(request.Id);
                if (find != null) return false;

                await _context.Specializations.AddAsync(request);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> Delete(Specialization request)
        {
            try
            {
                var find = await GetById(request.Id);
                if (find == null) return false;

                //delete from doctorspe
                var docSpes = _docSpeRepo.GetAllBySpeId(find.Id);
                if (docSpes.Any())
                {
                    foreach (var docSpe in docSpes)
                    {
                        await _docSpeRepo.Delete(docSpe);
                    }
                }

                _context.Specializations.Remove(find);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<Specialization> GetAll()
        {
            return _context.Specializations.ToList();
        }

        public async Task<Specialization> GetById(int id)
        {
            try
            {
                var isExist = _context.Specializations.SingleOrDefaultAsync(x => x.Id == id).Result;
                if (isExist == null) return null;

                return isExist;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> Update(Specialization request)
        {
            try
            {
                var find = await GetById(request.Id);
                if (find != null) return false;

                find = request;

                _context.Specializations.Update(find);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
