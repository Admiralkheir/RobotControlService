using MongoDB.Bson;
using RobotControlService.Domain.Entities;

namespace RobotControlService.Features.Auth.UpdateUser
{
    public record UpdateUserDto(string Username, string NewPassword, string NewRole, List<string>? RobotNames);
}
