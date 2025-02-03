using System.ComponentModel.DataAnnotations;

namespace AppointmentAPI.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PatientName { get; set; } = string.Empty;

        [Required]
        public string PatentContact { get; set; } = string.Empty;

        [Required]
        public DateTime AppointmentDateTime { get; set; } 

        [Required]
        public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; }
    }
}
