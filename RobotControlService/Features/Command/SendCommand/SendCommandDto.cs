namespace RobotControlService.Features.Command.SendCommand
{
    public record SendCommandDto(string Username, string RobotName, string CommandType, Dictionary<string, string> CommandParameters);
}
