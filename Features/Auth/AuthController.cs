using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using RobotControlService.Features.Auth.Login;

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
        public async Task<IActionResult> Login([FromBody] LoginDto loginRequestDto, CancellationToken cancellationToken)
        {
            var request = new LoginRequest(loginRequestDto.Username, loginRequestDto.Password);

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
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto createUserRequestDto, CancellationToken cancellationToken)
        {
            var request = new CreateUserRequest(createUserRequestDto.Username, createUserRequestDto.Password, createUserRequestDto.Roles);

            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }


    }
}
