using AutoMapper;
using SmartReminderSystem.Application.DTOs.ResponseDto;
using SmartReminderSystem.Application.DTOs;
using SmartReminderSystem.Application.Interfaces;
using SmartReminderSystem.Domain.Entities;
using SmartReminderSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SmartReminderSystem.Infrastructure.SMS.Interface;
using Hangfire;
using SmartReminderSystem.Infrastructure.Services;

namespace SmartReminderSystem.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ISmsService _smsService;
        private readonly IEmailService _emailService;

        public AppointmentService(ApplicationDbContext context, IMapper mapper, ISmsService smsService, IEmailService emailService)
        {
            _context = context;
            _mapper = mapper;
            _smsService = smsService;
            _emailService = emailService;
        }

        public async Task<AppointmentResponseDto> CreateAppointmentAsync(AppointmentRequestDto appointmentDto)
        {
            var appointment = _mapper.Map<Appointment>(appointmentDto);
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            var patient = await _context.Patients.FindAsync(appointmentDto.PatientId);
            if (patient != null)
            {
                var formattedPhoneNumber = FormatPhoneNumber(patient.PhoneNumber);

                // 📲 Immediate SMS Confirmation with Error Handling
                var confirmationMessage = $"Hello {patient.FullName}, your appointment is scheduled for {appointment.AppointmentDate:MMMM dd, yyyy HH:mm}.";
                try
                {
                    var smsSent = await _smsService.SendSmsAsync(formattedPhoneNumber, confirmationMessage);
                    if (!smsSent)
                    {
                        Console.WriteLine($"[SMS Error] Failed to send confirmation SMS to {formattedPhoneNumber}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SMS Exception] {ex.Message}");
                }

                // ⏰ Schedule Reminders (30 mins before)
                var reminderTime = appointment.AppointmentDate.AddMinutes(-30);
                var reminderSms = $"Reminder: Your appointment is at {appointment.AppointmentDate:HH:mm}.";
                var emailSubject = "📅 Appointment Reminder";
                var emailBody = GenerateEmailTemplate(patient.FullName, appointment.AppointmentDate);

                // 🗓️ Schedule SMS Reminder
                BackgroundJob.Schedule<ISmsService>(
                    sms => sms.SendSmsAsync(formattedPhoneNumber, reminderSms),
                    reminderTime
                );

                // 📧 Schedule Email Reminder with Error Handling
                BackgroundJob.Schedule(() =>
                    _emailService.SendEmailAsync(patient.Email, emailSubject, emailBody),
                    reminderTime
                );

            }

            return _mapper.Map<AppointmentResponseDto>(appointment);
        }


        public async Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByPatientAsync(Guid patientId)
        {
            var appointments = await _context.Appointments
                                             .Include(a => a.Patient)
                                             .Where(a => a.PatientId == patientId)
                                             .ToListAsync();

            return _mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);
        }

        public async Task<bool> DeleteAppointmentAsync(Guid id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return false;

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return true;
        }

        private string FormatPhoneNumber(string phoneNumber)
        {
            if (phoneNumber.StartsWith("0"))
            {
                // Remove leading zero and add country code (+234 for Nigeria)
                return "+234" + phoneNumber.Substring(1);
            }
            return phoneNumber;
        }

        private string GenerateEmailTemplate(string fullName, DateTime appointmentDate)
        {
            return $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <h2>Appointment Reminder</h2>
                        <p>Hi <strong>{fullName}</strong>,</p>
                        <p>This is a reminder for your upcoming appointment:</p>
                        <ul>
                            <li><strong>Date:</strong> {appointmentDate:MMMM dd, yyyy}</li>
                            <li><strong>Time:</strong> {appointmentDate:HH:mm}</li>
                        </ul>
                        <p>Please contact us if you need to reschedule.</p>
                        <br/>
                        <p>Thanks,</p>
                        <p><strong>Smart Reminder System Team</strong></p>
                    </body>
                    </html>";
        }


    }
}
