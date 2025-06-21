using FluentValidation;

namespace RobotControlService.Features.Robot.DeleteRobot
{
    public class DeleteRobotValidator : AbstractValidator<DeleteRobotRequest>
    {
        public DeleteRobotValidator()
        {
            RuleFor(request => request.Name)
                .NotEmpty().WithMessage("Robot Name is required.")
                .Length(3, 20).WithMessage("Robot Name must be between 3 and 20 characters long.");
        }
    }
}
