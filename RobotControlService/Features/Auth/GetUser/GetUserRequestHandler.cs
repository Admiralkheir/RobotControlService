using MediatR;
using Microsoft.EntityFrameworkCore;
using RobotControlService.Data;
using RobotControlService.Domain.Entities;
using RobotControlService.Exceptions;

namespace RobotControlService.Features.Auth.GetUser
{
    public class GetUserRequestHandler : IRequestHandler<GetUserRequest, GetUserResponse>
    {
        private readonly RobotDbContext _dbContext;
        public GetUserRequestHandler(RobotDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<GetUserResponse> Handle(GetUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

            if (user == null)
            {
                throw new UserNotFoundException(request.Username);
            }

            return new GetUserResponse(user.Id.ToString(), user.Username, user.CreatedDate, user.IsDeleted, user.Role.ToString(), user.RobotIds?.Select(s=>s.ToString()).ToList());
        }
    }
}
