using MediatR;
using Microsoft.EntityFrameworkCore;
using RobotControlService.Data;
using RobotControlService.Exceptions;

namespace RobotControlService.Features.Robot.DeleteRobot
{
    public class DeleteRobotRequestHandler : IRequestHandler<DeleteRobotRequest, DeleteRobotResponse>
    {
        private readonly RobotDbContext _dbContext;
        public DeleteRobotRequestHandler(RobotDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<DeleteRobotResponse> Handle(DeleteRobotRequest request, CancellationToken cancellationToken)
        {
            var robot = await _dbContext.Robots.FirstOrDefaultAsync(r => r.Name == request.Name, cancellationToken);
            if (robot == null)
            {
                throw new RobotNotFoundException(request.Name);
            }

            robot.IsDeleted = true;
            _dbContext.Robots.Update(robot);

            await _dbContext.SaveChangesAsync(cancellationToken);
            
            return new DeleteRobotResponse(robot.Id.ToString(), robot.Name, robot.IsDeleted);
        }
    }
}
