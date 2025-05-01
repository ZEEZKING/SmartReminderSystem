using Microsoft.AspNetCore.Mvc;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

[Route("api/[controller]")]
[ApiController]
public class SmsController : ControllerBase
{
    [HttpPost("send-test")]
    public IActionResult SendTestSms()
    {
        try
        {
            TwilioClient.Init("AC7bfdbb9c99aab975badea428b55fbd8b", "c6de0f172f38b4152216827afc5224de");

            var message = MessageResource.Create(
                body: "Test SMS from Twilio",
                from: new PhoneNumber("+18125794421"),
                to: new PhoneNumber("+2349027010528") // Replace with patient's phone number
            );

            return Ok(new { MessageSid = message.Sid, Status = message.Status.ToString() });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}
