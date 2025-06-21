using MediatR;

namespace RobotControlService.Features.Command.GetCommand
{
    public record GetCommandRequest(string Id) : IRequest<GetCommandResponse>;
}
