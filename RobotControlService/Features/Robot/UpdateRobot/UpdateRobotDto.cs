using MediatR;

namespace RobotControlService.Features.Robot.UpdateRobot
{
    public record UpdateRobotDto(string Name, string NewDescription) : IRequest<UpdateRobotResponse>;
}
