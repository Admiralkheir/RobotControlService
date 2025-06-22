using MongoDB.Bson;

namespace RobotControlService.Features.Auth.CreateUser
{
    public record CreateUserDto(string Username, string Password, string Role, List<string>? RobotIds);
}
