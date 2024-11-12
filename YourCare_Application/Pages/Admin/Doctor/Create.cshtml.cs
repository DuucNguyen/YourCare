using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using YourCare_Application.Models;
using static YourCare_Application.Models.ApplicationUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using YourCare_Application.Repository.Interfaces;

namespace YourCare_Application.Pages.Admin.Doctor
{
    [Authorize(Policy = "AdminOnly")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly ISpecializationRepository _specializationRepository;
        private readonly IDoctorSpecializationRepository _doctorSpecializationRepository;
        public CreateModel(IUserRepository userRepository,
            ApplicationDbContext context,
            ISpecializationRepository specializationRepository,
            IDoctorSpecializationRepository doctorSpecializationRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _specializationRepository = specializationRepository;
            _doctorSpecializationRepository = doctorSpecializationRepository;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty]
        public string? StatusMessage { get; set; }

        public class InputModel
        {
            [Display(Name = "Full Name")]
            [MaxLength(256, ErrorMessage = "Name must be less than 256 characters")]
            public string Name { get; set; }

            [EmailAddress]
            [Display(Name = "Email")]
            [MaxLength(256, ErrorMessage = "Email must be less than 256 characters")]
            public string Email { get; set; }

            [Phone]
            [Display(Name = "Phone Number")]
            [MaxLength(256, ErrorMessage = "PhoneNumber must be less than 256 characters")]
            public string? PhoneNumber { get; set; }

            public bool Gender { get; set; }

            [DisplayName("Date of Birth")]
            public DateTime Dob { get; set; }

            [MaxLength(12, ErrorMessage = "Invalid format !")]
            [DisplayName("IdentityNumber")]
            public string? IdentityNumber { get; set; }

            [MaxLength(256, ErrorMessage = "Address must be less than 256 characters")]
            public string Address { get; set; }

            [DisplayName("Avatar")]
            public IFormFile AvatarFile { get; set; }


            [DisplayName("Titles")]
            public string DoctorTitle { get; set; }


            [DisplayName("Description")]
            public string? DoctorDescription { get; set; }

            [DisplayName("Experience years")]
            public int? Experience { get; set; }

        }

        public void OnGet()
        {
            var count = _context.Roles.ToList();
            ViewData["specializations"] = _specializationRepository.GetAll();
        }
        public async Task<IActionResult> OnPost()
        {

            StatusMessage = "Error : Unexpected error. Try again !";


            ViewData["specializations"] = _specializationRepository.GetAll();

            if (!ModelState.IsValid) return Page();

            var isEmailExist = _userRepository.GetAll().Any(x => x.Email == Input.Email);
            if (isEmailExist) return Page();

            var isPhoneExist = _userRepository.GetAll().Any(x => x.PhoneNumber == Input.PhoneNumber);
            if (isPhoneExist) return Page();

            var roleDoctorId = _context.Roles.FirstOrDefault(x => x.Name.Equals("Doctor"))?.Id;

            var selectedSpecializations = Request.Form["selectedSpecializations"];
            var newDocId = Guid.NewGuid().ToString();


            var user = new Models.Doctor
            {
                Id = newDocId,
                Name = Input.Name,
                Email = Input.Email,
                UserName = Input.Email,
                PhoneNumber = Input.PhoneNumber,
                Dob = Input.Dob,
                Gender = Input.Gender,
                Address = Input.Address,

                DoctorTitle = Input.DoctorTitle,
                DoctorDescription = Input.DoctorDescription,
                Experience = Input.Experience,

                RoleId = roleDoctorId,
                IsActive = true
            };

            if (Input.AvatarFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    Input.AvatarFile.CopyTo(ms);
                    var avatarbytes = ms.ToArray();
                    user.Avatar = avatarbytes;
                }
            }

            var resultDoctor = await _userRepository.Add(user);

            if (resultDoctor)
            {
                foreach (var spe in selectedSpecializations)
                {
                    var newDocSpe = new DoctorSpecialization
                    {
                        DoctorID = newDocId,
                        SpecializationID = int.Parse(spe.ToString())
                    };
                    await _doctorSpecializationRepository.Add(newDocSpe);
                }
            }

            StatusMessage = "Updated successfully !";
            return Page();
        }
    }
}
