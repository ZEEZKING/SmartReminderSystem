using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartReminderSystem.Application.DTOs;
using SmartReminderSystem.Application.Interfaces;

namespace SmartReminderSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] PatientRequestDto patientDto)
        {
            var createdPatient = await _patientService.CreatePatientAsync(patientDto);
            return Ok(createdPatient);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById(Guid id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null) return NotFound();
            return Ok(patient);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await _patientService.GetAllPatientsAsync();
            return Ok(patients);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(Guid id)
        {
            var result = await _patientService.DeletePatientAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
