using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Functions.Actitivity
{
    public class TestRobotFunction(ILogger<TestRobotFunction> _logger)
    {
        [Function(nameof(TestRobot))]
        public async Task<RobotResponse> TestRobot([ActivityTrigger] RobotResponse robot)
        {
            _logger.LogInformation($"[Started]: {nameof(TestRobot)}");

            await Task.Delay(RobotConstants.WorkflowStepDelay);
            robot.IsTested = true;

            _logger.LogInformation($"Robot tested: {robot.IsTested}");
            _logger.LogInformation($"[Completed]: {nameof(TestRobot)}");

            return robot;
        }
    }
}
