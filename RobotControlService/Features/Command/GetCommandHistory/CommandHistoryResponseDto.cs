namespace RobotControlService.Features.Command.GetCommandHistory
{
    public record CommandHistoryResponseDto(string CommandId, string UserId, string RobotId, string CommandType, string CommandStatus, DateTime CreatedDate, DateTime? StartedDate, DateTime? ComplatedDate, Dictionary<string,string> CommandParameters, string FailureReason);
}
