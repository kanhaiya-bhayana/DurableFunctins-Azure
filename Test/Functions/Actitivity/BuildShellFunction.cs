namespace Test.Functions.Actitivity
{
    public class BuildShellFunction(ILogger<BuildShellFunction> _logger)
    {
        [Function(nameof(BuildShell))]
        public async Task<RobotResponse> BuildShell([ActivityTrigger] BuildShellInput buildInput)
        {
            _logger.LogInformation($"[Started]: {nameof(BuildShell)}");

            await Task.Delay(RobotConstants.WorkflowStepDelay);

            var result = new RobotResponse
            {
                IsEpic = true,
                IsSolid = true,
                IsProgrammed = false,
                IsTested = false
            };

            _logger.LogInformation($"Shell built: Epic: {result.IsEpic}, Solid: {result.IsSolid}");

            _logger.LogInformation($"[Completed]: {nameof(BuildShell)}");
            return result;
        }
    }
}
