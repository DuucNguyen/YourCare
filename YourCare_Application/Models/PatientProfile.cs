using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourCare_Application.Models
{
    public class PatientProfile
    {
        [Key]
        public string Id { get; set; }

        public string ApplicationUserID { get; set; }


        [MaxLength(256, ErrorMessage = "Tên phải nhỏ hơn 250 kí tự")]
        public string? Name { get; set; }

        public bool? Gender { get; set; }

        public DateTime? Dob { get; set; }

        [MaxLength(256, ErrorMessage = "Địa chỉ phải nhỏ hơn 250 kí tự !")]
        public string? Address { get; set; }

        [MaxLength(12, ErrorMessage = "Sai cú pháp !")]
        public string? IdentityNumber { get; set; }

        public string? InsuranceNumber { get; set; }

        [EmailAddress (ErrorMessage = "Không đúng định dạng email !")]
        public string? Email {  get; set; }

        [MaxLength(12, ErrorMessage = "Sai cú pháp !")]
        public string? PhoneNumber { get; set; }

        [MaxLength(100)]
        public string? Career {  get; set; }

        [MaxLength(100)]
        public string? Ethnic { get; set; }

        [ForeignKey("ApplicationUserID")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }

    }
}
