namespace RobotControlService.Exceptions
{
    public class UserExistsException : BadRequestException
    {
        public UserExistsException(string username)
            : base($"The User with this username: {username} already exists")
        {

        }
    }
}
