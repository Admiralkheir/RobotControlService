using FluentValidation;
using RobotControlService.Domain.Entities;

namespace RobotControlService.Features.Command.SendCommand
{
    public class SendCommandValidator : AbstractValidator<SendCommandRequest>
    {
        public SendCommandValidator()
        {
            RuleFor(request => request.RobotName)
                .NotEmpty().WithMessage("Robot Name is required.");

            RuleFor(request => request.Username)
                .NotEmpty().WithMessage("User Name is required.");

            RuleFor(request => request.CommandType)
                .IsEnumName(typeof(CommandType), false).WithMessage("Command must be a valid CommandType value.")
                .NotEmpty().WithMessage("Command type is required.");

            RuleFor(request => request.CommandParameters)
                .NotNull().WithMessage("Parameters cannot be null.");

            When(request => request.CommandType.ToUpperInvariant() == "MOVE", () =>
            {
                RuleFor(request => request.CommandParameters)
                    .Must(p => p.ContainsKey("direction"))
                    .WithMessage("Move command requires a 'direction' parameter.")
                    .Must(p =>
                    {
                        string directionValue = p["direction"];
                        string direction = directionValue?.ToString()?.ToLowerInvariant();

                        // Valid directions: forward, backward, left, right
                        return direction == "forward" ||
                               direction == "backward" ||
                               direction == "left" ||
                               direction == "right";
                    })
                    .WithMessage("Direction must be one of: forward, backward, left, right.")
                    .Must(p => p.ContainsKey("distance"))
                    .WithMessage("Move command requires a 'distance' parameter.")
                    .Must(p =>
                    {
                        string distanceValue = p["distance"];
                        double distance;

                        if (double.TryParse(distanceValue, out var jsonDoubleVal))
                            distance = jsonDoubleVal;
                        else
                            return false;

                        return distance > 0;
                    })
                    .WithMessage("Move command requires a positive numeric distance value.");
            });

            When(request => request.CommandType.ToUpperInvariant() == "ROTATE", () =>
            {
                RuleFor(request => request.CommandParameters)
                    .Must(p => p.ContainsKey("degrees"))
                    .WithMessage("Rotate command requires a 'degrees' parameter.")
                    .Must(p =>
                    {
                        string degreesValue = p["degrees"];
                        double degrees;

                        // Handle common numeric types directly
                        if (double.TryParse(degreesValue, out var doubleVal))
                            degrees = doubleVal;
                        else
                            return false;

                        return true; // Any numeric value for degrees is acceptable
                    })
                    .WithMessage("Rotate command requires a numeric degrees value.");
            });
        }
    }
}
