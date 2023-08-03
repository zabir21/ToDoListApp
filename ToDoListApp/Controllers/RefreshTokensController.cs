using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoListApp.BLL.Exceptions;
using ToDoListApp.BLL.Services.Interfaces;
using ToDoListApp.Contracts.Requests;
using ToDoListApp.Contracts.Responses;
using ToDoListApp.Contracts.Responses.Base;
using ToDoListApp.Enums;

namespace ToDoListApp.Controllers
{
    [Route("api/token")]
    [ApiController]
    public class RefreshTokensController : BaseApiController
    {
        private readonly IAuthService _authService;

        public RefreshTokensController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<TokenResponseModel>>> RefreshToken(GetRefreshTokenRequestModel model)
        {
            if (model is null)
            {
                return BadRequest("Invalid client request");
            }

            try
            {
                var result = await _authService.GetRefreshToken(model);

                return Ok(result);
            }
            catch (AccessTokenInvalidException)
            {
                return BadRequest("Invalid access token or refresh token");
            }
            catch(InvalidPasswordException)
            {
                return BadRequest("Invalid password");
            }
        }

        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpPost]
        [Route("revoke/{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult>> Revoke(string username)
        {
            try
            {
                await _authService.DeleteToken(username);

                return Ok();
            }
            catch (InvalidUserNameException)
            {
                return BadRequest("Invalid user name");
            }
        }

        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpPost]
        [Route("revoke-all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResult>> RevokeAll()
        {
            await _authService.DeleteTokenAll();

            return Ok();
        }
    }
}
