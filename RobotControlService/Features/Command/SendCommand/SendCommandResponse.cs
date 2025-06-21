using RobotControlService.Domain.Entities;

namespace RobotControlService.Features.Command.SendCommand
{
    public record SendCommandResponse(string RobotName, string RobotStatus, Position Position, string CurrentCommandId);
}
