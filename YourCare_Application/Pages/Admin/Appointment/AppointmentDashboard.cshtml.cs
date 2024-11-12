using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YourCare_Application.Models;
using YourCare_Application.Repository.Interfaces;

namespace YourCare_Application.Pages.Admin
{
    public class AppointmentDashboardModel : PageModel
    {

        private readonly IUserRepository _userRepo;
        private readonly ISpecializationRepository _speRepo;
        private readonly IDoctorSpecializationRepository _docSpeRepo;
        private readonly ITimetableRepository _timetableRepo;
        private readonly IPatientProfileRepository _patitentProfileRepo;
        private readonly IAppointmentRepository _appointmentRepo;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AppointmentDashboardModel(
           IUserRepository userRepository,
           ISpecializationRepository speRepo,
           IDoctorSpecializationRepository docSpeRepo,
           ITimetableRepository timetableRepo,
           IPatientProfileRepository patitentProfileRepo,
           IAppointmentRepository appointmentRepo,
           UserManager<ApplicationUser> userManager
           )
        {
            _userRepo = userRepository;
            _speRepo = speRepo;
            _docSpeRepo = docSpeRepo;
            _timetableRepo = timetableRepo;
            _patitentProfileRepo = patitentProfileRepo;
            _appointmentRepo = appointmentRepo;
            _userManager = userManager;
        }

        public List<Models.Appointment> Appointments { get; set; } = new List<Models.Appointment>();

        public List<DataMonthly> DataMonthlyStatistic { get; set; } = new List<DataMonthly>();

        public class DataMonthly
        {
            public int Month { get; set; }
            public int MonthCompleted { get; set; }
            public int MonthCancelled { get; set; }
            public int MonthAbsent { get; set; }
            public int MonthWaiting { get; set; }
        }

        public IActionResult OnGet(string? txtDate)
        {
            try
            {
                Appointments = _appointmentRepo.GetAll().Result;

                if (!string.IsNullOrEmpty(txtDate))
                {
                    var currentDate = DateTime.Parse(txtDate);
                    Appointments = Appointments.Where(x => x.TimeTable.Date.Date == currentDate.Date).ToList();
                }
                else
                {
                    Appointments = Appointments.Where(x => x.TimeTable.Date.Date == DateTime.Now.Date).ToList();
                }

                #region monthly appointment
                var totalAppointment = _appointmentRepo.GetAll().Result;

                var monthAppointment = totalAppointment.Count <= 0 ? null : totalAppointment.Where(x => x.TimeTable.Date == DateTime.Now.Date);

                var newData = new DataMonthly
                {
                    MonthCompleted = monthAppointment == null ? 0 : monthAppointment.Where(x => x.Status == Constants.StatusConstant.Status.Đã_hoàn_thành).ToList().Count(),
                    MonthAbsent = monthAppointment == null ? 0 : monthAppointment.Where(x => x.Status == Constants.StatusConstant.Status.Vắng).ToList().Count(),
                    MonthCancelled = monthAppointment == null ? 0 : monthAppointment.Where(x => x.Status == Constants.StatusConstant.Status.Đã_hủy).ToList().Count(),
                    MonthWaiting = monthAppointment == null ? 0 : monthAppointment.Where(x => x.Status == Constants.StatusConstant.Status.Đang_Chờ).ToList().Count(),
                };

                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " - " + ex.StackTrace);
                return Redirect("/Error");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostCancel()
        {
            try
            {
                var txtId = Request.Form["txtAId"];
                if (string.IsNullOrEmpty(txtId)) return Redirect("/Error");

                int id = int.Parse(txtId);

                var find = await _appointmentRepo.GetById(id);
                if (find == null) return Page();
                find.Status = Constants.StatusConstant.Status.Đã_hủy;
                await _appointmentRepo.CancelAppointment(find);

                ViewData["msg"] = "Cập nhật thành công !";

                OnGet(find.TimeTable.Date.ToString());
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
