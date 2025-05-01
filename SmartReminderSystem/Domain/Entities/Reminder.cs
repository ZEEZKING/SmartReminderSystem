namespace SmartReminderSystem.Domain.Entities
{
    public class Reminder : BaseEntity
    {
        public Guid AppointmentId { get; set; }
        public DateTime SentAt { get; set; }
        public string Method { get; set; } // SMS/Email  
    }
}
