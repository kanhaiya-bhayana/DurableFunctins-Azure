namespace Test.Functions.Actitivity
{
    public class FetchPartsFunction(ILogger<FetchPartsFunction> logger)
    {
        [Function(nameof(FetchParts))]
        public async Task<PartsResponse> FetchParts([ActivityTrigger] TaskActivityContext context)
        {
            logger.LogInformation($"[Started]: {nameof(FetchParts)}");
            await Task.Delay(RobotConstants.WorkflowStepDelay);

            var parts = new PartsResponse
            {
                MyParts = new List<string> {
                    "Screws",
                    "Motors",
                    "Duct tape"
                }
            };
            logger.LogInformation($"Found parts {string.Join(", ", parts.MyParts)}");
            logger.LogInformation($"[Completed]: {nameof(FetchParts)}");
            return parts;
        }
    }
}
