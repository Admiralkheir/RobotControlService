using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto, CancellationToken cancellationToken)
        {
            var request = new LoginRequest(loginRequestDto.Username, loginRequestDto.Password);

            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }

    }
}
