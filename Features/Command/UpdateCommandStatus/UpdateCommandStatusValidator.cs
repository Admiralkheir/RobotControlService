using FluentValidation;
using MediatR;
using RobotControlService.Domain.Entities;

namespace RobotControlService.Features.Command.UpdateCommandStatus
{
    public class UpdateCommandStatusValidator : AbstractValidator<UpdateCommandStatusRequest>
    {
        public UpdateCommandStatusValidator()
        {
            RuleFor(request => request.CommandId)
                .NotEmpty().WithMessage("Command ID cannot be empty.")
                .Length(24).WithMessage("Command ID must be 24 characters long.");

            RuleFor(request => request.NewCommandStatus)
                .IsEnumName(typeof(CommandStatus), false).WithMessage("Command must be a valid CommandStatus value.")
                .NotEmpty().WithMessage("Command status is required.");

        }
    }
}
