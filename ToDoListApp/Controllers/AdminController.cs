using Microsoft.AspNetCore.Mvc;
using ToDoListApp.BLL.Services;
using ToDoListApp.Enum;

namespace ToDoListApp.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthenticationController> _logger;

        public AdminController(IUserService userService, ILogger<AuthenticationController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("roles")]
        public async Task<IActionResult> AddRoles(Guid id, Roles roles)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid payload");

                var result = await _userService.AddRoleToUser(id, roles);

                if(result.Status==0)
                {
                    return BadRequest(result.Message);
                }

                return Ok(result.Message);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
