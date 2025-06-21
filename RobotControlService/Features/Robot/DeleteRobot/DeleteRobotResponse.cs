namespace RobotControlService.Features.Robot.DeleteRobot
{
    public record DeleteRobotResponse(string RobotId, string Name, bool IsDeleted);
}
