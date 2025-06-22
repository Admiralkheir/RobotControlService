using MongoDB.Bson;
using RobotControlService.Domain.Entities;

namespace RobotControlService.Features.Auth.CreateUser
{
    public record CreateUserResponse(string UserId, string Username, DateTime CreatedDate, List<string> RobotIds, string Role);
}
