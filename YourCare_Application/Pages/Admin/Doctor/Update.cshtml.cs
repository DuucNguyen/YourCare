using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using YourCare_Application.Models;
using System.Numerics;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using YourCare_Application.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace YourCare_Application.Pages.Admin.Doctor
{
    [Authorize(Policy = "AdminOnly")]
    public class UpdateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly ISpecializationRepository _specializationRepository;
        private readonly IDoctorSpecializationRepository _doctorSpecializationRepository;
        public UpdateModel(IUserRepository userRepository,
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
            public string Id { get; set; }

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
            public IFormFile? AvatarFile { get; set; }

            public string? AvatarString { get; set; }

            [DisplayName("Titles")]
            public string DoctorTitle { get; set; }

            [DisplayName("Description")]
            public string? DoctorDescription { get; set; }

            [DisplayName("Experience years")]
            public int? Experience { get; set; }

        }

        public void OnGet(string id)
        {
            var specializations = _specializationRepository.GetAll();

            if (string.IsNullOrEmpty(id)) return;

            var doctor = _userRepository.GetById(id).Result as Models.Doctor;
            if (doctor == null) return;

            Input = new InputModel();
            Input.Id = id;

            if (!string.IsNullOrEmpty(doctor.Name)) Input.Name = doctor.Name;
            if (!string.IsNullOrEmpty(doctor.Address)) Input.Address = doctor.Address;
            if (!string.IsNullOrEmpty(doctor.DoctorTitle)) Input.DoctorTitle = doctor.DoctorTitle;
            if (!string.IsNullOrEmpty(doctor.DoctorDescription)) Input.DoctorDescription = doctor.DoctorDescription;
            if (doctor.Experience != null) Input.Experience = doctor.Experience;
            if (doctor.Gender != null) Input.Gender = bool.Parse(doctor.Gender.ToString());
            if (doctor.Dob != null) Input.Dob = DateTime.Parse(doctor.Dob.ToString());
            if (doctor.Avatar != null) Input.AvatarString = $"data:image/png;base64,{Convert.ToBase64String(doctor.Avatar)}";

            Input.Email = doctor.Email;
            Input.PhoneNumber = doctor.PhoneNumber;

            var doctorSpes = _doctorSpecializationRepository.GetAll()
                .Where(x => x.DoctorID == doctor.Id)
                .Select(x => x.SpecializationID)
                .ToList();

            var selectedSpecs = specializations
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    IsChecked = doctorSpes.Contains(x.Id)
                }).ToList();

            ViewData["specializations"] = selectedSpecs;
            ViewData["selectedSpec"] = selectedSpecs;
        }

        public async Task<IActionResult> OnPost()
        {
            StatusMessage = "Error : Unexpected error. Try again !";
            var specializations = _specializationRepository.GetAll();

            var doctorSpes = _doctorSpecializationRepository.GetAll()
               .Where(x => x.DoctorID == Input.Id)
               .Select(x => x.SpecializationID)
               .ToList();

            var selectedSpecs = specializations
               .Select(x => new
               {
                   x.Id,
                   x.Name,
                   IsChecked = doctorSpes.Contains(x.Id)
               }).ToList();
            ViewData["specializations"] = selectedSpecs;

            var findDoctor = _userRepository
                .GetById(Input.Id).Result as Models.Doctor;

            if (findDoctor == null) return Page();

            if (!ModelState.IsValid) return Page();

            var isEmailExist = _userRepository.GetAll().Any(x => x.Email == Input.Email);
            if (isEmailExist && findDoctor.Email != Input.Email) return Page();

            var isPhoneExist = _userRepository.GetAll().Any(x => x.PhoneNumber == Input.PhoneNumber);
            if (isPhoneExist && findDoctor.PhoneNumber != Input.PhoneNumber) return Page();

            var roleDoctorId = _context.Roles.FirstOrDefault(x => x.Name.Equals("Doctor"))?.Id;

            findDoctor.Name = Input.Name;
            findDoctor.Email = Input.Email;
            findDoctor.UserName = Input.Email;
            findDoctor.PhoneNumber = Input.PhoneNumber;
            findDoctor.Dob = Input.Dob;
            findDoctor.Gender = Input.Gender;
            findDoctor.Address = Input.Address;

            findDoctor.DoctorTitle = Input.DoctorTitle;
            findDoctor.DoctorDescription = Input.DoctorDescription;
            findDoctor.Experience = Input.Experience;

            if (Input.AvatarFile != null && Input.AvatarFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    Input.AvatarFile.CopyTo(ms);
                    var avatarbytes = ms.ToArray();
                    findDoctor.Avatar = avatarbytes;
                }
            }

            var resultDoctor = _userRepository.Update(findDoctor);

            if (resultDoctor.Result)
            {
                var selectedSpecializations = Request.Form["selectedSpecializations"];

                foreach (var spe in selectedSpecializations)
                {
                    var newDocSpe = new DoctorSpecialization
                    {
                        DoctorID = Input.Id,
                        SpecializationID = int.Parse(spe.ToString())
                    };

                    if (!doctorSpes.Contains(int.Parse(spe.ToString())))
                    {
                        await _doctorSpecializationRepository.Add(newDocSpe);
                    }
                }
                var exceptIds = doctorSpes.Select(x => x.ToString()).ToList().Except(selectedSpecializations);

                foreach (var exceptId in exceptIds)
                {
                    var newDocSpe = new DoctorSpecialization
                    {
                        DoctorID = Input.Id,
                        SpecializationID = int.Parse(exceptId.ToString())
                    };
                    await _doctorSpecializationRepository.Delete(newDocSpe);
                }
            }

            StatusMessage = "Updated successfully !";

            return Page();
        }
    }
}
