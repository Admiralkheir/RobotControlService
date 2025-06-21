using MediatR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using RobotControlService.Data;
using RobotControlService.Exceptions;

namespace RobotControlService.Features.Command.GetCommand
{
    public class GetCommandRequestHandler : IRequestHandler<GetCommandRequest, GetCommandResponse>
    {
        private readonly RobotDbContext _dbContext;
        public GetCommandRequestHandler(RobotDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<GetCommandResponse> Handle(GetCommandRequest request, CancellationToken cancellationToken)
        {
            var command = await _dbContext.Commands.FirstOrDefaultAsync(c => c.Id == ObjectId.Parse(request.Id), cancellationToken);

            if (command == null)
            {
                throw new CommandNotFoundException(request.Id);
            }

            return new GetCommandResponse(command.Id.ToString(), command.RobotId.ToString(), command.UserId.ToString(), command.CommandStatus.ToString(), command.CommandType.ToString(), command.FailureReason, command.CommandParameters, command.CreatedDate, command.StartedDate, command.CompletedDate);
        }
    }
}
