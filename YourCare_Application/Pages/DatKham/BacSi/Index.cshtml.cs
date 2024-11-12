using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ReflectionIT.Mvc.Paging;
using System.Runtime.Intrinsics.X86;
using YourCare_Application.Models;
using YourCare_Application.Repository.Interfaces;

namespace YourCare_Application.Pages.DatKham.BacSi
{
    [Authorize(Policy = "AdminOrPatient")]
    public class IndexModel : PageModel
    {
        private readonly IUserRepository _userRepository;
        private readonly ISpecializationRepository _speRepo;
        private readonly IDoctorSpecializationRepository _docSpeRepo;

        public IndexModel(ILogger<IndexModel> logger,
            IUserRepository userRepository,
            ISpecializationRepository speRepo,
            IDoctorSpecializationRepository docSpeRepo)
        {
            _userRepository = userRepository;
            _speRepo = speRepo;
            _docSpeRepo = docSpeRepo;
        }

        public IPagingList<Models.Doctor> Doctors { get; set; }
        public List<Models.Specialization> Specializations { get; set; }
        public int TotalRecordCount { get; set; }

        public void OnGet(int pageIndex = 1, int pageSize = 10)
        {
            string txtSearch = Request.Query["txtSearch"];
            string spe = Request.Query["specialization"];

            var qry = _userRepository.GetAllDoctor()
                .Where(x => x.IsActive == true)
                .OrderBy(x => x.Name);

            Specializations = _speRepo.GetAll()
                .Select(x => new Models.Specialization
                {
                    Id = x.Id,
                    Name = x.Name,
                    Image = x.Image,
                    ImageString = x.Image != null ? $"data:image/png;base64,{Convert.ToBase64String(x.Image)}" : "",
                })
                .ToList();

            foreach (var doctor in qry)
            {
                doctor.AvatarString = doctor.Avatar != null
                    ? $"data:image/png;base64,{Convert.ToBase64String(doctor.Avatar)}"
                    : "";
                doctor.Specializations = _docSpeRepo.GetAllSpeByDoctorId(doctor.Id);
            }

            if (!string.IsNullOrEmpty(txtSearch))
            {
                qry = qry.Where(x => x.Name.Contains(txtSearch)).OrderBy(x => x.Name);
                ViewData["txtSearch"] = txtSearch;
            }

            if (!string.IsNullOrEmpty(spe))
            {
                if (!spe.Equals("all"))
                {
                    var speId = Int32.Parse(spe);
                    qry = qry.Where(x => x.Specializations.Select(x => x.Id).Contains(speId)).OrderBy(x => x.Id);
                }
            }

            TotalRecordCount = qry.Count();

            Doctors = (IPagingList<Models.Doctor>)PagingList.Create(qry, pageSize, pageIndex);

            Doctors.RouteValue = new RouteValueDictionary
            {
                { "txtSearch" , txtSearch ?? string.Empty },
                { "specialization" , spe ?? string.Empty },
                { "pageSize", pageSize }
            };
        }
    }
}
