using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RobotControlService.Features.Auth.CreateUser;
using RobotControlService.Features.Auth.DeleteUser;
using RobotControlService.Features.Auth.GetUser;
using RobotControlService.Features.Auth.Login;
using RobotControlService.Features.Auth.UpdateUser;
using RobotControlService.Features.Robot.CreateRobot;
using RobotControlService.Features.Robot.DeleteRobot;
using RobotControlService.Features.Robot.GetRobotStatus;
using RobotControlService.Features.Robot.UpdateRobot;

namespace RobotControlService.Features.Robot
{
    [ApiVersion(1)]
    [ApiController]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class RobotController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RobotController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("CreateRobot")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRobot([FromBody] CreateRobotDto robotDto, CancellationToken cancellationToken)
        {
            var request = new CreateRobotRequest(robotDto.Name, robotDto.Description, robotDto.Position);

            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }

        [HttpDelete("DeleteRobot")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRobot([FromQuery] string robotName, CancellationToken cancellationToken)
        {
            var request = new DeleteRobotRequest(robotName);

            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }

        [HttpGet("GetRobotStatus")]
        [Authorize(Roles = "Admin,Operator,Monitor")]
        public async Task<IActionResult> GetRobotStatus([FromQuery] string robotName, CancellationToken cancellationToken)
        {
            var request = new GetRobotStatusRequest(robotName);

            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }

        [HttpPut("UpdateRobot")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRobot([FromBody] UpdateRobotDto updateRobotDto, CancellationToken cancellationToken)
        {
            var request = new UpdateRobotRequest(updateRobotDto.Name, updateRobotDto.NewDescription);

            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }
    }
}
