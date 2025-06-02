using MediatR;

namespace RobotControlService.Features.Auth.Login
{
    public record LoginRequest(string Username, string Password) : IRequest<LoginResponse>;
}