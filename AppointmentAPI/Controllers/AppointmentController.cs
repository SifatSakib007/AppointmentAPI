using AppointmentAPI.Data;
using AppointmentAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppointmentAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]    
    public class AppointmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("check-auth")]
        public IActionResult CheckAuthentication()
        {
            var user = HttpContext.User.Identity;
            if (user == null || !user.IsAuthenticated)
            {
                Console.WriteLine("🔴 User is NOT authenticated.");
                return Unauthorized(new { message = "User is not authenticated" });
            }

            Console.WriteLine($"✅ Authenticated User: {user.Name}");
            return Ok(new { message = "User is authenticated", user = user.Name });
        }


        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] Appointment appointment)
        {
            if (appointment.AppointmentDateTime <= DateTime.Now)
            {
                return BadRequest(new { message = "Appointment date should be in the future" });
            }
            if (appointment.AppointmentDateTime > DateTime.Now.AddYears(1))
            {
                return BadRequest(new { message = "Appointment date cannot be more than 1 year in advance" });
            }

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, appointment);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            var appointment = await _context.Appointments.Include(a => a.Doctor).FirstOrDefaultAsync(a => a.Id == id);
            if (appointment == null)
            {
                return NotFound(new { message = "Appointment not found" });
            }
            return Ok(appointment);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await _context.Appointments.Include(a => a.Doctor).ToListAsync();

            if (appointments.Count == 0)
            {
                return NotFound(new { message = "No appointments found" });
            }

            return Ok(appointments);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] Appointment updateAppointment)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound(new { message = "Appointment not found" });
            }

            if(updateAppointment.AppointmentDateTime <= DateTime.Now)
            {
                return BadRequest(new { message = "Appointment date should be in the future" });
            }

            appointment.PatientName = updateAppointment.PatientName;
            appointment.PatentContact = updateAppointment.PatentContact;
            appointment.AppointmentDateTime = updateAppointment.AppointmentDateTime;
            appointment.DoctorId = updateAppointment.DoctorId;

            await _context.SaveChangesAsync();
            return Ok(appointment);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound(new { message = "Appointment not found" });
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Appointment deleted successfully" });
        }

    }
}
