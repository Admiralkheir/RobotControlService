using FluentValidation;
using MediatR;
using RobotControlService.Domain.Entities;

namespace RobotControlService.Features.Auth.UpdateUser
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserValidator()
        {
            RuleFor(request => request.Username)
                .NotEmpty().WithMessage("Username is required.")
                .Length(3, 50).WithMessage("Username must be between 3 and 50 characters long.");

            RuleFor(request => request.NewPassword)
                    .NotEmpty().WithMessage("Password is required.")
                    .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                    .MaximumLength(16).WithMessage("Password must not exceed 16 characters.");

            RuleFor(request => request.NewRole)
                    .IsEnumName(typeof(UserRole)).WithMessage("Role must be a valid UserRole value.")
                    .NotEmpty().WithMessage("At least one role is required.");

        }
    }
}
