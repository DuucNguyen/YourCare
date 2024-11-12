using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourCare_Application.Models
{
    public class Timetable
    {
        [Key]
        public int Id { get; set; }

        public string DoctorID {  get; set; }

        [Required]
        public DateTime Date {  get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public bool IsAvailable { get; set; }


        [ForeignKey("DoctorID")]
        public virtual Doctor Doctor { get; set; }

        [NotMapped]
        public virtual Appointment Appointment { get; set; }

    }
}
