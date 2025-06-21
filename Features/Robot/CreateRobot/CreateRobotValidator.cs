using FluentValidation;
using RobotControlService.Domain.Entities;

namespace RobotControlService.Features.Robot.CreateRobot
{
    public class CreateRobotValidator : AbstractValidator<CreateRobotRequest>
    {
        public CreateRobotValidator()
        {
            RuleFor(request => request.Name)
                .NotEmpty().WithMessage("Robot Name is required.")
                .Length(3, 20).WithMessage("Robot Name must be between 3 and 20 characters long.");

            RuleFor(request => request.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MinimumLength(50).WithMessage("Description must be at least 50 characters long.")
                .MaximumLength(500).WithMessage("Password must not exceed 500 characters.");

            RuleFor(request => request.Position)
                .NotEmpty().WithMessage("Position is required.")
                .Must(pos => pos.Orientation < 360 && pos.Orientation > 0).WithMessage("Orientation must be between 0 and 360 degrees.");

        }
    }
}
