using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using YourCare_Application.Constants;
using YourCare_Application.Repository.Interfaces;

namespace YourCare_Application.Pages.Admin.Appointment
{
    public class AppointmentDetailModel : PageModel
    {

        private readonly IUserRepository _userRepo;
        private readonly ITimetableRepository _timetableRepo;
        private readonly IPatientProfileRepository _patitentProfileRepo;
        private readonly IAppointmentRepository _appointmentRepo;
        public AppointmentDetailModel(
           IUserRepository userRepository,
           ITimetableRepository timetableRepo,
           IPatientProfileRepository patitentProfileRepo,
           IAppointmentRepository appointmentRepo
           )
        {
            _userRepo = userRepository;
            _timetableRepo = timetableRepo;
            _patitentProfileRepo = patitentProfileRepo;
            _appointmentRepo = appointmentRepo;
        }
        public Models.Doctor Doc { get; set; } = new Models.Doctor();
        public Models.Timetable TimeTable { get; set; } = new Models.Timetable();

        [BindProperty]
        public Models.Appointment Appointment { get; set; } = new Models.Appointment();
        public Models.PatientProfile PatientProfile { get; set; } = new Models.PatientProfile();


        public async Task<IActionResult> OnGet(int appointmentId)
        {
            try
            {
                Appointment = await _appointmentRepo.GetById(appointmentId);
                if (Appointment == null) return Redirect("/Error");
                Console.WriteLine(decimal.Parse(Appointment.TotalPrice.ToString("F0")));

                Doc = await _userRepo.GetDoctorById(Appointment.DoctorID);
                Appointment.Doctor.AvatarString = Appointment.Doctor.Avatar != null ? $"data:image/png;base64,{Convert.ToBase64String(Appointment.Doctor.Avatar)}" : "";


                TimeTable = await _timetableRepo.GetById(Appointment.TimetableID);
                PatientProfile = await _patitentProfileRepo.GetById(Appointment.PatientProfileID);

                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " - " + ex.StackTrace);
                return Redirect("/Error");
            }

        }

        
    }
}
