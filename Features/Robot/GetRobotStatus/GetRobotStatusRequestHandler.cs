using MediatR;
using Microsoft.EntityFrameworkCore;
using RobotControlService.Data;
using RobotControlService.Exceptions;

namespace RobotControlService.Features.Robot.GetRobotStatus
{
    public class GetRobotStatusRequestHandler : IRequestHandler<GetRobotStatusRequest, GetRobotStatusResponse>
    {
        private readonly RobotDbContext _dbContext;
        public GetRobotStatusRequestHandler(RobotDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<GetRobotStatusResponse> Handle(GetRobotStatusRequest request, CancellationToken cancellationToken)
        {
            var robot = await _dbContext.Robots.FirstOrDefaultAsync(r => r.Name == request.Name && r.IsDeleted == false, cancellationToken);

            if (robot == null)
            {
                throw new RobotNotFoundException(request.Name);
            }

            return new GetRobotStatusResponse(robot.Name, robot.Status.ToString(), robot.CurrentPosition, robot.CurrentCommandId?.ToString());
        }
    }
}
