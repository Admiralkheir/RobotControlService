using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using RobotControlService.Domain.Entities;

namespace RobotControlService.Features.Command.SendCommand
{
    public record SendCommandResponse(string CommandId,string RobotName, string RobotStatus, Position Position, string CurrentCommandId);
}
