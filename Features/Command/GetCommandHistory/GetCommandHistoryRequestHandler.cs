using MediatR;
using Microsoft.EntityFrameworkCore;
using RobotControlService.Data;
using RobotControlService.Domain.Entities;
using RobotControlService.Exceptions;
using System.Drawing.Printing;
using System.Linq;
using System.Numerics;

namespace RobotControlService.Features.Command.GetCommandHistory
{
    public class GetCommandHistoryRequestHandler : IRequestHandler<GetCommandHistoryRequest, GetCommandHistoryResponse>
    {
        private readonly RobotDbContext _dbContext;
        public GetCommandHistoryRequestHandler(RobotDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<GetCommandHistoryResponse> Handle(GetCommandHistoryRequest request, CancellationToken cancellationToken)
        {
            var robot = await _dbContext.Robots.FirstOrDefaultAsync(r => r.Name == request.RobotName, cancellationToken);

            if (robot == null)
            {
                throw new RobotNotFoundException(request.RobotName);
            }

            var commands = await _dbContext.Commands.Where(c => c.RobotId == robot.Id)
                .OrderBy(c => c.CreatedDate)
                .AsNoTracking()
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new CommandHistoryResponseDto(
                    c.Id.ToString(),
                    c.UserId.ToString(),
                    c.RobotId.ToString(),
                    c.CommandType.ToString(),
                    c.CommandStatus.ToString(),
                    c.CreatedDate,
                    c.StartedDate, // Fix: Handle nullable DateTime  
                    c.CompletedDate, // Fix: Handle nullable DateTime  
                    c.CommandParameters,
                    c.FailureReason
                ))
                .ToListAsync(cancellationToken);

            var count = await _dbContext.Commands.CountAsync(cancellationToken);

            var totalPages = (int)Math.Ceiling(count / (double)request.PageSize);

            return new GetCommandHistoryResponse(new PaginatedList<CommandHistoryResponseDto>(commands, request.PageIndex, totalPages));
        }
    }
}
