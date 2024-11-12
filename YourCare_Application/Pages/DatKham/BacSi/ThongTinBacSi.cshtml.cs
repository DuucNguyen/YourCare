using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.DotNet.MSIdentity.Shared;
using NetTopologySuite.Index.HPRtree;
using YourCare_Application.Models;
using YourCare_Application.Repository.Interfaces;

namespace YourCare_Application.Pages.DatKham.BacSi
{
    [Authorize(Policy = "AdminOrPatient")]
    public class ThongTinBacSiModel : PageModel
    {
        private readonly IUserRepository _userRepo;
        private readonly ISpecializationRepository _speRepo;
        private readonly IDoctorSpecializationRepository _docSpeRepo;
        private readonly ITimetableRepository _timetableRepo;

        public ThongTinBacSiModel(ILogger<IndexModel> logger,
           IUserRepository userRepository,
           ISpecializationRepository speRepo,
           IDoctorSpecializationRepository docSpeRepo,
           ITimetableRepository timetableRepo)
        {
            _userRepo = userRepository;
            _speRepo = speRepo;
            _docSpeRepo = docSpeRepo;
            _timetableRepo = timetableRepo;
        }
        public Models.Doctor Doc { get; set; }
        public List<Models.Timetable> Timetables { get; set; }
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

        public async Task<IActionResult> OnGet(string doctorID)
        {
            if (string.IsNullOrEmpty(doctorID)) return Redirect("/Error");

            await FindDoc(doctorID);

            var chosenDate = DateTime.Now.Date;
            ViewData["chosenDate"] = chosenDate;

            return Page();
        }



        public async Task<JsonResult> OnGetAJAX(string doctorID, int timeTableID)
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

            ViewData["chosenDate"] = chosenDate;

            return new JsonResult(new
            {
                timetables = timetables,
                chosenTimetb = timeTableID,
                chosenDate = chosenDate
            });
        }

    }
}
