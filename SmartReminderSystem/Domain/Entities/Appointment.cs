namespace SmartReminderSystem.Domain.Entities
{
    public class Appointment : BaseEntity
    {
        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Notes { get; set; }
        public bool ReminderSent { get; set; } = false;
    }
}
