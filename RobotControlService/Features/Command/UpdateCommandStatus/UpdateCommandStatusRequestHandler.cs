using MediatR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using RobotControlService.Data;
using RobotControlService.Domain.Entities;
using RobotControlService.Exceptions;

namespace RobotControlService.Features.Command.UpdateCommandStatus
{
    public class UpdateCommandStatusRequestHandler : IRequestHandler<UpdateCommandStatusRequest, UpdateCommandStatusResponse>
    {
        private readonly RobotDbContext _dbContext;
        public UpdateCommandStatusRequestHandler(RobotDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<UpdateCommandStatusResponse> Handle(UpdateCommandStatusRequest request, CancellationToken cancellationToken)
        {
            var command = await _dbContext.Commands.FirstOrDefaultAsync(c => c.Id == ObjectId.Parse(request.CommandId), cancellationToken);

            if (command == null)
            {
                throw new CommandNotFoundException(request.CommandId);
            }

            var robot = await _dbContext.Robots.FirstOrDefaultAsync(r => r.Id == command.RobotId, cancellationToken);

            // if command status is inprogress, update robot status, command status and current commad id
            // if command status is completed, update robot status, commmand status and current command id
            // if command status is failed, update robot status, command status and current command id
            var commandStatus = Enum.Parse<CommandStatus>(request.NewCommandStatus, true);

            switch (commandStatus)
            {
                case CommandStatus.InProgress:
                    //command
                    command.CommandStatus = CommandStatus.InProgress;
                    command.StartedDate = DateTime.UtcNow;
                    //robot
                    robot.CurrentCommandId = command.Id;
                    robot.Status = command.CommandType == CommandType.Move ? RobotStatus.Moving : RobotStatus.Rotating;
                    robot.LastSeenDate = DateTime.UtcNow;
                    break;

                case CommandStatus.Completed:
                    // command
                    command.CommandStatus = CommandStatus.Completed;
                    command.CompletedDate = DateTime.UtcNow;
                    //robot
                    robot.CurrentCommandId = null;
                    robot.Status = RobotStatus.Idle;
                    robot.LastSeenDate = DateTime.UtcNow;
                    robot.CurrentPosition = CalculatePosition(command.CommandType, command.CommandParameters, robot.CurrentPosition);
                    break;

                case CommandStatus.Failed:
                    // command
                    command.CommandStatus = CommandStatus.Failed;
                    command.CompletedDate = DateTime.UtcNow;
                    command.FailureReason = request.FailureReason;
                    // robot
                    robot.CurrentCommandId = null;
                    robot.Status = RobotStatus.Idle;
                    robot.LastSeenDate = DateTime.UtcNow;
                    break;
            }

            _dbContext.Commands.Update(command);
            _dbContext.Robots.Update(robot);

            // save changes to the database
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new UpdateCommandStatusResponse(robot.Name, robot.Status.ToString(), robot.CurrentPosition, robot.CurrentCommandId.ToString());
        }

        private Position CalculatePosition(CommandType commandType, Dictionary<string, string> commandParameters, Position currentPosition)
        {
            switch (commandType)
            {
                case CommandType.Move:
                    if (commandParameters.TryGetValue("distance", out var distanceObj) && double.TryParse(distanceObj, out var distance))
                    {
                        // get string direction value from command parameters
                        if (commandParameters.TryGetValue("direction", out var direction))
                        {
                            // calculate new position based on direction and distance
                            switch (direction.ToLower())
                            {
                                case "forward":
                                    currentPosition.Y += distance;
                                    break;
                                case "backward":
                                    currentPosition.Y -= distance;
                                    break;
                                case "right":
                                    currentPosition.X += distance;
                                    break;
                                case "left":
                                    currentPosition.X -= distance;
                                    break;
                            }
                        }
                    }
                    break;

                case CommandType.Rotate:
                    // get double degree from command parameters
                    if (commandParameters.TryGetValue("degrees", out var degreeObj) && double.TryParse(degreeObj, out var degree))
                    {
                        currentPosition.Orientation = (currentPosition.Orientation + degree) % 360;
                    }
                    break;
            }

            return currentPosition; // return the current position if parameters are not valid
        }
    }
}
