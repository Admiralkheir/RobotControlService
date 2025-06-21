using Microsoft.EntityFrameworkCore;
using RobotControlService.Domain.Entities;
using RobotControlService.Features.Auth;

namespace RobotControlService.Data
{
    public class Seed
    {
        public static async Task SeedAdminUserAsync(RobotDbContext dbContext, IAuthService authService, IConfiguration configuration)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == "TestAdmin");

            if (user == null)
            {
                var adminUser = new User
                {
                    Username = "TestAdmin",
                    // We can read from Azure Key Vault or other secure storage
                    PasswordHash = authService.HashPassword(configuration.GetValue<string>("TestAdminPassword")!), 
                    Role = UserRole.Admin,
                    CreatedDate = DateTime.UtcNow
                    //RobotIds = new List<ObjectId>()
                };
                await dbContext.Users.AddAsync(adminUser);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
