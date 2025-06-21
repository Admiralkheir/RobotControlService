namespace RobotControlService.Exceptions
{
    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException(string username)
            : base($"The User with this userName: {username} was not found")
        {

        }
    }
}
