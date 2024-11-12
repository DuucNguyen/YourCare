using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using YourCare_Application.Models;
using YourCare_Application.Repository.Interfaces;

namespace YourCare_Application.Pages.Admin.Specialization
{
    [Authorize(Policy = "AdminOnly")]
    public class IndexModel : PageModel
    {
        private readonly ISpecializationRepository _specializationRepo;
        public IndexModel(
            ISpecializationRepository specializationRepo
            )
        {
            _specializationRepo = specializationRepo;
        }

        public IPagingList<Models.Specialization> Specializations { get; set; } = default!;

        public async Task OnGet(int pageIndex = 1, int pageSize = 10)
        {
            var qry = _specializationRepo.GetAll().OrderBy(x => x.Id);

            foreach (var item in qry)
            {
                item.ImageString = item.Image != null ? $"data:image/png;base64,{Convert.ToBase64String(item.Image)}" : "";
            }

            Specializations = PagingList.Create(qry, pageSize, pageIndex);

            ViewData["specialization"] = PagingList.Create(qry, pageSize, pageIndex);
        }
    }
}
