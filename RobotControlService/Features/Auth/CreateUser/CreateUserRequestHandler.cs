using MediatR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using RobotControlService.Data;
using RobotControlService.Domain.Entities;
using RobotControlService.Exceptions;

namespace RobotControlService.Features.Auth.CreateUser
{
    public class CreateUserRequestHandler : IRequestHandler<CreateUserRequest, CreateUserResponse>
    {
        private readonly IAuthService _authService;
        private readonly RobotDbContext _dbContext;

        public CreateUserRequestHandler(IAuthService authService, RobotDbContext dbContext)
        {
            _authService = authService;
            _dbContext = dbContext;
        }

        public async Task<CreateUserResponse> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

            if (user != null)
            {
                throw new UserExistsException(request.Username);
            }

            var passwordHash = _authService.HashPassword(request.Password);

            user = new User()
            {
                PasswordHash = passwordHash,
                Username = request.Username,
                Role = Enum.Parse<UserRole>(request.Role,true),
                RobotIds = request.RobotIds?.Select(s=>ObjectId.Parse(s)).ToList(),
                CreatedDate = DateTime.UtcNow
            };

            await _dbContext.Users.AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new CreateUserResponse(user.Id.ToString(), user.Username, user.CreatedDate, user.RobotIds?.Select(s => s.ToString()).ToList(), user.Role.ToString());
        }
    }
}
