using HCMS.IdentityService.Domain.Commands;
using HCMS.IdentityService.Domain.DTOs;
using HCMS.IdentityService.Domain.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace HCMS.IdentityService.Controllers
{
    [ApiController]
    [Route("api/identity")]
    public class IdentityController : ControllerBase
    {
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto, [FromServices] RegisterHandler handler)
        {
            try
            {
                var command = new RegisterCommand(
                    dto.Email,
                    dto.Password,
                    dto.Role);

                await handler.Handle(command);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto, [FromServices] LoginHandler handler)
        {
            try
            {
                var command = new LoginCommand(
                    dto.Email,
                    dto.Password);

                var result = await handler.Handle(command);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
