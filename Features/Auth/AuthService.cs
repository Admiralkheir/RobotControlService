using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using RobotControlService.Domain.Entities;
using System.Configuration;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RobotControlService.Features.Auth
{
    public class AuthService : IAuthService
    {
        private const int SALT_SIZE = 16; // Size of the salt in bytes
        private const int HASH_SIZE = 32; // Size of the hash in bytes
        private const int ITERATIONS = 10000; // Number of iterations for PBKDF2
        private static readonly HashAlgorithmName HASH_ALGORITHM = HashAlgorithmName.SHA512;

        private readonly IConfiguration configuration;

        public AuthService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            string secretKey = configuration["Jwt:Secret"]!;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create claims based on user information
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
            claims.AddClaim(new Claim(JwtRegisteredClaimNames.Name, user.Username));

            // Add robot IDs if the user has any
            if (user.RobotIds != null && user.RobotIds.Count > 0)
            {
                foreach (var robotId in user.RobotIds)
                {
                    claims.AddClaim(new Claim("robotId", robotId.ToString()));
                }
            }

            // Add role claim
            claims.AddClaim(new Claim(ClaimTypes.Role, user.Role.ToString()));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ExpirationInMinutes")),
                SigningCredentials = credentials,
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"]
            };

            var handler = new JsonWebTokenHandler();

            string token = handler.CreateToken(tokenDescriptor);

            return token;
        }

        public string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SALT_SIZE);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, ITERATIONS, HASH_ALGORITHM, HASH_SIZE);

            return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            string[] parts = passwordHash.Split('-');
            byte[] hash = Convert.FromHexString(parts[0]);
            byte[] salt = Convert.FromHexString(parts[1]);

            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, ITERATIONS, HASH_ALGORITHM, HASH_SIZE);

            return CryptographicOperations.FixedTimeEquals(hash, inputHash);
        }
    }
}
