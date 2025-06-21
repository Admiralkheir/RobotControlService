using MediatR;

namespace RobotControlService.Features.Command.UpdateCommandStatus
{
    public record UpdateCommandStatusRequest(string CommandId, string NewCommandStatus, string FailureReason) : IRequest<UpdateCommandStatusResponse>;
}
