namespace RobotControlService.Features.Command.GetCommand
{
    public record GetCommandResponse(string CommandId, string RobotId, string UserId, string CommandStatus, string CommandType, string FailureReason, Dictionary<string, string> CommandParameters, DateTime? CreatedDate, DateTime? StartedDate, DateTime? ComplatedDate);
}
