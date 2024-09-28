using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace RequestApproval;

public class InviteAcceptedFunction(
    ILogger<InviteAcceptedFunction> _logger)
{
    [Function(nameof(InviteAccepted))]
    public bool InviteAccepted([ActivityTrigger] string name)
    {
        _logger.LogInformation( $"Woohoo! '{name}' said yes, we're going...");
        return true;
    }
}