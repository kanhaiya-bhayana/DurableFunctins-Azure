using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace SMS_Verification.Functions.Orchestrator
{
    public class PhoneVerificationOrchestrator(ILogger<PhoneVerificationOrchestrator> log)
    {
        [Function("RunOrchestrator")]
        public async Task<bool> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            var phoneNumber = context.GetInput<string>();
            if (string.IsNullOrEmpty(phoneNumber))
            {
                throw new ArgumentNullException(nameof(phoneNumber), "A phone number input is required.");
            }

            int challengeCode = await context.CallActivityAsync<int>("SendSms", phoneNumber);

            using (var timeoutCts = new CancellationTokenSource())
            {
                // The user has 90 seconds to respond with the code they received in the SMS message.
                DateTime expiration = context.CurrentUtcDateTime.AddSeconds(90);
                Task timeoutTask = context.CreateTimer(expiration, timeoutCts.Token);

                bool authorized = false;
                for (int retryCount = 0; retryCount <= 3; retryCount++)
                {
                    Task<int> challengeResponseTask = context.WaitForExternalEvent<int>("SmsChallengeResponse");

                    Task winner = await Task.WhenAny(challengeResponseTask, timeoutTask);
                    if (winner == challengeResponseTask)
                    {
                        // We got back a response! Compare it to the challenge code.
                        if (challengeResponseTask.Result == challengeCode)
                        {
                            authorized = true;
                            log.LogInformation("ATM activated successfully");
                            break;
                        }
                    }
                    else
                    {
                        // Timeout expired
                        log.LogInformation("Time expired");
                        break;
                    }
                }

                if (!timeoutTask.IsCompleted)
                {
                    // All pending timers must be complete or canceled before the function exits.
                    timeoutCts.Cancel();
                }

                return authorized;
            }
        }
    }
}
