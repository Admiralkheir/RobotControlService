namespace RobotControlService.Exceptions
{
    public class RobotExistsException : BadRequestException
    {
        public RobotExistsException(string robotName) 
            : base($"The Robot with this Name: {robotName} already exists")
        {
            
        }
    }
}
