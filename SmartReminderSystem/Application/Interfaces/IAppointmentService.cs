using SmartReminderSystem.Application.DTOs.ResponseDto;
using SmartReminderSystem.Application.DTOs;

namespace SmartReminderSystem.Application.Interfaces
{
    public interface IAppointmentService
    {
        Task<AppointmentResponseDto> CreateAppointmentAsync(AppointmentRequestDto appointmentDto);
        Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByPatientAsync(Guid patientId);
        Task<bool> DeleteAppointmentAsync(Guid id);
    }
}
