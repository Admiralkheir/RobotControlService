namespace RobotControlService.Exceptions
{
    public class RobotNotFoundException : NotFoundException
    {
        public RobotNotFoundException(string robotNames) 
            : base($"Robots with these usernames: {robotNames} was not found.")
        { 

        }
    
    }
}
