using MediatR;
using MongoDB.Bson;
using RobotControlService.Domain.Entities;

namespace RobotControlService.Features.Auth.CreateUser
{
    public record CreateUserRequest(string Username, string Password, string Role, List<ObjectId> RobotIds) : IRequest<CreateUserResponse>;
}