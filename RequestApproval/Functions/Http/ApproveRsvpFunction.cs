using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;

namespace RequestApproval;

public class ApproveRsvpFunction
{
    [Function(nameof(RsvpHttp))]
    public async Task<HttpResponseData> RsvpHttp(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get",Route = "RsvpHttp/{invitationId}/{isAccepted}")]
        HttpRequestData req,
        string invitationId,
        bool isAccepted,
        [DurableClient] DurableTaskClient client
        )
    {
        await client.RaiseEventAsync(invitationId, EventNames.RsvpReceived, isAccepted);
        return req.CreateResponse(HttpStatusCode.OK);
    }
}