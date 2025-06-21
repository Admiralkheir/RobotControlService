using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RobotControlService.Features.Command.GetCommand;
using RobotControlService.Features.Command.GetCommandHistory;
using RobotControlService.Features.Command.SendCommand;
using RobotControlService.Features.Command.UpdateCommandStatus;
using RobotControlService.Features.Robot.CreateRobot;
using RobotControlService.Features.Robot.DeleteRobot;
using RobotControlService.Features.Robot.GetRobotStatus;
using RobotControlService.Features.Robot.UpdateRobot;

namespace RobotControlService.Features.Command
{
    [ApiVersion(1)]
    [ApiController]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class CommandController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CommandController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetCommand")]
        [Authorize(Roles = "Admin, Monitor, Operator")]
        public async Task<IActionResult> GetCommand([FromQuery] string Id, CancellationToken cancellationToken)
        {
            var request = new GetCommandRequest(Id);

            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }

        [HttpGet("GetCommandHistory")]
        [Authorize(Roles = "Admin, Operator, Monitor")]
        public async Task<IActionResult> GetCommandHistory([FromQuery] string robotName, CancellationToken cancellationToken, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var request = new GetCommandHistoryRequest(robotName, pageIndex, pageSize);

            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }

        [HttpPost("SendCommand")]
        [Authorize(Roles = "Admin,Operator")]
        public async Task<IActionResult> SendCommand([FromBody] SendCommandDto sendCommandDto, CancellationToken cancellationToken)
        {
            var request = new SendCommandRequest(sendCommandDto.Username, sendCommandDto.RobotName, sendCommandDto.CommandType, sendCommandDto.CommandParameters);

            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }

        [HttpPut("UpdateCommandStatus")]
        [Authorize(Roles = "Robot,Admin")]
        public async Task<IActionResult> UpdateCommandStatus([FromBody] UpdateCommandStatusDto updateCommandStatusDto, CancellationToken cancellationToken)
        {
            var request = new UpdateCommandStatusRequest(updateCommandStatusDto.CommandId, updateCommandStatusDto.NewCommandStatus, updateCommandStatusDto.FailureReason);

            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }

    }
}
