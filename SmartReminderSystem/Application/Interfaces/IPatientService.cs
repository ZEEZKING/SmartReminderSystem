using SmartReminderSystem.Application.DTOs.ResponseDto;
using SmartReminderSystem.Application.DTOs;

namespace SmartReminderSystem.Application.Interfaces
{
    public interface IPatientService
    {
        Task<PatientResponseDto> CreatePatientAsync(PatientRequestDto patientDto);
        Task<PatientResponseDto> GetPatientByIdAsync(Guid id);
        Task<IEnumerable<PatientResponseDto>> GetAllPatientsAsync();
        Task<bool> DeletePatientAsync(Guid id);
    }
}
