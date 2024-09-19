namespace Test.Functions.Actitivity
{
    public class FetchToolsFunction
    {
        private readonly ILogger<FetchToolsFunction> _logger;

        public FetchToolsFunction(ILogger<FetchToolsFunction> logger)
        {
            _logger = logger;
        }

        [Function(nameof(FetchTools))]
        public async Task<ToolsResponse> FetchTools(
            [ActivityTrigger] TaskActivityContext context)
        {
            _logger.LogInformation($"[Started]: {nameof(FetchTools)}");
            await Task.Delay(RobotConstants.WorkflowStepDelay);

            var tools = new ToolsResponse
            {
                MyTools = new List<string> {
                    "Screw Driver",
                    "Pliers",
                    "Hammer"
                },
                MyFriendsTools = new List<string>
                {
                    "Soldering Iron",
                    "Toolbox"
                }
            };
            _logger.LogInformation($"Found my {string.Join(", ", tools.MyTools)}");
            _logger.LogInformation($"Borrowing my friends {string.Join(", ", tools.MyFriendsTools)}");

            _logger.LogInformation($"[Completed]: {nameof(FetchTools)}");

            return tools;
        }
    }
}
