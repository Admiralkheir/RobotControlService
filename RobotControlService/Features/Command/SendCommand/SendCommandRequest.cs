using MediatR;

namespace RobotControlService.Features.Command.SendCommand
{
    public record SendCommandRequest(string Username, string RobotName, string CommandType, Dictionary<string, string> CommandParameters) : IRequest<SendCommandResponse>;
}
