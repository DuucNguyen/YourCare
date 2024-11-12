using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YourCare_Application.Models;
using YourCare_Application.Repository.Interfaces;

namespace YourCare_Application.Pages.Admin.Patient
{
    public class AppointmentListModel : PageModel
    {

        private readonly IUserRepository _userRepo;
        private readonly ITimetableRepository _timetableRepo;
        private readonly IPatientProfileRepository _patitentProfileRepo;
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        public AppointmentListModel(
           IUserRepository userRepository,
           ISpecializationRepository speRepo,
           IDoctorSpecializationRepository docSpeRepo,
           ITimetableRepository timetableRepo,
           UserManager<ApplicationUser> userManager,
           IPatientProfileRepository patitentProfileRepo,
           IAppointmentRepository appointmentRepo
           )
        {
            _userRepo = userRepository;
            _timetableRepo = timetableRepo;
            _userManager = userManager;
            _patitentProfileRepo = patitentProfileRepo;
            _appointmentRepo = appointmentRepo;
        }
        public List<Models.Appointment> Appointments { get; set; } = new List<Models.Appointment>();

        [BindProperty]
        public Models.Appointment Appointment { get; set; } = new Models.Appointment();

        public async Task<IActionResult> OnGet(int? appointmentId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Redirect("/Error");

            Appointments = await _appointmentRepo.GetAllByUserId(user.Id);

            if (appointmentId == null) return Page();
            Appointment = await _appointmentRepo.GetById(appointmentId ?? default(Int32));
            if (Appointment == null) return Redirect("/Error");
            Appointment.Doctor.AvatarString = Appointment.Doctor.Avatar != null ? $"data:image/png;base64,{Convert.ToBase64String(Appointment.Doctor.Avatar)}" : "";

            ViewData["chosenAppointment"] = Appointment.Id;
            return Page();
        }
    }
}
