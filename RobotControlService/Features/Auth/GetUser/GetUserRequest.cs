using MediatR;

namespace RobotControlService.Features.Auth.GetUser
{
    public record GetUserRequest(string Username) : IRequest<GetUserResponse>;
}
