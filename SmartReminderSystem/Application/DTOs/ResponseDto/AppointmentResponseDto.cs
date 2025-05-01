namespace SmartReminderSystem.Application.DTOs.ResponseDto
{
    public class AppointmentResponseDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Notes { get; set; }
        public bool ReminderSent { get; set; }
    }
}
