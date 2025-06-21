using MediatR;

namespace RobotControlService.Features.Robot.UpdateRobot
{
    public record UpdateRobotRequest(string Name, string NewDescription) : IRequest<UpdateRobotResponse>;
}
