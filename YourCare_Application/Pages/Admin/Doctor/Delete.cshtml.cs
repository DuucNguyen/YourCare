using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using YourCare_Application.Migrations;
using YourCare_Application.Models;
using YourCare_Application.Repository.Interfaces;

namespace YourCare_Application.Pages.Admin.Doctor
{
    [Authorize(Policy = "AdminOnly")]
    public class DeleteModel : PageModel
    {
        private readonly IUserRepository _userRepository;
        public DeleteModel(
            IUserRepository userRepo
            )
        {
            _userRepository = userRepo;
        }

        [BindProperty]
        public Models.Doctor Doctor { get; set; } = default!;

        [BindProperty]
        public string? StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var doc = _userRepository.GetById(id).Result as Models.Doctor;
            if (doc == null) return RedirectToPage("./Index");

            if (doc.Avatar != null) ViewData["AvatarString"] = $"data:image/png;base64,{Convert.ToBase64String(doc.Avatar)}";

            Doctor = doc;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? id)
        {
            StatusMessage = "Error : Undetermined error !";
            var doc = _userRepository.GetById(id).Result as Models.Doctor;

            if (doc == null) return RedirectToPage("/Error");
            var result = await _userRepository.Deactivate(doc);

            return RedirectToPage("./Index");
        }
    }
}
