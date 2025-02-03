using AppointmentAPI.Controllers;
using AppointmentAPI.Data;
using AppointmentAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Test
{
    [TestFixture]
    public class AppointmentControllerTest
    {
        private AppointmentController _controller;
        private ApplicationDbContext _context;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique database name
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new AppointmentController(_context);
        }


        private async Task SeedDatabase()
        {
            var appointment1 = new Appointment
            {
                Id = 1,
                PatientName = "John Doe",
                PatentContact = "123456789",
                AppointmentDateTime = DateTime.Now.AddDays(2),
                DoctorId = 1,
                Doctor = new Doctor
                {
                    DoctorId = 1,
                    DoctorName = "Dr. Smith"
                }
            };

            var appointment2 = new Appointment
            {
                Id = 2,
                PatientName = "Jane Smith",
                PatentContact = "987654321",
                AppointmentDateTime = DateTime.Now.AddDays(5),
                DoctorId = 2,
                Doctor = new Doctor
                {
                    DoctorId = 2,
                    DoctorName = "Dr. Johnson"
                }
            };

            // Remove all existing appointments to ensure a clean state
            var existingAppointments = await _context.Appointments.ToListAsync();
            _context.Appointments.RemoveRange(existingAppointments);
            await _context.SaveChangesAsync();

            // Add the new appointments
            await _context.Appointments.AddAsync(appointment1);
            await _context.Appointments.AddAsync(appointment2);

            await _context.SaveChangesAsync();

            // Debugging: Check Data is Persisted
            int count = await _context.Appointments.CountAsync();
            Console.WriteLine($"Total Appointments after seeding: {count}");
            var appointments = await _context.Appointments.AsNoTracking().ToListAsync();
            foreach (var appointment in appointments)
            {
                Console.WriteLine($"Appointment: {appointment.Id}, {appointment.PatientName}, {appointment.PatentContact}, {appointment.AppointmentDateTime}, {appointment.DoctorId}");
            }
        }


        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }


        [Test]
        public async Task GetAppointmentById_ShouldReturnAppointment_WhenExists()
        {
            // Arrange
            var appointment = new Appointment
            {
                Id = 1,
                PatientName = "John Doe",
                PatentContact = "1234567890",
                AppointmentDateTime = DateTime.Now.AddDays(1),
                DoctorId = 1,
                Doctor = new Doctor
                {
                    DoctorId = 1,
                    DoctorName = "Dr. Smith"
                }
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetAppointmentById(1) as OkObjectResult;

            // Log the retrieved appointment
            Console.WriteLine($"Result: {result?.Value}");

            // Assert
            Assert.NotNull(result, "Response should not be null.");
            var retrievedAppointment = result.Value as Appointment;
            Assert.NotNull(retrievedAppointment, "Appointment should not be null.");
            Assert.That(retrievedAppointment?.Id, Is.EqualTo(1), $"Expected ID 1 but got {retrievedAppointment?.Id}");
        }



        [Test]
        public async Task GetAppointmentById_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Act
            var result = await _controller.GetAppointmentById(99) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            var responseObj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(result.Value));
            Assert.AreEqual("Appointment not found", responseObj?.message?.ToString());
        }

        [Test]
        public async Task GetAllAppointments_ShouldReturnListOfAppointments()
        {
            // Arrange
            await SeedDatabase(); // Seed the database with test data

            // Act
            var result = await _controller.GetAllAppointments() as OkObjectResult;

            // Log the retrieved data (for debugging)
            Console.WriteLine($"Result: {result?.Value}");

            // Assert
            Assert.NotNull(result, "Response should not be null.");
            var appointments = result.Value as List<Appointment>;
            Assert.NotNull(appointments, "Appointments list should not be null.");
            Console.WriteLine($"Appointments Count: {appointments?.Count}");
            Assert.That(appointments.Count, Is.EqualTo(2), $"Expected 2 appointments, but got {appointments.Count}");
        }




        [Test]
        public async Task UpdateAppointment_ShouldReturnUpdatedAppointment_WhenSuccessful()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                context.Appointments.Add(new Appointment
                {
                    Id = 1,
                    PatientName = "Original Name",
                    PatentContact = "123456789",
                    AppointmentDateTime = DateTime.Now.AddDays(5),
                    DoctorId = 2
                });
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var controller = new AppointmentController(context);

                var updatedAppointment = new Appointment
                {
                    Id = 1,
                    PatientName = "Updated Name",
                    PatentContact = "999999999",
                    AppointmentDateTime = DateTime.Now.AddDays(10),
                    DoctorId = 3
                };

                // Act
                var result = await controller.UpdateAppointment(1, updatedAppointment) as OkObjectResult;

                // Assert
                Assert.NotNull(result);
                var updated = JsonConvert.DeserializeObject<Appointment>(JsonConvert.SerializeObject(result.Value));
                Assert.That(updated?.PatientName, Is.EqualTo("Updated Name"));
                Assert.That(updated?.PatentContact, Is.EqualTo("999999999"));
                Assert.That(updated?.DoctorId, Is.EqualTo(3));
            }
        }


        [Test]
        public async Task UpdateAppointment_ShouldReturnBadRequest_WhenDateIsPast()
        {
            // Arrange
            var existingAppointment = new Appointment
            {
                Id = 1,
                PatientName = "Existing Name",
                PatentContact = "888888888",
                AppointmentDateTime = DateTime.Now.AddMinutes(10),
                DoctorId = 3
            };

            // Add the existing appointment to the database
            _context.Appointments.Add(existingAppointment);
            await _context.SaveChangesAsync();

            var updatedAppointment = new Appointment
            {
                Id = 1,
                PatientName = "Updated Name",
                PatentContact = "999999999",
                AppointmentDateTime = DateTime.Now.AddMinutes(-5),
                DoctorId = 3
            };

            // Act
            var result = await _controller.UpdateAppointment(1, updatedAppointment) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            var responseObj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(result.Value));
            Assert.AreEqual("Appointment date should be in the future", responseObj?.message?.ToString());
        }


        [Test]
        public async Task UpdateAppointment_ShouldReturnNotFound_WhenAppointmentDoesNotExist()
        {
            // Arrange
            var updatedAppointment = new Appointment
            {
                Id = 99,
                PatientName = "New Name",
                PatentContact = "000000000",
                AppointmentDateTime = DateTime.Now.AddDays(2),
                DoctorId = 1
            };

            // Act
            var result = await _controller.UpdateAppointment(99, updatedAppointment) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            var responseObj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(result.Value));
            Assert.AreEqual("Appointment not found", responseObj?.message?.ToString());
        }

        [Test]
        public async Task DeleteAppointment_ShouldReturnOk_WhenDeletedSuccessfully()
        {
            // Arrange
            var appointment = new Appointment
            {
                Id = 1,
                PatientName = "John Doe",
                PatentContact = "123456789",
                AppointmentDateTime = DateTime.Now.AddDays(2),
                DoctorId = 1,
                Doctor = new Doctor
                {
                    DoctorId = 1,
                    DoctorName = "Dr. Smith"
                }
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteAppointment(1) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var responseObj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(result.Value));
            Assert.AreEqual("Appointment deleted successfully", responseObj?.message?.ToString());
        }


        [Test]
        public async Task DeleteAppointment_ShouldReturnNotFound_WhenAppointmentDoesNotExist()
        {
            // Act
            var result = await _controller.DeleteAppointment(99) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            var responseObj = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(result.Value));
            Assert.AreEqual("Appointment not found", responseObj?.message?.ToString());
        }
    }
}
