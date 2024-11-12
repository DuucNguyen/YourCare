using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YourCare_Application.Models;
using YourCare_Application.Repository.Interfaces;

namespace YourCare_Application.Pages.TrangChuBacSi
{
    [Authorize(Policy = "AdminOrDoctor")]
    public class ThongTinLichKhamModel : PageModel
    {
        private readonly IUserRepository _userRepo;
        private readonly ISpecializationRepository _speRepo;
        private readonly IDoctorSpecializationRepository _docSpeRepo;
        private readonly ITimetableRepository _timetableRepo;
        private readonly IPatientProfileRepository _patitentProfileRepo;
        private readonly IAppointmentRepository _appointmentRepo;
        public ThongTinLichKhamModel(
           IUserRepository userRepository,
           ISpecializationRepository speRepo,
           IDoctorSpecializationRepository docSpeRepo,
           ITimetableRepository timetableRepo,
           IPatientProfileRepository patitentProfileRepo,
           IAppointmentRepository appointmentRepo
           )
        {
            _userRepo = userRepository;
            _speRepo = speRepo;
            _docSpeRepo = docSpeRepo;
            _timetableRepo = timetableRepo;
            _patitentProfileRepo = patitentProfileRepo;
            _appointmentRepo = appointmentRepo;
        }

        public List<Appointment> Appointments { get; set; } = new List<Appointment>();

        public async Task<IActionResult> OnGet(string doctorId, string? txtDate)
        {
            if (string.IsNullOrEmpty(doctorId)) return Redirect("/Error");
            Appointments = await _appointmentRepo.GetAllByDoctorId(doctorId);

            if (!string.IsNullOrEmpty(txtDate))
            {
                var date = DateTime.Parse(txtDate);
                Appointments = Appointments.Where(x => x.TimeTable.Date.Date == date.Date).ToList();
                ViewData["chosenDate"] = date.Date.ToString("yyyy-MM-dd");
            }
            else
            {
                Appointments = Appointments.Where(x => x.TimeTable.Date.Date == DateTime.Now.Date).ToList();
                ViewData["chosenDate"] = DateTime.Now.Date.ToString("yyyy-MM-dd");
            }

            ViewData["doctorId"] = doctorId;
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            return Page();
        }
    }
}
