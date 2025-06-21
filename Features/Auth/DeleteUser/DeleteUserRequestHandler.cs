using MediatR;
using Microsoft.EntityFrameworkCore;
using RobotControlService.Data;
using RobotControlService.Exceptions;

namespace RobotControlService.Features.Auth.DeleteUser
{
    public class DeleteUserRequestHandler : IRequestHandler<DeleteUserRequest, DeleteUserResponse>
    {
        private readonly RobotDbContext _dbContext;

        public DeleteUserRequestHandler(RobotDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<DeleteUserResponse> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

            if (user == null)
            {
                throw new UserNotFoundException(request.Username);
            }

            user.IsDeleted = true;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new DeleteUserResponse(user.Id.ToString(), user.Username);
        }
    }
}
