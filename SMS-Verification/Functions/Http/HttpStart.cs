

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using SMS_Verification.Functions.Orchestrator;

namespace SMS_Verification.Functions.Http
{
    public class HttpStart
    {
        [Function("HttpStartPhoneVerification")]
        public static async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient starter)
        {
            // Parse the query parameters from the request URL
            var queryParams = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            string phoneNumber = queryParams["phoneNumber"];

            // Validate phone number input
            if (string.IsNullOrEmpty(phoneNumber))
            {
                // Return a 400 Bad Request response if the phone number is missing
                var badRequestResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                await badRequestResponse.WriteStringAsync("A phone number is required.");
                return badRequestResponse;
            }

            // Log the phone number if needed (commented log)
            // log.LogInformation($"Starting phone verification for {phoneNumber}");

            // Start the orchestration for phone verification
            string instanceId = await starter.ScheduleNewOrchestrationInstanceAsync(nameof(PhoneVerificationOrchestrator.RunOrchestrator), phoneNumber);

            // Create a custom response to return the orchestration status details
            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            var statusQueryGetUri = $"{req.Url.Scheme}://{req.Url.Host}/runtime/webhooks/durabletask/instances/{instanceId}/status";
            var checkStatusResponse = new
            {
                message = "Phone verification started successfully.",
                instanceId = instanceId,
                statusQueryGetUri = statusQueryGetUri
            };

            await response.WriteAsJsonAsync(checkStatusResponse);
            return response;
        }
    }

}
