namespace SmartReminderSystem.Application.DTOs
{
    public class AppointmentRequestDto
    {
        public Guid PatientId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Notes { get; set; }
    }
}
