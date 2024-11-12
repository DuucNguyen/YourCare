using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YourCare_Application.Constants;
using YourCare_Application.Models;
using YourCare_Application.Repository.Interfaces;

namespace YourCare_Application.Pages.Admin.Doctor
{
    [Authorize(Policy = "AdminOnly")]
    public class ManageTimetableModel : PageModel
    {
        private readonly ITimetableRepository _timetableRepo;
        private readonly IUserRepository _userRepo;
        private readonly ISpecializationRepository _speRepo;
        private readonly IDoctorSpecializationRepository _doctorSpeRepo;


        public ManageTimetableModel(
            ITimetableRepository timetableRepo,
            IUserRepository userRepo,
            ISpecializationRepository speRepo
            , IDoctorSpecializationRepository doctorSpeRepo
        )
        {
            _timetableRepo = timetableRepo;
            _userRepo = userRepo;
            _speRepo = speRepo;
            _doctorSpeRepo = doctorSpeRepo;
        }

        public Models.Doctor Doc { get; set; }
        public string StatusMessage { get; set; }
        public async Task<IActionResult> OnGet(string id)
        {
            try
            {
                ViewData["morningTimetable"] = TimeTableConstant.MorningTimeTable;
                ViewData["afternoonTimetable"] = TimeTableConstant.AfternoonTimeTable;

                if (!string.IsNullOrEmpty(id)) Doc = await _userRepo.GetDoctorById(id);

                if (Doc != null)
                {
                    Doc.AvatarString = Doc.Avatar != null ? $"data:image/png;base64,{Convert.ToBase64String(Doc.Avatar)}" : "";
                    Doc.Specializations = _doctorSpeRepo.GetAllSpeByDoctorId(Doc.Id);
                }

                return Page();
            }
            catch (Exception ex)
            {
                return Redirect("/Error");
            }

        }

        public async Task<IActionResult> OnPost()
        {
            try
            {
                string docID = Request.Query["id"];
                if (!string.IsNullOrEmpty(docID)) Doc = await _userRepo.GetDoctorById(docID);

                if (Doc != null)
                {
                    Doc.AvatarString = Doc.Avatar != null ? $"data:image/png;base64,{Convert.ToBase64String(Doc.Avatar)}" : "";
                    Doc.Specializations = _doctorSpeRepo.GetAllSpeByDoctorId(Doc.Id);
                }

                List<string> timeSlots = Request.Form["timeSlot"].ToList();
                DateTime startDate = DateTime.Parse(Request.Form["startDate"]);
                DateTime endDate = DateTime.Parse(Request.Form["endDate"]);

                List<Timetable> timetables = new List<Timetable>();

                var currentDate = startDate;

                while (currentDate <= endDate)
                {
                    if (currentDate.DayOfWeek != DayOfWeek.Sunday || currentDate.DayOfWeek != DayOfWeek.Saturday)
                    {
                        foreach (var t in timeSlots)
                        {
                            TimeOnly startTime = TimeOnly.Parse(t.Split("-")[0]);
                            TimeOnly endTime = TimeOnly.Parse(t.Split("-")[1]);

                            Timetable timetable = new Timetable
                            {
                                DoctorID = Doc.Id,
                                Date = currentDate,
                                StartTime = startTime.ToTimeSpan(),
                                EndTime = endTime.ToTimeSpan(),
                                IsAvailable = true
                            };
                            timetables.Add(timetable);
                        }
                    }
                    currentDate = currentDate.AddDays(1);
                }
                
                await _timetableRepo.AddRange(timetables);

                StatusMessage = "Added Successfully !";

                await OnGet(docID);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " -> " + ex.StackTrace);
                return Redirect("/Error");
            }

            return Page();
        }
    }
}
