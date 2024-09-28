using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace RequestApproval;

public class InviteFriendFunction(
    ILogger<InviteFriendFunction> _logger)
{
    [Function(nameof(InviteFriend))]
    public bool InviteFriend(
        [ActivityTrigger] SendInviteRequest request, TaskActivityContext context)
        {
            _logger.LogInformation($"Sent invite to friend '{request.Friend}'!");

            var acceptUrl = $"http://localhost:7167/api/RsvpHttp/{request.InviteId}";

            _logger.LogInformation($"To accept click here: '{acceptUrl}/true'");
            _logger.LogInformation($"To decline click here: '{acceptUrl}/false'");
            return true;
        }
}