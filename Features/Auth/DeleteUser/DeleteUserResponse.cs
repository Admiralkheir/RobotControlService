using MongoDB.Bson;

namespace RobotControlService.Features.Auth.DeleteUser
{
    public record DeleteUserResponse(string UserId, string Username);
}
