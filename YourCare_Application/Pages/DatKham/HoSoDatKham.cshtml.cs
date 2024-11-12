using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using YourCare_Application.Models;
using YourCare_Application.Repository.Interfaces;
using YourCare_Application.ServerHubs;

namespace YourCare_Application.Pages.DatKham
{

   


    [Authorize(Policy = "AdminOrPatient")]
    public class HoSoDatKhamModel : PageModel
    {
        private readonly IHubContext<ServerHub> _hubContext;

        private readonly IUserRepository _userRepo;
        private readonly ISpecializationRepository _speRepo;
        private readonly IDoctorSpecializationRepository _docSpeRepo;
        private readonly ITimetableRepository _timetableRepo;
        private readonly IPatientProfileRepository _patitentProfileRepo;
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        public HoSoDatKhamModel(ILogger<IndexModel> logger,
           IUserRepository userRepository,
           ISpecializationRepository speRepo,
           IDoctorSpecializationRepository docSpeRepo,
           ITimetableRepository timetableRepo,
           UserManager<ApplicationUser> userManager,
           IPatientProfileRepository patitentProfileRepo,
           IAppointmentRepository appointmentRepo,
            IHubContext<ServerHub> hubContext
           )
        {
            _userRepo = userRepository;
            _speRepo = speRepo;
            _docSpeRepo = docSpeRepo;
            _timetableRepo = timetableRepo;
            _userManager = userManager;
            _patitentProfileRepo = patitentProfileRepo;
            _appointmentRepo = appointmentRepo;
            _hubContext = hubContext;
        }

        public Models.Doctor Doc { get; set; } = new Models.Doctor();

        public Timetable TimeTable { get; set; } = new Models.Timetable();

        public List<PatientProfile> PatientProfiles { get; set; } = new List<PatientProfile>();
        public PatientProfile PatientProfile { get; set; } = new PatientProfile();

        [BindProperty]
        public Appointment Appointment { get; set; } = new Appointment();

        [BindProperty]
        public InputModel Profile { get; set; } = new InputModel();

        public class InputModel
        {
            public string ApplicationUserID { get; set; }

            [MaxLength(256, ErrorMessage = "Tên phải nhỏ hơn 250 kí tự")]
            [Required(ErrorMessage = "Vui lòng điền họ tên!")]
            public string Name { get; set; }
            [Required(ErrorMessage = "Vui lòng chọn giới tính!")]
            public bool Gender { get; set; }
            [Required(ErrorMessage = "Vui lòng chọn ngày sinh!")]
            public DateTime Dob { get; set; }

            [MaxLength(12, ErrorMessage = "Sai cú pháp !")]
            [Required(ErrorMessage = "Số điện thoại không được để trống")]
            public string PhoneNumber { get; set; }


            [MaxLength(256, ErrorMessage = "Địa chỉ phải nhỏ hơn 250 kí tự !")]
            public string? Address { get; set; }

            [MaxLength(12, ErrorMessage = "Sai cú pháp !")]
            public string? IdentityNumber { get; set; }

            public string? InsuranceNumber { get; set; }

            [EmailAddress(ErrorMessage = "Không đúng định dạng email !")]
            public string? Email { get; set; }


            [MaxLength(100)]
            public string? Career { get; set; }

            [MaxLength(100)]
            public string? Ethnic { get; set; }
        }

        private async Task FindDoc(string doctorID)
        {
            if (!string.IsNullOrEmpty(doctorID)) Doc = await _userRepo.GetDoctorById(doctorID);

            if (Doc != null)
            {
                Doc.AvatarString = Doc.Avatar != null ? $"data:image/png;base64,{Convert.ToBase64String(Doc.Avatar)}" : "";
                Doc.Specializations = _docSpeRepo.GetAllSpeByDoctorId(Doc.Id);
                Doc.Timetables = await _timetableRepo.GetInRange(Doc.Id, DateTime.Now.Date, DateTime.Now.AddDays(10).Date);
            }
        }

        public async Task<IActionResult> OnGet(string? doctorID, int? timetableID)
        {
            try
            {
                PatientProfiles = new List<PatientProfile>();
                var user = await _userManager.GetUserAsync(User);

                if (string.IsNullOrEmpty(doctorID)) return Redirect("/Error");
                await FindDoc(doctorID);
                if (Doc == null) return Redirect("/Error");

                if (timetableID == null) return Redirect("/Error");
                TimeTable = await _timetableRepo.GetById(timetableID ?? default(Int32));
                if (TimeTable == null) return Redirect("/Error");

                PatientProfiles = await _patitentProfileRepo.GetAllByUserId(user.Id);
                var chosenDate = TimeTable != null ? TimeTable.Date : DateTime.Now.Date;

                ViewData["chosenDate"] = chosenDate;

                if (PatientProfiles.Any())
                {
                    PatientProfile = PatientProfiles.FirstOrDefault();
                    Appointment.PatientProfileID = PatientProfile.Id;
                    Appointment.DoctorID = Doc.Id;
                    Appointment.TimetableID = TimeTable.Id;

                    return Page();
                }
                else
                {
                    var newID = Guid.NewGuid().ToString();
                    //add new profile
                    var newProfile = new PatientProfile
                    {
                        Id = newID,
                        ApplicationUserID = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Gender = user.Gender,
                        Dob = user.Dob,
                        Address = user.Address,
                    };

                    await _patitentProfileRepo.Add(newProfile);

                    PatientProfiles = await _patitentProfileRepo.GetAllByUserId(user.Id);
                    PatientProfile = PatientProfiles.FirstOrDefault();
                    Appointment.PatientProfileID = PatientProfile.Id;
                    Appointment.DoctorID = Doc.Id;
                    Appointment.TimetableID = TimeTable.Id;

                    return Page();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " -> " + ex.StackTrace);
                return Redirect("/Error");
            }
        }

        public async Task<JsonResult> OnGetAJAX(string doctorID, int timeTableID, int fatherId)
        {
            await FindDoc(doctorID);

            var findDate = await _timetableRepo.GetById(timeTableID);

            var chosenDate = findDate != null ? findDate.Date : DateTime.Now.Date;

            var timetables = Doc.Timetables
                .Where(x => x.Date.Date == chosenDate.Date
                && x.IsAvailable
                )
                .OrderBy(x => x.StartTime)
                .Select(x => new
                {
                    Id = x.Id,
                    Date = x.Date.ToString("d"),
                    StartTime = x.StartTime.ToString(@"hh\:mm"),
                    EndTime = x.EndTime.ToString(@"hh\:mm"),
                    IsAvailable = x.IsAvailable,
                }).ToList();

            TimeTable = await _timetableRepo.GetById(timeTableID);

            ViewData["chosenDate"] = chosenDate;

            return new JsonResult(new
            {
                timetables = timetables,
                chosenTimetb = timeTableID,
                chosenDate = chosenDate,
                fatherId = fatherId
            });
        }

        public async Task<IActionResult> OnPostAddProfile() //add new prifile
        {
            try
            {
                var docId = Request.Form["txtDocId"];
                var timetableId = Int32.Parse(Request.Form["txtTimetableId"]);

                var user = await _userManager.GetUserAsync(User);

                //add new profile
                var newProfile = new PatientProfile
                {
                    Id = Guid.NewGuid().ToString(),
                    ApplicationUserID = user.Id,
                    Name = Profile.Name,
                    Email = Profile.Email,
                    PhoneNumber = Profile.PhoneNumber,
                    Gender = Profile.Gender,
                    Dob = Profile.Dob,
                    Address = Profile.Address,
                    InsuranceNumber = Profile.InsuranceNumber,
                    IdentityNumber = Profile.IdentityNumber,
                    Ethnic = Profile.Ethnic,
                    Career = Profile.Career,
                };

                await _patitentProfileRepo.Add(newProfile);
                ViewData["CreateProfileMsg"] = "Tạo hồ sơ thành công !";
                return await OnGet(docId, timetableId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " -> " + ex.StackTrace);
                return Redirect("/Error");
            }

        }

        public async Task<IActionResult> OnPostUpdateProfile() //add new prifile
        {
            try
            {
                var docId = Request.Form["txtDocId"];
                var timetableId = Int32.Parse(Request.Form["txtTimetableId"]);

                var user = await _userManager.GetUserAsync(User);
                var id = Request.Form["txtProfileId"];
                var find = await _patitentProfileRepo.GetById(id);

                if (find == null) return Redirect("/Error");

                find.ApplicationUserID = user.Id;
                find.Name = Profile.Name;
                find.Email = Profile.Email;
                find.PhoneNumber = Profile.PhoneNumber;
                find.Gender = Profile.Gender;
                find.Dob = Profile.Dob;
                find.Address = Profile.Address;
                find.InsuranceNumber = Profile.InsuranceNumber;
                find.IdentityNumber = Profile.IdentityNumber;
                find.Ethnic = Profile.Ethnic;
                find.Career = Profile.Career;

                await _patitentProfileRepo.Update(find);
                ViewData["CreateProfileMsg"] = "Cập nhật hồ sơ thành công !";
                return await OnGet(docId, timetableId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " -> " + ex.StackTrace);
                return Redirect("/Error");
            }

        }

        public async Task<IActionResult> OnPostAppointment() //make appointment
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                Appointment.TotalPrice = 0;
                Appointment.CreatedBy = user.Id;
                Appointment.UpdatedOn = DateTime.Now.Date;
                Appointment.Status = Constants.StatusConstant.Status.Đang_Chờ;

                await _appointmentRepo.Add(Appointment);
                await _timetableRepo.Deactivate(Appointment.TimetableID);

                await _hubContext.Clients.All.SendAsync("LoadAll");

                //redirect success page
                return Redirect("/Success");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " -> " + ex.StackTrace);
                return Redirect("/Error");
            }

        }
    }
}
