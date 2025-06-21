using MediatR;
using MongoDB.Bson;
using RobotControlService.Domain.Entities;

namespace RobotControlService.Features.Auth.UpdateUser
{
    public record UpdateUserRequest(string Username, string NewPassword, string NewRole, List<string> RobotNames) : IRequest<UpdateUserResponse>;
}
