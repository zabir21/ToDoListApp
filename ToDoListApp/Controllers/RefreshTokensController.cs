using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDoListApp.BLL.Services;
using ToDoListApp.Contracts.Requests;
using ToDoListApp.DAL.Entity.Identity;
using ToDoListApp.Enum;

namespace ToDoListApp.Controllers
{
    [Route("api/token")]
    [ApiController]
    public class RefreshTokensController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;

        public RefreshTokensController(IAuthService authService, ILogger<AuthenticationController> logger, IConfiguration configuration, UserManager<User> userManager)
        {
            _authService = authService;
            _logger = logger;
            _configuration = configuration;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> RefreshToken(GetRefreshTokenRequestModel model)
        {
            try
            {
                if (model is null)
                {
                    return BadRequest("Invalid client request");
                }

                var result = await _authService.GetRefreshToken(model);
                if (result.StatusCode == 0)
                    return BadRequest(result.StatusMessage);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpPost]
        [Route("revoke/{username}")]
        public async Task<IActionResult> Revoke(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return BadRequest("Invalid user name");

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);

            return Ok("Success");
        }

        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpPost]
        [Route("revoke-all")]
        public async Task<IActionResult> RevokeAll()
        {
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                user.RefreshToken = null;
                await _userManager.UpdateAsync(user);
            }
            return Ok("Success");
        }
    }
}
