using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourCare_Application.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() : base()
        {

        }

        [Display(Name = "Full Name")]
        [MaxLength(256, ErrorMessage = "Address must be less than 256 characters")]
        public string? Name { get; set; }

        public bool? Gender { get; set; }

        public DateTime? Dob { get; set; }

        [MaxLength(256, ErrorMessage = "Address must be less than 256 characters")]
        public string? Address { get; set; }

        [DisplayName("Avatar")]
        public byte[]? Avatar { get; set; }

        [NotMapped]
        public string? AvatarString { get; set; }

        public bool? IsActive { get; set; }

        [NotMapped]
        [DisplayName("Role")]
        public string RoleId { get; set; }


        [NotMapped]
        public virtual ICollection<Appointment> DoctorAppointments { get; set; }
        [NotMapped]
        public virtual ICollection<Appointment> PatientAppointments { get; set; }

        [NotMapped]
        public virtual ICollection<Appointment> CreatedAppointments { get; set; }

        [NotMapped]
        public virtual ICollection<Timetable> Timetables { get; set; }

        [NotMapped]
        public virtual ICollection<PatientProfile> PatientProfiles { get; set; }



    }
}
