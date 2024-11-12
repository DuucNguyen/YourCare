using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;
using YourCare_Application.Models;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using YourCare_Application.Repository.Interfaces;

namespace YourCare_Application.Pages.Admin.Doctor
{
    [Authorize(Policy = "AdminOnly")]
    public class IndexModel : PageModel
    {
        private readonly IUserRepository _userRepository;
        private readonly IDoctorSpecializationRepository _doctorSpeRepo;
        private readonly ISpecializationRepository _speRepo;
        public IndexModel(IUserRepository userRepository,
            IDoctorSpecializationRepository doctorSpeRepo,
            ISpecializationRepository speRepo
            )
        {
            _userRepository = userRepository;
            _doctorSpeRepo = doctorSpeRepo;
            _speRepo = speRepo;
        }
        public IPagingList<Models.Doctor> Doctors { get; set; } = default!;

        public List<Models.Specialization> Specializations { get; set; }
        public int TotalRecordCount { get; set; }

        public async Task OnGet(int pageIndex = 1, int pageSize = 10)
        {
            string spe = Request.Query["specialization"];

            Specializations = _speRepo.GetAll().OrderBy(x => x.Name).ToList();

            var qry = _userRepository.GetAllDoctor()
                .OrderBy(x => x.Id);

            foreach (var item in qry)
            {
                item.AvatarString = item.Avatar != null ? $"data:image/png;base64,{Convert.ToBase64String(item.Avatar)}" : "";
                item.Specializations = _doctorSpeRepo.GetAllSpeByDoctorId(item.Id);
            }

            if (!string.IsNullOrEmpty(spe))
            {
                if (!spe.Equals("all"))
                {
                    var speId = int.Parse(spe);
                    qry = qry.Where(x => x.Specializations.Select(x => x.Id).Contains(speId)).OrderBy(x => x.Id);
                    ViewData["spe"] = speId;
                }
            }
            string sort = "Name";

            qry = qry.OrderByDescending(x => x.IsActive);

            TotalRecordCount = qry.Count();

            Doctors = PagingList.Create(qry, pageSize, pageIndex);

            Doctors.RouteValue = new RouteValueDictionary
            {
                { "specialization" , spe ?? string.Empty },
                { "pageSize", pageSize }
            };
        }

        public void OnPost() { }
    }
}
