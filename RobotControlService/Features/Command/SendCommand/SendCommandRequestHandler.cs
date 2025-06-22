using MediatR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using RobotControlService.Data;
using RobotControlService.Exceptions;
using System.Data;

namespace RobotControlService.Features.Command.SendCommand
{
    public class SendCommandRequestHandler : IRequestHandler<SendCommandRequest, SendCommandResponse>
    {
        private readonly RobotDbContext _dbContext;
        public SendCommandRequestHandler(RobotDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<SendCommandResponse> Handle(SendCommandRequest request, CancellationToken cancellationToken)
        {
            var robot = await _dbContext.Robots.FirstOrDefaultAsync(r => r.Name == request.RobotName && r.IsDeleted == false, cancellationToken);

            if (robot == null)
            {
                throw new RobotNotFoundException(request.RobotName);
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username && u.IsDeleted == false, cancellationToken);

            if (user == null)
            {
                throw new UserNotFoundException(request.Username);
            }

            var command = new RobotControlService.Domain.Entities.Command()
            {
                RobotId = robot.Id,
                UserId = user.Id,
                CommandType = Enum.Parse<RobotControlService.Domain.Entities.CommandType>(request.CommandType, true),
                CreatedDate = DateTime.UtcNow,
                CommandStatus = RobotControlService.Domain.Entities.CommandStatus.Queued,
                CommandParameters = request.CommandParameters
            };


            await _dbContext.Commands.AddAsync(command, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new SendCommandResponse(command.Id.ToString(), robot.Name, robot.Status.ToString(), robot.CurrentPosition, robot.CurrentCommandId.ToString());
        }
    }
}
