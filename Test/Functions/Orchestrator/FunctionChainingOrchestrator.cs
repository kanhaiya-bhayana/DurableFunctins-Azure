using Test.Functions.Actitivity;

namespace Test.Functions.Orchestrator
{
    public class FunctionChainingOrchestrator(ILogger<FunctionChainingOrchestrator> _logger)
    {
        [Function(nameof(FunctionChainingOrchestrator))]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            if (!context.IsReplaying)
            {

                _logger.LogInformation($"Started: {nameof(FunctionChainingOrchestrator)}");
            }
            var toolsTask = context.CallActivityAsync<ToolsResponse>(nameof(FetchToolsFunction.FetchTools));
            var partsTask = context.CallActivityAsync<PartsResponse>(nameof(FetchPartsFunction.FetchParts));
            await Task.WhenAll(toolsTask, partsTask);

            var coffee = await context.CallActivityAsync<bool>(nameof(HaveCoffeeFunction.HaveCoffee));

            var buildInput = new BuildShellInput { Tools = toolsTask.Result, Parts = partsTask.Result };

            var robot = await context.CallActivityAsync<RobotResponse>(nameof(BuildShellFunction.BuildShell), buildInput);
            robot = await context.CallActivityAsync<RobotResponse>(nameof(ProgramRobotFunction.ProgramRobot), robot);
            robot = await context.CallActivityAsync<RobotResponse>(nameof(TestRobotFunction.TestRobot), robot);

            _logger.LogInformation($"Completed robot. Programmed: {robot.IsProgrammed}, tested: {robot.IsTested}, solid: {robot.IsSolid}, epic: {robot.IsEpic} ");

            _logger.LogInformation($"[Completed]: {nameof(RunOrchestrator)}");
        }
    }
}
