using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartReminderSystem.Application.DTOs;
using SmartReminderSystem.Application.Interfaces;
using System.Text.Json;

namespace SmartReminderSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IOpenAIService _openAIService;
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IOpenAIService openAIService, IAppointmentService appointmentService)
        {
            _openAIService = openAIService;
            _appointmentService = appointmentService;
        }

        [HttpPost("parse-and-create")]
        public async Task<IActionResult> ParseAndCreate([FromBody] string naturalText)
        {
            if (string.IsNullOrWhiteSpace(naturalText))
                return BadRequest("Input cannot be empty.");

            string aiResponse;
            try
            {
                aiResponse = await _openAIService.ParseAppointmentAsync(naturalText);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing AI response: {ex.Message}");
            }

            if (string.IsNullOrWhiteSpace(aiResponse))
                return BadRequest("AI response is empty.");

            AppointmentRequestDto appointmentDto;
            try
            {
                appointmentDto = JsonSerializer.Deserialize<AppointmentRequestDto>(aiResponse);
            }
            catch (JsonException)
            {
                return BadRequest("Failed to parse appointment details.");
            }

            if (appointmentDto == null)
                return BadRequest("Could not extract valid appointment details.");

            var createdAppointment = await _appointmentService.CreateAppointmentAsync(appointmentDto);
            return Ok(createdAppointment);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentRequestDto appointmentDto)
        {
            var createdAppointment = await _appointmentService.CreateAppointmentAsync(appointmentDto);
            return Ok(createdAppointment);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetAppointmentsByPatient(Guid patientId)
        {
            var appointments = await _appointmentService.GetAppointmentsByPatientAsync(patientId);
            return Ok(appointments);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(Guid id)
        {
            var result = await _appointmentService.DeleteAppointmentAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
