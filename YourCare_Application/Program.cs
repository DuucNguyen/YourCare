using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ReflectionIT.Mvc.Paging;
using YourCare_Application.Models;
using YourCare_Application.Repository;
using YourCare_Application.Repository.Interfaces;
using YourCare_Application.ServerHubs;
using YourCare_Application.Services;
using YourCare_Application.Services.EmailSender;


namespace YourCare_Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddSignalR();

            #region DbContext + Identity
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConStr"));
                options.EnableSensitiveDataLogging();
            },
                ServiceLifetime.Transient
            );

            builder.Services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromSeconds(10); // Adjust as needed
            });



            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            #endregion

            #region EmailService
            IConfiguration _configuration = builder.Configuration;

            builder.Services.Configure<MailKitEmailSenderOptions>(
                _configuration.GetSection("MailKit:SMTP"));

            builder.Services.AddTransient<IEmailSender, MailKitEmailSender>();

            // config inactive cookie timeout
            builder.Services.ConfigureApplicationCookie(o =>
            {
                o.ExpireTimeSpan = TimeSpan.FromDays(5);
                o.SlidingExpiration = true;
            });

            // changes all data protection tokens timeout 
            builder.Services.Configure<DataProtectionTokenProviderOptions>(o =>
            o.TokenLifespan = TimeSpan.FromHours(3));
            #endregion

            #region Scope
            //avoid multithread
            builder.Services.AddScoped<ApplicationDbContext>();
            builder.Services.AddTransient<ApplicationDbContext>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ISpecializationRepository, SpecializationRepository>();
            builder.Services.AddScoped<IDoctorSpecializationRepository, DoctorSpecializationRepository>();
            builder.Services.AddScoped<ITimetableRepository, TimetableRepository>();
            builder.Services.AddScoped<IPatientProfileRepository, PatientProfileRepository>();
            builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            #endregion

            #region Policy
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole("Admin"));

                options.AddPolicy("Admin_View_Doctor", policy =>
                    policy.RequireClaim("Admin_View_Doctor", "1"));

                options.AddPolicy("Admin_Create_Doctor", policy =>
                    policy.RequireClaim("Admin_View_Doctor", "1"));

                options.AddPolicy("Admin_Update_Doctor", policy =>
                    policy.RequireClaim("Admin_View_Doctor", "1"));

                options.AddPolicy("Admin_Delete_Doctor", policy =>
                    policy.RequireClaim("Admin_View_Doctor", "1"));


            });
            #endregion

            #region Pagination
            builder.Services.AddPaging(options =>
            {
                options.ViewName = "Pagination";
                options.PageParameterName = "pageIndex";
            });
            #endregion

            #region External authentications
            builder.Services.AddAuthentication()
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = _configuration["Authentication:Google:ClientId"];
                    googleOptions.ClientSecret = _configuration["Authentication:Google:ClientSecret"];
                    googleOptions.AccessDeniedPath = "/AccessDeniedPathInfo";
                    googleOptions.Scope.Add("email");
                })
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = _configuration["Authentication:Facebook:AppId"];
                    facebookOptions.AppSecret = _configuration["Authentication:Facebook:AppSecret"];
                    facebookOptions.AccessDeniedPath = "/AccessDeniedPathInfo";
                    facebookOptions.Scope.Add("email");
                });
            #endregion


            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("DoctorOnly", policy => policy.RequireRole("Doctor"));
                options.AddPolicy("AdminOrDoctor", policy => policy.RequireRole("Admin", "Doctor"));
                options.AddPolicy("AdminOrPatient", policy => policy.RequireRole("Admin", "Patient"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();
            app.MapHub<ServerHub>("/hub");



            app.Run();
        }
    }
}