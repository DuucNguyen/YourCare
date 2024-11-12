using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Numerics;
using YourCare_Application.Models;
using YourCare_Application.Repository.Interfaces;

namespace YourCare_Application.Repository
{
    public class UserRepository : IUserRepository
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private ILogger<UserRepository> _logger;
        private IDoctorSpecializationRepository _IdoctorSpecializationRepo;


        public UserRepository(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<UserRepository> logger,
            IDoctorSpecializationRepository doctorSpecializationRepo
            )
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
            _IdoctorSpecializationRepo = doctorSpecializationRepo;
        }



        public async Task<bool> Add(ApplicationUser request)
        {
            try
            {
                var result = await _userManager.CreateAsync(request);

                if (result.Succeeded)
                {
                    await _context.UserRoles.AddAsync(new IdentityUserRole<string>
                    {
                        UserId = request.Id,
                        RoleId = request.RoleId
                    });
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    foreach (var e in result.Errors)
                    {
                        Console.WriteLine(e.Description);
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error_Create: " + ex.Message + " -> " + ex.StackTrace);
                return false;
            }
        }

        public async Task<bool> Deactivate(ApplicationUser request)
        {
            try
            {
                var find = await _context.ApplicationUser.FirstOrDefaultAsync(x => x.Id == request.Id);
                if (find == null) return false;

                var userRoleFind = _context.UserRoles.FirstOrDefaultAsync(x => x.UserId == request.Id).Result;
                var role = _context.Roles.FirstOrDefaultAsync(x => x.Id == userRoleFind.RoleId).Result;

                if (userRoleFind != null && role.Name.Equals("Doctor"))
                {

                    var dcoSpes = _IdoctorSpecializationRepo.GetAll().Where(x => x.DoctorID == find.Id);
                    if (dcoSpes.Any())
                    {
                        foreach (var item in dcoSpes)
                        {
                            _IdoctorSpecializationRepo.Delete(item);
                        }
                    }
                }

                find.IsActive = false;
                _context.Update(find);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error_DeleteDoctor: " + ex.Message + " -> " + ex.StackTrace);
                return false;
            }
        }

        public List<ApplicationUser> GetAll()
        {
            try
            {
                return _context.Users.Where(x => x.IsActive == true).ToList();
            }
            catch (Exception ex)
            {
                return new List<ApplicationUser>();
            }
        }

        public async Task<ApplicationUser> GetById(string id)
        {
            try
            {
                return await _context.Users
                    .SingleOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ApplicationUser> GetByEmail(string email)
        {
            try
            {
                var find = await _context.Users
                    .SingleOrDefaultAsync(x => x.Email == email);
                return find;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> Update(ApplicationUser request)
        {
            try
            {
                var find = await _context.Users.FindAsync(request.Id);
                if (find == null) return false;

                find = request;

                _context.Update(find);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<Doctor> GetAllDoctor()
        {
            try
            {
                var doctorId = _context.Roles.FirstOrDefault(x => x.Name.Equals("Doctor"))?.Id;
                var doctors = _context.UserRoles.Where(x => x.RoleId == doctorId)
                    .Select(x => x.UserId)
                    .ToList();
                if (doctorId == null) return null;

                return _context.Users
                    .Where(x => doctors.Contains(x.Id))
                    .OfType<Doctor>()
                    .ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Doctor> GetDoctorById(string id)
        {
            try
            {
                var find = await _context.Users
                    .OfType<Doctor>()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (find != null) return find;

                throw new Exception("Doctor not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public List<ApplicationUser> GetAllUser()
        {
            try
            {
                var patientRole = _context.Roles.FirstOrDefault(x => x.Name == "Patient");
                if (patientRole == null)
                {
                    // Handle the case where the Patient role does not exist
                    return new List<ApplicationUser>();
                }
                var patients = _context.Users
                    .Where(user => _context.UserRoles
                        .Any(ur => ur.UserId == user.Id && ur.RoleId == patientRole.Id))
                    .ToList();
                return patients;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
