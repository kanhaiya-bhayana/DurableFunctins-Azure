using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using RequestApproval.DataContracts;

namespace RequestApproval
{
    public class EventCoordinatorOrchestrator
    {
        [Function(nameof(EventCoordinator))]
        public async Task EventCoordinator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            var inviteReq = context.GetContextInput<InviteFriendRequest>();

            // Send out the invite
            var sendIviteReq = new SendInviteRequest
            {
                Friend = inviteReq.Friend,
                InviteId = context.InstanceId
            };
            await context.CallActivityAsync(nameof(InviteFriendFunction.InviteFriend),sendIviteReq);

            // Set a reminder timer if no response
            using var timeoutCts = new CancellationTokenSource();
            var reminderDueTime = context.CurrentUtcDateTime.AddSeconds(10);
            var reminderTimeout = context.CreateTimer(reminderDueTime, timeoutCts.Token);

            // Wait for reminder or RSVP (whichever comes first)
            var rsvpForEvent = context.WaitForExternalEvent<bool>(EventNames.RsvpReceived, timeoutCts.Token);
            if (reminderTimeout == await Task.WhenAny(reminderTimeout, rsvpForEvent))
            {
                // Send a reminder
                inviteReq.ReminderCount++;
                await context.CallSubOrchestratorAsync(nameof(EventCoordinator), inviteReq);
            }
            else
            {
                // RSVP received
                timeoutCts.Cancel();
                if (rsvpForEvent.Result)
                {
                    // Yay!
                    await context.CallActivityAsync(nameof(InviteAcceptedFunction.InviteAccepted),inviteReq.Friend);
                }
                else
                {
                    // Invite new friend
                    var newInviteReq = new InviteFriendRequest() { Friend = "Lauren" };
                    await context.CallSubOrchestratorAsync(nameof(EventCoordinator), newInviteReq);
                }
            }
        }
    }
}