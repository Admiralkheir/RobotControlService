namespace RobotControlService.Exceptions
{
    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException(string usernane)
            : base($"The User with this username: {usernane} was not found")
        {

        }
    }
}
