using MediatR;
using MongoDB.Bson;

namespace RobotControlService.Features.Auth.DeleteUser
{
    public record DeleteUserRequest(string Username) : IRequest<DeleteUserResponse>;
}
