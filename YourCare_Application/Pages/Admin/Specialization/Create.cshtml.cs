using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using YourCare_Application.Models;
using YourCare_Application.Repository.Interfaces;

namespace YourCare_Application.Pages.Admin.Specialization
{
    [Authorize(Policy = "AdminOnly")]
    public class CreateModel : PageModel
    {
        private readonly ISpecializationRepository _specializationRepo;

        public CreateModel(
            ISpecializationRepository specializationRepo
            )
        {
            _specializationRepo = specializationRepo;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "Specialization name")]
            public string Name { get; set; }

            [Display(Name = "Image")]
            public IFormFile ImageFile { get; set; }

            public string? ImageString { get; set; }
        }

        public string? StatusMessage { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                StatusMessage = "Error: Unexpected error occurred. Try again !";
                return Page();
            }

            var specialization = new Models.Specialization
            {
                Name = Input.Name
            };

            if (Input.ImageFile != null && Input.ImageFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    Input.ImageFile.CopyTo(ms);
                    var imagebytes = ms.ToArray();
                    specialization.Image = imagebytes;
                }
            }

            var result = await _specializationRepo.Add(specialization);

            if (result) StatusMessage = "Created successfully !";

            return RedirectToPage("./Index");
        }
    }
}
