using System.ComponentModel.DataAnnotations;

namespace AppointmentAPI.Models
{
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; } 
        [Required]
        public string DoctorName { get; set; } = string.Empty;
    }
}
