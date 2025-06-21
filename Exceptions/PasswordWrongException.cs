namespace RobotControlService.Exceptions
{
    public class PasswordWrongException : UnauthorizedException
    {
        public PasswordWrongException() : base("Password is wrong")
        {
            
        }
    }
}
