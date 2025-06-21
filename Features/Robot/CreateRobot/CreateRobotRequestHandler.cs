using MediatR;
using Microsoft.EntityFrameworkCore;
using RobotControlService.Data;
using RobotControlService.Exceptions;
using System.Runtime.CompilerServices;

namespace RobotControlService.Features.Robot.CreateRobot
{
    public class CreateRobotRequestHandler : IRequestHandler<CreateRobotRequest, CreateRobotResponse>
    {
        private readonly RobotDbContext _dbContext;

        public CreateRobotRequestHandler(RobotDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<CreateRobotResponse> Handle(CreateRobotRequest request, CancellationToken cancellationToken)
        {
            var robot = await _dbContext.Robots.FirstOrDefaultAsync(s => s.Name == request.Name, cancellationToken);

            if(robot != null)
            {
                throw new RobotExistsException($"Robot with name {request.Name} already exists.");
            }

            robot = new Domain.Entities.Robot()
            {
                Name = request.Name,
                Description = request.Description,
                CurrentPosition = request.Position,
                CreatedDate = DateTime.UtcNow,
                Status = Domain.Entities.RobotStatus.Idle
            };

            await _dbContext.Robots.AddAsync(robot, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new CreateRobotResponse(robot.Id.ToString(),robot.CreatedDate,robot.Name,robot.Description,robot.IsDeleted,robot.Status.ToString(),robot.CurrentPosition);

        }
    }
}
