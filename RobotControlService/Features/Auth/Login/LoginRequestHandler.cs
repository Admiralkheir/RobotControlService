using MediatR;
using Microsoft.EntityFrameworkCore;
using RobotControlService.Data;
using RobotControlService.Exceptions;

namespace RobotControlService.Features.Auth.Login
{
    public class LoginRequestHandler : IRequestHandler<LoginRequest, LoginResponse>
    {
        private readonly IAuthService _authService;
        private readonly RobotDbContext _dbContext;
        public LoginRequestHandler(IAuthService authService, RobotDbContext dbContext)
        {
            _authService = authService;
            _dbContext = dbContext;
        }
        public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

            if (user == null)
            {
                throw new UserNotFoundException(request.Username);
            }

            var verified = _authService.VerifyPassword(request.Password, user.PasswordHash);

            if (!verified)
            {
                throw new PasswordWrongException();
            }

            var token = _authService.GenerateToken(user);

            return new LoginResponse(token);
        }
    }
}
