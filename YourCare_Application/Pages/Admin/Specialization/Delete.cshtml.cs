using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using YourCare_Application.Models;
using YourCare_Application.Repository.Interfaces;

namespace YourCare_Application.Pages.Admin.Specialization
{
    [Authorize(Policy = "AdminOnly")]
    public class DeleteModel : PageModel
    {
        private readonly ISpecializationRepository _specializationRepo;

        public DeleteModel(
            ISpecializationRepository specializationRepo
            )
        {
            _specializationRepo = specializationRepo;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public int Id { get; set; }

            [Display(Name = "Specialization name")]
            public string Name { get; set; }

            [Display(Name = "Image")]
            public IFormFile? ImageFile { get; set; }

            public string? ImageString { get; set; }
        }

        public string? StatusMessage { get; set; }

        public async Task<IActionResult> OnGet(string? id)
        {
            try
            {
                if (id == null) return RedirectToPage("/Error");

                var find = await _specializationRepo.GetById(int.Parse(id));

                if (find == null) return RedirectToPage("/Error");

                Input = new InputModel();

                Input.Id = find.Id;
                Input.Name = find.Name;
                Input.ImageString = find.Image != null ? $"data:image/png;base64,{Convert.ToBase64String(find.Image)}" : "";

                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error");
            }
        }


        public async Task<IActionResult> OnPostAsync(int id)
        {
            var specialization = new Models.Specialization
            {
                Id = id,
            };

            var result = await _specializationRepo.Delete(specialization);
            if (result) return RedirectToPage("./Index");
            else return Page();
        }
    }
}
