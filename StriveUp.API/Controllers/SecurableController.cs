using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StriveUp.API.Interfaces;

namespace StriveUp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class SecurableController : ControllerBase
    {
        private readonly ISecurableService _securableService;

        public SecurableController(ISecurableService securableService)
        {
            _securableService = securableService;
        }
        [HttpGet("mapboxToken")]
        public async Task<ActionResult<string>> GetMapboxToken()
        {
            var token = await _securableService.GetMapboxTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return NotFound("Token not found");
            }
            return Ok(token);
        }
    }
}
