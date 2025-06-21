using FluentValidation;

namespace RobotControlService.Features.Robot.GetRobotStatus
{
    public class GetRobotStatusValidator : AbstractValidator<GetRobotStatusRequest>
    {
        public GetRobotStatusValidator()
        {
            RuleFor(request => request.Name)
                .NotEmpty().WithMessage("Robot Name is required.")
                .Length(3, 20).WithMessage("Robot Name must be between 3 and 20 characters long.");
        }
    }

}