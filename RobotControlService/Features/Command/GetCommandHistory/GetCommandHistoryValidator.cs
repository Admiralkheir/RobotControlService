using FluentValidation;

namespace RobotControlService.Features.Command.GetCommandHistory
{
    public class GetCommandHistoryValidator : AbstractValidator<GetCommandHistoryRequest>
    {
        public GetCommandHistoryValidator()
        {
            RuleFor(request => request.RobotName)
                .NotEmpty().WithMessage("Robot name cannot be empty.")
                .Length(3, 20).WithMessage("Robot name must be between 3 and 20 characters long.");

            RuleFor(request => request.PageIndex)
                .GreaterThanOrEqualTo(1).WithMessage("Page index must be greater than or equal to 1.");

            RuleFor(request => request.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
        }
    }
}
