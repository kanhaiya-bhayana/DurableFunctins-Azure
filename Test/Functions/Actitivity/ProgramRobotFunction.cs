namespace Test.Functions.Actitivity
{
    public class ProgramRobotFunction(ILogger<ProgramRobotFunction> _logger)
    {
        [Function(nameof(ProgramRobot))]
        public async Task<RobotResponse> ProgramRobot([ActivityTrigger] RobotResponse robot)
        {
            _logger.LogInformation($"[Started]: {nameof(ProgramRobot)}");

            await Task.Delay(RobotConstants.WorkflowStepDelay);

            robot.IsProgrammed = true;
            _logger.LogInformation($"Robot programmed: {robot.IsProgrammed}");
            _logger.LogInformation($"[Completed]: {nameof(ProgramRobot)}");

            return robot;
        }
    }
}
