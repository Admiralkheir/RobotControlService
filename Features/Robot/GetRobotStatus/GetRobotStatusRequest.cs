using MediatR;

namespace RobotControlService.Features.Robot.GetRobotStatus
{
    public record GetRobotStatusRequest(string Name) : IRequest<GetRobotStatusResponse>;
}
