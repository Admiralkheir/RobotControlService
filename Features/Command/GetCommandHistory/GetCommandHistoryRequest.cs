using MediatR;

namespace RobotControlService.Features.Command.GetCommandHistory
{
    public record GetCommandHistoryRequest(string RobotName, int PageIndex, int PageSize) : IRequest<GetCommandHistoryResponse>;
}
