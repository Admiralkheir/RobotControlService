using MediatR;

namespace RobotControlService.Features.Robot.DeleteRobot
{
    public record DeleteRobotRequest(string Name) : IRequest<DeleteRobotResponse>;
}
