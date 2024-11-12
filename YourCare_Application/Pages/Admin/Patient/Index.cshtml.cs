using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using YourCare_Application.Models;
using YourCare_Application.Repository.Interfaces;
using YourCare_Application.ServerHubs;

namespace YourCare_Application.Pages.Admin.Patient
{
    public class IndexModel : PageModel
    {
        private readonly IHubContext<ServerHub> _hubContext;

        private readonly IUserRepository _userRepo;
        private readonly ISpecializationRepository _speRepo;
        private readonly IDoctorSpecializationRepository _docSpeRepo;
        private readonly ITimetableRepository _timetableRepo;
        private readonly IPatientProfileRepository _patitentProfileRepo;
        private readonly IAppointmentRepository _appointmentRepo;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
           IUserRepository userRepository,
           ISpecializationRepository speRepo,
           IDoctorSpecializationRepository docSpeRepo,
           ITimetableRepository timetableRepo,
           IPatientProfileRepository patitentProfileRepo,
           IAppointmentRepository appointmentRepo,
           UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           IHubContext<ServerHub> hubContext
           )
        {
            _userRepo = userRepository;
            _speRepo = speRepo;
            _docSpeRepo = docSpeRepo;
            _timetableRepo = timetableRepo;
            _patitentProfileRepo = patitentProfileRepo;
            _appointmentRepo = appointmentRepo;
            _userManager = userManager;
            _signInManager = signInManager;
            _hubContext = hubContext;
        }
        public List<Models.Appointment> Appointments { get; set; } = new List<Models.Appointment>();
        public List<ApplicationUser> Patients { get; set; } = new List<ApplicationUser>();

        public List<DataPatient> DataPatients { get; set; } = new List<DataPatient>();

        [BindProperty]
        public ApplicationUser User { get; set; } = new ApplicationUser();

        public class DataPatient
        {
            public ApplicationUser User { get; set; }
            public int TotalAppointments { get; set; }
            public int CompletedAppointments { get; set; }
            public int AbsentAppointments { get; set; }
            public int WaitingAppointments { get; set; }
            public int CancelledAppointments { get; set; }
            public int TotalProfiles { get; set; }
        }

        public IActionResult OnGet()
        {
            try
            {
                Appointments = _appointmentRepo.GetAll().Result;
                Patients = _userRepo.GetAllUser();

                foreach (var user in Patients)
                {
                    //if (user.LockoutEnd != null) continue;
                    var userAppointments = Appointments.Where(x => x.CreatedBy == user.Id).ToList();

                    DataPatients.Add(new DataPatient
                    {
                        User = user,
                        TotalAppointments = userAppointments != null ? userAppointments.Count : 0,
                        CompletedAppointments = userAppointments != null ? userAppointments.Where(x => x.Status == Constants.StatusConstant.Status.Đã_hoàn_thành).ToList().Count : 0,
                        AbsentAppointments = userAppointments != null ? userAppointments.Where(x => x.Status == Constants.StatusConstant.Status.Vắng).ToList().Count : 0,
                        WaitingAppointments = userAppointments != null ? userAppointments.Where(x => x.Status == Constants.StatusConstant.Status.Đang_Chờ).ToList().Count : 0,
                        CancelledAppointments = userAppointments != null ? userAppointments.Where(x => x.Status == Constants.StatusConstant.Status.Đã_hủy).ToList().Count : 0,
                        TotalProfiles = _patitentProfileRepo.GetAllByUserId(user.Id).Result.Count
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Page();
        }

        public async Task<IActionResult> OnPostBanUser()
        {
            try
            {
                var user = await _userManager.FindByIdAsync(User.Id);
                if (user == null)
                {
                    return NotFound(); // Handle user not found
                }

                // Update the security stamp to invalidate the user's session
                await _userManager.UpdateSecurityStampAsync(user);

                await ForceLogoutAndBanAsync(user.Id); // Updates security stamp
                await _hubContext.Clients.User(user.Id).SendAsync("ForceLogout");

                OnGet();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Page();
        }


        public async Task ForceLogoutAndBanAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.SecurityStamp = Guid.NewGuid().ToString();
                user.LockoutEnd = DateTimeOffset.MaxValue;

                await _userManager.UpdateAsync(user);
            }
        }

        public async Task<IActionResult> OnPostUnBanUser()
        {
            try
            {
                var user = await _userManager.FindByIdAsync(User.Id);
                if (user == null)
                {
                    return NotFound(); // Handle user not found
                }

                user.LockoutEnd = null;
                await _userManager.UpdateAsync(user);
                await _hubContext.Clients.User(user.Id).SendAsync("UnBan");


                OnGet();
            }
            catch (Exception ex)
            {
                return Redirect("/Error");
            }
            return Page();
        }
    }
}
