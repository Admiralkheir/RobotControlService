using RobotControlService.Domain.Entities;

namespace RobotControlService.Features.Command.UpdateCommandStatus
{
    public record UpdateCommandStatusResponse(string RobotName, string RobotStatus, Position Position, string CurrentCommandId);
}
