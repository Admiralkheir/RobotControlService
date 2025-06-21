using RobotControlService.Domain.Entities;

namespace RobotControlService.Features.Robot.CreateRobot
{
    public record CreateRobotResponse(string RobotId, DateTime CreatedDate, string Name, string Description, bool IsDeleted, string Status, Position Position);
}
