namespace RobotControlService.Features.Command.UpdateCommandStatus
{
    public record UpdateCommandStatusDto(string CommandId, string NewCommandStatus, string? FailureReason);
}
