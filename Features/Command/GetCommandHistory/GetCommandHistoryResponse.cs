using RobotControlService.Domain.Entities;

namespace RobotControlService.Features.Command.GetCommandHistory
{
    public record GetCommandHistoryResponse(PaginatedList<CommandHistoryResponseDto> CommandList);
}
