using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YourCare_Application.Models;
using YourCare_Application.Repository.Interfaces;

namespace YourCare_Application.Pages
{
    //[Authorize(Policy = "AdminOrPatient")]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IUserRepository _userRepository;
        private readonly ISpecializationRepository _speRepo;
        private readonly IDoctorSpecializationRepository _docSpeRepo;

        public IndexModel(ILogger<IndexModel> logger ,
            IUserRepository userRepository,
            ISpecializationRepository speRepo,
            IDoctorSpecializationRepository docSpeRepo)
        {
            _logger = logger;
            _userRepository = userRepository;
            _speRepo = speRepo;
            _docSpeRepo = docSpeRepo;
        }

        public List<Models.Doctor> Doctors { get; set; }
        public List<Models.Specialization> Specializations { get; set; }

        public void OnGet()
        {
            Doctors = _userRepository.GetAllDoctor()
                .Where(x=>x.IsActive==true)
                .Take(10).ToList();
            Specializations = _speRepo.GetAll()
                .Select(x => new Models.Specialization
                {
                    Id = x.Id,
                    Name = x.Name,
                    Image = x.Image,
                    ImageString = x.Image != null ? $"data:image/png;base64,{Convert.ToBase64String(x.Image)}" : "",
                })
                .ToList();

            foreach (var doctor in Doctors)
            {
                doctor.AvatarString = doctor.Avatar!=null 
                    ? $"data:image/png;base64,{Convert.ToBase64String(doctor.Avatar)}" 
                    : "";
                doctor.Specializations = _docSpeRepo.GetAllSpeByDoctorId(doctor.Id);
            }
        }
    }
}
