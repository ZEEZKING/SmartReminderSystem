using SmartReminderSystem.Infrastructure.SMS.Interface;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace SmartReminderSystem.Infrastructure.SMS.Services
{
    public class TwilioSmsService : ISmsService
    {
        private readonly IConfiguration _configuration;
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromPhoneNumber;

        public TwilioSmsService(IConfiguration configuration)
        {
            _configuration = configuration;
            _accountSid = _configuration["Twilio:AccountSid"];
            _authToken = _configuration["Twilio:AuthToken"];
            _fromPhoneNumber = _configuration["Twilio:PhoneNumber"];
        }

        public async Task<bool> SendSmsAsync(string to, string message)
        {
            try
            {
                TwilioClient.Init(_accountSid, _authToken);

                var messageResult = await MessageResource.CreateAsync(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(_fromPhoneNumber),
                    to: new Twilio.Types.PhoneNumber(to)
                );

                Console.WriteLine($"Twilio Response: SID={messageResult.Sid}, Status={messageResult.Status}, ErrorCode={messageResult.ErrorCode}, ErrorMessage={messageResult.ErrorMessage}");


                return messageResult.ErrorCode == null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Twilio Error: {ex.Message}");
                return false;
            }
        }
    }
}
