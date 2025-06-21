using MediatR;
using Microsoft.EntityFrameworkCore;
using RobotControlService.Data;
using RobotControlService.Domain.Entities;
using RobotControlService.Exceptions;

namespace RobotControlService.Features.Auth.UpdateUser
{
    public class UpdateUserRequestHandler : IRequestHandler<UpdateUserRequest, UpdateUserResponse>
    {
        private readonly RobotDbContext _dbContext;
        private readonly IAuthService _authService;

        public UpdateUserRequestHandler(IAuthService authService, RobotDbContext dbContext)
        {
            _authService = authService;
            _dbContext = dbContext;
        }

        public async Task<UpdateUserResponse> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);


            if (user == null)
            {
                throw new UserNotFoundException(request.Username);
            }

            // get robotIds if RobotNames are provided
            if (request.RobotNames != null && request.RobotNames.Count > 0)
            {
                var robotIds = await _dbContext.Robots
                    .Where(r => request.RobotNames.Contains(r.Name))
                    .Select(r => r.Id)
                    .ToListAsync(cancellationToken);
                if (robotIds.Count == 0)
                {
                    throw new RobotNotFoundException(string.Join(",", request.RobotNames));
                }
                user.RobotIds = robotIds;
            }

            user.Role = Enum.Parse<UserRole>(request.NewRole, true); // Parse the new role from string to UserRole enum
            user.PasswordHash = _authService.HashPassword(request.NewPassword);

            _dbContext.Users.Update(user);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new UpdateUserResponse(user.Username, user.Id.ToString(), user.Role.ToString(), user.RobotIds?.Select(s=>s.ToString()).ToList());
        }
    }
}
