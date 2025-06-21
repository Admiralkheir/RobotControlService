using FluentValidation;
using MediatR;
using RobotControlService.Domain.Entities;

namespace RobotControlService.Features.Robot.UpdateRobot
{
    public class UpdateRobotValidator : AbstractValidator<UpdateRobotRequest>
    {
        public UpdateRobotValidator()
        {
            RuleFor(request => request.Name)
                .NotEmpty().WithMessage("Robot Name is required.")
                .Length(3, 20).WithMessage("Robot Name must be between 3 and 20 characters long.");

            RuleFor(request => request.NewDescription)
                .NotEmpty().WithMessage("Description is required.")
                .MinimumLength(50).WithMessage("Description must be at least 50 characters long.")
                .MaximumLength(500).WithMessage("Password must not exceed 500 characters.");
        }
    }
}
