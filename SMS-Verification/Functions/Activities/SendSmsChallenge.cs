using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio;

namespace SMS_Verification.Functions.Activities
{
    public class SendSmsChallenge
    {
        [Function("SendSms")]
        public static int SendSms(
            [ActivityTrigger] string phoneNumber,
            ILogger<SendSmsChallenge> log)
        {
            // Set your Twilio Account SID and Auth Token from environment variables
            string accountSid = Environment.GetEnvironmentVariable("TwilioAccountSid");
            string authToken = Environment.GetEnvironmentVariable("TwilioAuthToken");
            string fromPhoneNumber = Environment.GetEnvironmentVariable("TwilioPhoneNumber");

            if (string.IsNullOrEmpty(accountSid) || string.IsNullOrEmpty(authToken) || string.IsNullOrEmpty(fromPhoneNumber))
            {
                throw new ArgumentNullException("Twilio credentials or phone number not set.");
            }

            // Initialize Twilio client
            TwilioClient.Init(accountSid, authToken);

            // Generate a random 4-digit verification code
            var rand = new Random();
            int challengeCode = rand.Next(1000, 10000); // Ensures a 4-digit code

            //log.LogInformation($"Sending verification code {challengeCode} to {phoneNumber}.");

            // Send SMS using Twilio API
            var message = MessageResource.Create(
                body: $"Your verification code is {challengeCode:0000}",
                from: new PhoneNumber(fromPhoneNumber),
                to: new PhoneNumber(phoneNumber)
            );

            //log.LogInformation($"Message sent with SID: {message.Sid}");

            return challengeCode;
        }
    }

}

