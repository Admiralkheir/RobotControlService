using RobotControlService.Domain.Entities;

namespace RobotControlService.Features.Robot.CreateRobot
{
    public record CreateRobotDto(string Name, string Description, Position Position);
}
