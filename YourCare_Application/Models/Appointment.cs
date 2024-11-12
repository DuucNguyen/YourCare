using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static YourCare_Application.Constants.StatusConstant;

namespace YourCare_Application.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string DoctorID { get; set; }

        [ForeignKey("DoctorID")]
        public virtual ApplicationUser Doctor { get; set; }

        [Required]
        public string PatientProfileID { get; set; }

        public string? PatientNote { get; set; }
        public string? DoctorDianosis { get; set; }
        public string? DoctorNote { get; set; }

        [ForeignKey("PatientProfileID")]
        public virtual PatientProfile PatientProfile { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        [Required]
        public int TimetableID { get; set; }

        [ForeignKey("TimetableID")]
        public virtual Timetable TimeTable { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual ApplicationUser CreatedByUser { get; set; }

        public DateTime UpdatedOn { get; set; }

        public int? PatientRating { get; set; }
        public string? PatientFeedBack { get; set; }

        public virtual Status Status { get; set; }

    }
}
