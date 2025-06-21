using MongoDB.Bson;
using RobotControlService.Domain.Entities;

namespace RobotControlService.Features.Auth.UpdateUser
{
    public record UpdateUserResponse(string Username, string UserId, string Role, List<string> RobotIds);
}
