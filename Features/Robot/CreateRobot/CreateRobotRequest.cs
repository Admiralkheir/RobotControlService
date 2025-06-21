using MediatR;
using RobotControlService.Domain.Entities;

namespace RobotControlService.Features.Robot.CreateRobot
{
    public record CreateRobotRequest(string Name, string Description, Position Position) : IRequest<CreateRobotResponse> ;
}
