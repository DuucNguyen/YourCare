using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql.Internal.TypeHandlers.LTreeHandlers;
using YourCare_Application.Models;
using YourCare_Application.Repository.Interfaces;

namespace YourCare_Application.Pages.DatKham
{
    [Authorize(Policy = "AdminOrPatient")]
    public class LichKhamModel : PageModel
    {
        private readonly IUserRepository _userRepo;
        private readonly ITimetableRepository _timetableRepo;
        private readonly IPatientProfileRepository _patitentProfileRepo;
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        public LichKhamModel(
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
        public List<Appointment> Appointments { get; set; } = new List<Appointment>();

        [BindProperty]
        public Appointment Appointment { get; set; } = new Appointment();

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

        public async Task<IActionResult> OnPostFeedback()
        {
            try
            {
                var find = await _appointmentRepo.GetById(Appointment.Id);
                if (find == null) return Page();

                var txtRating = Request.Form["txtRating"];
                var txtFeedback = Request.Form["txtFeedback"];

                find.PatientRating = !string.IsNullOrEmpty(txtRating) ? Int32.Parse(txtRating) : null;
                find.PatientFeedBack = !string.IsNullOrEmpty(txtFeedback) ? txtFeedback : "";

                await _appointmentRepo.Update(find);
                await OnGet(find.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " - " + ex.StackTrace);
            }
            return Page();
        }

        public async Task<IActionResult> OnPostCancel()
        {
            try
            {
                var find = await _appointmentRepo.GetById(Appointment.Id);
                if (find == null) return Page();
                find.Status = Constants.StatusConstant.Status.Đã_hủy;
                await _appointmentRepo.CancelAppointment(find);

                ViewData["msg"] = "Cập nhật thành công !";

                await OnGet(find.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " - " + ex.StackTrace);
                return Redirect("/Error");
            }

            return Page();
        }

    }
}
