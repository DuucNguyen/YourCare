using System.ComponentModel.DataAnnotations.Schema;

namespace YourCare_Application.Models
{
    public class DoctorSpecialization
    {
        [ForeignKey("FK_DoctorSpecialization_Doctor")]
        public string DoctorID { get; set; }

        [ForeignKey("FK_DoctorSpecialization_Specialization")]
        public int SpecializationID { get; set; }

        public virtual Doctor Doctor { get; set; }
        public virtual Specialization Specialization { get; set; }

    }
}
