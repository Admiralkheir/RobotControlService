namespace RobotControlService.Exceptions
{
    public class CommandNotFoundException : NotFoundException
    {
        public CommandNotFoundException(string commandId)
            : base($"There is no command with this id: {commandId}")
        {
        }
    }
}
