using Appointment_HospitalService.DTO.RequestDto;
using Appointment_HospitalService.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Appointment_HospitalService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointment _appointmentService;
        private readonly HttpClient _httpClient;

        public AppointmentController(IAppointment appointmentService, HttpClient httpClient)
        {
            _appointmentService = appointmentService;
            _httpClient = httpClient;
        }


        [Authorize(Roles = "Patient")]
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentRequestDto appointment, int DoctorID)
        {
            try
            {
                var patientIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int patientId = Convert.ToInt32(patientIdClaim);
                var addedAppointment = await _appointmentService.CreateAppointment(appointment, patientId, DoctorID);
                return Ok(new { Success = true, Message = "Appointment added successfully", Data = addedAppointment });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred.", Error = ex.StackTrace });
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("GetByPatient")]
        public async Task<IActionResult> GetAllAppointmentsByPatient()
        {
            try
            {
                var patientId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var appointments = await _appointmentService.GetAllAppointmentsByPatient(patientId);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving appointments by doctor: {ex.Message}");
            }
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("GetByDoctor")]
        public async Task<IActionResult> GetAllAppointmentsByDoctor()
        {
            try
            {
                var doctorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var appointments = await _appointmentService.GetAllAppointmentsByDoctor(doctorId);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving appointments by doctor: {ex.Message}");
            }
        }

        [Authorize(Roles = "Patient,Doctor,Admin")]
        [HttpGet("GetAppointmentById")]
        public async Task<IActionResult> GetAppointmentsById()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var appointments = await _appointmentService.GetAppointmentsById(userId);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving appointments by patient: {ex.Message}");
            }
        }


    }
}
