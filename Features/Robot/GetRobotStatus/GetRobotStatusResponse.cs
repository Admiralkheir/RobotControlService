using RobotControlService.Domain.Entities;

namespace RobotControlService.Features.Robot.GetRobotStatus
{
    public record GetRobotStatusResponse(string RobotName, string RobotStatus, Position Position, string CurrentCommandId);
}
