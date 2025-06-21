using MediatR;
using Microsoft.EntityFrameworkCore;
using RobotControlService.Data;
using RobotControlService.Exceptions;

namespace RobotControlService.Features.Robot.UpdateRobot
{
    public class UpdateRobotRequestHandler : IRequestHandler<UpdateRobotRequest, UpdateRobotResponse>
    {
        private readonly RobotDbContext _dbContext;
        public UpdateRobotRequestHandler(RobotDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<UpdateRobotResponse> Handle(UpdateRobotRequest request, CancellationToken cancellationToken)
        {
            var robot = await _dbContext.Robots.FirstOrDefaultAsync(s => s.Name == request.Name && s.IsDeleted == false);

            if (robot == null)
            {
                throw new RobotNotFoundException(request.Name);
            }

            robot.Description = request.NewDescription;

            _dbContext.Robots.Update(robot);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new UpdateRobotResponse(robot.Name, robot.Description, robot.Id.ToString());
        }
    }
}
