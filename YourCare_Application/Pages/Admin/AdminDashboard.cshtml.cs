using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using System.Runtime.ConstrainedExecution;
using YourCare_Application.Models;
using YourCare_Application.Repository.Interfaces;

namespace YourCare_Application.Pages.Admin
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminDashboardModel : PageModel
    {
        private readonly IUserRepository _userRepo;
        private readonly ISpecializationRepository _speRepo;
        private readonly IDoctorSpecializationRepository _docSpeRepo;
        private readonly ITimetableRepository _timetableRepo;
        private readonly IPatientProfileRepository _patitentProfileRepo;
        private readonly IAppointmentRepository _appointmentRepo;
        public AdminDashboardModel(
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

        public List<Models.Doctor> Doctors { get; set; } = new List<Models.Doctor>();

        public List<DataLeaderBoard> LeaderBoard { get; set; } = new List<DataLeaderBoard>(); //monthly

        public List<DataMonthly> DataMonthlyStatistic { get; set; } = new List<DataMonthly>();

        public class DataLeaderBoard
        {
            public Models.Doctor Doc { get; set; } = new Models.Doctor();
            public int CompletedAppointments { get; set; }
            public int TotalAppointments { get; set; }

            public double Rating { get; set; }
        }

        public class DataMonthly
        {
            public int Month { get; set; }
            public int MonthCompleted { get; set; }
            public int MonthCancelled { get; set; }
            public int MonthAbsent { get; set; }
            public int MonthWaiting { get; set; }
        }

        public void OnGet()
        {
            try
            {
                int monthCount = 0;
                Doctors = _userRepo.GetAllDoctor();

                #region doctorLeaderBoard
                foreach (var doctor in Doctors)
                {
                    doctor.AvatarString = doctor.Avatar != null ? $"data:image/png;base64,{Convert.ToBase64String(doctor.Avatar)}" : "";

                    var generalList = _appointmentRepo.GetAllByDoctorId(doctor.Id);

                    var monthList = generalList != null ? generalList.Result.Where(x => x.TimeTable.Date.Month == DateTime.Now.Date.Month).ToList() : null;

                    monthCount += monthList != null ? monthList.Where(x=>x.Status!=Constants.StatusConstant.Status.Đã_hủy).ToList().Count : 0;


                    LeaderBoard.Add(new DataLeaderBoard
                    {
                        Doc = doctor,
                        CompletedAppointments = monthList != null ? monthList.Where(x => x.Status == Constants.StatusConstant.Status.Đã_hoàn_thành).ToList().Count : 0,
                        TotalAppointments = monthList != null ? monthList.Count : 0
                    });
                }

                LeaderBoard = LeaderBoard
                    .OrderByDescending(x => x.TotalAppointments).ToList();

                #endregion

                #region monthly appointment
                var totalAppointment = _appointmentRepo.GetAll().Result;
                var month = 1;
                while (month <= 12)
                {
                    var monthAppointment = totalAppointment.Count <= 0 ? null : totalAppointment.Where(x => x.TimeTable.Date.Month == month);

                    DataMonthlyStatistic.Add(new DataMonthly
                    {
                        Month = month,
                        MonthCompleted = monthAppointment == null ? 0 : monthAppointment.Where(x => x.Status == Constants.StatusConstant.Status.Đã_hoàn_thành).ToList().Count(),
                        MonthAbsent = monthAppointment == null ? 0 : monthAppointment.Where(x => x.Status == Constants.StatusConstant.Status.Vắng).ToList().Count(),
                        MonthCancelled = monthAppointment == null ? 0 : monthAppointment.Where(x => x.Status == Constants.StatusConstant.Status.Đã_hủy).ToList().Count(),
                        MonthWaiting = monthAppointment == null ? 0 : monthAppointment.Where(x => x.Status == Constants.StatusConstant.Status.Đang_Chờ).ToList().Count(),
                    });

                    month++;
                }
                #endregion

                #region specialization leaderBoard


                #endregion

                ViewData["monthCount"] = monthCount;
                ViewData["chosenMonth"] = DateTime.Now.Month;

            }
            catch (Exception ex)
            {
                Console.WriteLine("AdminDashboard: " + ex.Message + " - " + ex.StackTrace);
            }
        }

        public IActionResult OnPost()
        {
            try
            {
                var txtMonth = Request.Form["txtMonth"];

                int monthCount = 0;
                Doctors = _userRepo.GetAllDoctor();
                var totalAppointment = _appointmentRepo.GetAll().Result;


                #region doctorLeaderBoard
                foreach (var doctor in Doctors)
                {
                    doctor.AvatarString = doctor.Avatar != null ? $"data:image/png;base64,{Convert.ToBase64String(doctor.Avatar)}" : "";

                    var generalList = _appointmentRepo.GetAllByDoctorId(doctor.Id);

                    var monthList = generalList != null ? generalList.Result.Where(x => x.TimeTable.Date.Month == DateTime.Now.Date.Month).ToList() : null;

                    if (!string.IsNullOrEmpty(txtMonth))
                    {
                        int month = Int32.Parse(txtMonth);
                        monthList = generalList != null ? generalList.Result.Where(x => x.TimeTable.Date.Month == month).ToList() : null;
                    }

                    monthCount += monthList != null ? monthList.Count : 0;

                    double average = 0;
                    var totalCompleted = monthList != null ? monthList.Where(x => x.Status == Constants.StatusConstant.Status.Đã_hoàn_thành).ToList().Count : 0;
                    if (totalCompleted > 0)
                    {
                        var totalRating = monthList.Where(x => x.PatientRating > 0).Sum(x => x.PatientRating);
                        var countRating = monthList.Where(x => x.PatientRating > 0).ToList().Count;
                        average = countRating > 0 ? (double)totalRating / countRating : 0;
                    }

                    LeaderBoard.Add(new DataLeaderBoard
                    {
                        Doc = doctor,
                        CompletedAppointments = monthList != null ? monthList.Where(x => x.Status == Constants.StatusConstant.Status.Đã_hoàn_thành).ToList().Count : 0,
                        TotalAppointments = monthList != null ? monthList.Count : 0,
                        Rating = average
                    });
                }

                LeaderBoard = LeaderBoard.OrderByDescending(x => x.TotalAppointments).Take(10).ToList();

                #endregion

                #region monthly appointment
                var currentMonth = 1;
                while (currentMonth <= 12)
                {
                    var monthAppointment = totalAppointment.Count <= 0 ? null : totalAppointment.Where(x => x.TimeTable.Date.Month == currentMonth);

                    DataMonthlyStatistic.Add(new DataMonthly
                    {
                        Month = currentMonth,
                        MonthCompleted = monthAppointment == null ? 0 : monthAppointment.Where(x => x.Status == Constants.StatusConstant.Status.Đã_hoàn_thành).ToList().Count(),
                        MonthAbsent = monthAppointment == null ? 0 : monthAppointment.Where(x => x.Status == Constants.StatusConstant.Status.Vắng).ToList().Count(),
                        MonthCancelled = monthAppointment == null ? 0 : monthAppointment.Where(x => x.Status == Constants.StatusConstant.Status.Đã_hủy).ToList().Count(),
                        MonthWaiting = monthAppointment == null ? 0 : monthAppointment.Where(x => x.Status == Constants.StatusConstant.Status.Đang_Chờ).ToList().Count(),
                    });

                    currentMonth++;
                }
                #endregion

                #region rating leaderBoard

                var appointmentInMonth = totalAppointment.Where(x => x.TimeTable.Date.Month == DateTime.Now.Month).ToList();
                if (!string.IsNullOrEmpty(txtMonth))
                {
                    int month = Int32.Parse(txtMonth);
                    ViewData["chosenMonth"] = month;

                    appointmentInMonth = totalAppointment.Where(x => x.TimeTable.Date.Month == month).ToList();
                }

                foreach (var appointment in appointmentInMonth)
                {

                }

                #endregion

                ViewData["monthCount"] = monthCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine("AdminDashboard: " + ex.Message + " - " + ex.StackTrace);
            }
            return Page();
        }
    }
}
