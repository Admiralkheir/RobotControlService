using MongoDB.Bson;
using RobotControlService.Domain.Entities;

namespace RobotControlService.Features.Auth.GetUser
{
    public record GetUserResponse(string UserId, string Username, DateTime CreatedDate, bool IsDeleted, string Role, List<string> RobotIds);
}
