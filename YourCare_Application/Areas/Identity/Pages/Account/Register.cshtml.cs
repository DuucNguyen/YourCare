using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using YourCare_Application.Models;
using static System.Net.Mime.MediaTypeNames;

namespace YourCare_Application.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [AllowNull]
        public string? ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            try
            {
                returnUrl = returnUrl ?? Url.Content("~/");
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email, Name = Input.Email };
                    var result = await _userManager.CreateAsync(user, Input.Password);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = user.Id, code = code },
                            protocol: Request.Scheme);

                        #region email body 
                        var htmlTemplate =
                         @"<!DOCTYPE html>
                        <html lang='en'>
                        <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Email Confirmation</title>
                        <style>
                            body {
                                font-family: Arial, sans-serif;
                                background-color: #f4f4f4;
                                margin: 0;
                                padding: 0;
                            }
                            .container {
                                width: 100%;
                                max-width: 600px;
                                margin: 0 auto;
                                background-color: #ffffff;
                                border-radius: 8px;
                                overflow: hidden;
                                box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                            }
                            .header {
                                background-color: #2c7a7b;
                                color: #ffffff;
                                text-align: center;
                                padding: 20px;
                            }
                            .header h1 {
                                margin: 0;
                                font-size: 24px;
                            }
                            .content {
                                padding: 30px;
                                color: #333;
                            }
                            .content h2 {
                                color: #2c7a7b;
                            }
                            .content p {
                                line-height: 1.6;
                                margin-bottom: 20px;
                            }
                            .button-container {
                                text-align: center;
                                margin-top: 20px;
                            }
                            .button {
                                background-color: #3182ce;
                                text-decoration: none;
                                padding: 15px 25px;
                                border-radius: 5px;
                                font-size: 18px;
                                display: inline-block;
                            }
                            .button a{
                                color: #fff;
                            }
                            .footer {
                                text-align: center;
                                padding: 20px;
                                font-size: 12px;
                                background-color: #f4f4f4;
                                color: #777;
                            }
                            .footer a {
                                color: #3182ce;
                                text-decoration: none;
                            }
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h1>Welcome to YourCare!</h1>
                            </div>
                            <div class='content'>
                                <h2>Confirm Your Email</h2>
                                <p>Hello,</p>
                                <p>Thank you for signing up with <strong>YourCare</strong>. To complete your registration and activate your account, please confirm your email address by clicking the button below:</p>";

                        var htmlBodyLink = $"<div class='button-container'><a href = " +
                            $"'{HtmlEncoder.Default.Encode(callbackUrl)}' class='button'>Confirm Email</a></div><p>If the button doesn't work, you click the following link:" +
                            $"</p><p><a href = '{HtmlEncoder.Default.Encode(callbackUrl)}' >Click here</a></p></div>";

                        var htmlBody_2 = @"<div class='footer'>
                            <p>If you didn't create an account, please ignore this email.</p>
                            <p>&copy; 2024 YourCare. All rights reserved.</p>
                            </div>
                            </div>
                            </body>
                            </html>";

                        #endregion

                        var htmlBody = htmlTemplate + htmlBodyLink + htmlBody_2;

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            htmlBody);

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " - " + ex.StackTrace);
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
