using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using YourCare_Application.Constants;
using YourCare_Application.Repository.Interfaces;

namespace YourCare_Application.Pages.TrangChuBacSi
{
    [Authorize(Policy = "AdminOrDoctor")]
    public class HoSoLichKhamModel : PageModel
    {

        private readonly IUserRepository _userRepo;
        private readonly ITimetableRepository _timetableRepo;
        private readonly IPatientProfileRepository _patitentProfileRepo;
        private readonly IAppointmentRepository _appointmentRepo;
        public HoSoLichKhamModel(
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

        public SelectList Prices;

        public async Task<IActionResult> OnGet(int appointmentId)
        {
            try
            {
                Appointment = await _appointmentRepo.GetById(appointmentId);
                if (Appointment == null) return Redirect("/Error");
                Console.WriteLine(decimal.Parse(Appointment.TotalPrice.ToString("F0")));
                Prices = new SelectList(PriceConstant.Prices, decimal.Parse(Appointment.TotalPrice.ToString("F0")));

                Doc = await _userRepo.GetDoctorById(Appointment.DoctorID);
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

        public async Task<IActionResult> OnPost()
        {
            try
            {
                var appointment = await _appointmentRepo.GetById(Appointment.Id);
                if (appointment == null) return Redirect("/Error");
                var txtPrice = Request.Form["txtPrice"];

                appointment.DoctorNote = Appointment.DoctorNote;
                appointment.DoctorDianosis = Appointment.DoctorDianosis;
                appointment.TotalPrice = decimal.Parse(txtPrice);

                appointment.Status = StatusConstant.Status.Đã_hoàn_thành;

                Doc = await _userRepo.GetDoctorById(appointment.DoctorID);

                await _appointmentRepo.Update(appointment);

                return RedirectToPage("/TrangChuBacSi/ThongTinLichKham", new { doctorId = Doc.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " - " + ex.StackTrace);
                Redirect("/Error");
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAbsent()
        {
            try
            {
                var appointment = await _appointmentRepo.GetById(Appointment.Id);
                if (appointment == null) return Redirect("/Error");

                appointment.TotalPrice = 0;
                appointment.Status = StatusConstant.Status.Vắng;

                Doc = await _userRepo.GetDoctorById(appointment.DoctorID);
                await _appointmentRepo.Update(appointment);

                ViewData["msg"] = "Cập nhật thành công !";

                return RedirectToPage("/TrangChuBacSi/ThongTinLichKham", new { doctorId = Doc.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " - " + ex.StackTrace);
                Redirect("/Error");
            }
            return Page();
        }
    }
}
