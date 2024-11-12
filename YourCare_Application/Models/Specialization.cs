using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourCare_Application.Models
{
    public class Specialization
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Specialization is required !")]
        [MaxLength(150, ErrorMessage = "Max lenght <= 150 character !")]
        public string Name { get; set; }

        [Required]
        public byte[] Image { get; set; }

        [NotMapped]
        public string? ImageString { get; set; }
        public virtual ICollection<DoctorSpecialization> DoctorSpecializations { get; set; }
    }
}
