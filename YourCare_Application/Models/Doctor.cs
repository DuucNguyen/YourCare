using System.ComponentModel.DataAnnotations.Schema;

namespace YourCare_Application.Models
{
    public class Doctor : ApplicationUser
    {
        public int? Experience { get; set; }
        public string? DoctorDescription { get; set; }
        public string? DoctorTitle { get; set; }
        public int SpecializationID { get; set; }

        [ForeignKey("SpecializationID")]
        public virtual ICollection<DoctorSpecialization> DoctorSpecializations { get; set; }

        [NotMapped]
        public List<Specialization> Specializations { get; set; }
        [NotMapped]
        public List<Timetable> Timetables { get; set; }
    }
}
