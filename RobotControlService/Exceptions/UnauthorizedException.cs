namespace RobotControlService.Exceptions
{
    public class UnauthorizedException : ApplicationException
    {
        protected UnauthorizedException(string message)
            : base("Unauthorized", message)
        {

        }
    }
}
