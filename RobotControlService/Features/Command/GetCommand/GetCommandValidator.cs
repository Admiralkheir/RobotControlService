using FluentValidation;

namespace RobotControlService.Features.Command.GetCommand
{
    public class GetCommandValidator : AbstractValidator<GetCommandRequest>
    {
        public GetCommandValidator()
        {
            RuleFor(request => request.Id)
                .NotEmpty().WithMessage("Command ID cannot be empty.")
                .Length(24).WithMessage("Command ID must be 24 characters long.");
        }
    }
}
