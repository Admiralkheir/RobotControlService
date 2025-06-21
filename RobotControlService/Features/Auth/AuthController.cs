using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using RobotControlService.Features.Auth.CreateUser;
using RobotControlService.Features.Auth.DeleteUser;
using RobotControlService.Features.Auth.GetUser;
using RobotControlService.Features.Auth.Login;
using RobotControlService.Features.Auth.UpdateUser;

namespace RobotControlService.Features.Auth
{
    [ApiVersion(1)]
    [ApiController]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
        {
            var request = new LoginRequest(loginDto.Username, loginDto.Password);

            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }

        [HttpGet("GetUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUser([FromQuery] string username, CancellationToken cancellationToken)
        {
            var request = new GetUserRequest(username);

            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }

        [HttpDelete("DeleteUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser([FromQuery] string username, CancellationToken cancellationToken)
        {
            var request = new DeleteUserRequest(username);

            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }

        [HttpPost("CreateUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto, CancellationToken cancellationToken)
        {
            var request = new CreateUserRequest(createUserDto.Username, createUserDto.Password, createUserDto.Role, createUserDto.RobotIds);

            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }

        [HttpPut("UpdateUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto, CancellationToken cancellationToken)
        {
            var request = new UpdateUserRequest(updateUserDto.Username, updateUserDto.NewPassword, updateUserDto.NewRole, updateUserDto.RobotNames);

            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }

    }
}
