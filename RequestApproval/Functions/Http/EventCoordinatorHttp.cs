using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using RequestApproval.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestApproval.Functions.Http
{
    public class EventCoordinatorHttp
    {
        [Function(nameof(InviteFriendHttp))]
        public async Task<HttpResponseData> InviteFriendHttp(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient client)
        {
            var instanceId = "";
            try
            {
                var inviteReq = await req.GetFromBody<InviteFriendRequest>();
                instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(EventCoordinatorOrchestrator.EventCoordinator), inviteReq);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test: " +ex.Message);
            }
            return client.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
