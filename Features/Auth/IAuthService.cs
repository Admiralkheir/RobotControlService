using RobotControlService.Domain.Entities;

namespace RobotControlService.Features.Auth
{
    public interface IAuthService
    {
        public string HashPassword(string password);
        public bool VerifyPassword(string password, string passwordHash);
        public string GenerateToken(User user);
    }
}
