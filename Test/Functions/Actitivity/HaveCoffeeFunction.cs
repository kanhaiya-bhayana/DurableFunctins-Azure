namespace Test.Functions.Actitivity
{
    public class HaveCoffeeFunction
        (ILogger<HaveCoffeeFunction> logger)
    {

        [Function(nameof(HaveCoffee))]
        public async Task HaveCoffee([ActivityTrigger] TaskActivityContext context)
        {
            logger.LogInformation($"[Started]: {nameof(HaveCoffee)}");

            logger.LogInformation("Making Coffee");
            await Task.Delay(RobotConstants.WorkflowStepDelay);
            logger.LogInformation("Here you go");

            await Task.Delay(RobotConstants.WorkflowStepDelay);
            logger.LogInformation("Ah... much better");

            logger.LogInformation($"Completed: {nameof(HaveCoffee)}");
        }

    }
}
