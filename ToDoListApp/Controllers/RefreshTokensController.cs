using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDoListApp.BLL.Exceptions;
using ToDoListApp.BLL.Models.Dto;
using ToDoListApp.BLL.Services;
using ToDoListApp.BLL.Services.Interfaces;
using ToDoListApp.Contracts.Requests;
using ToDoListApp.Contracts.Responses;
using ToDoListApp.Contracts.Responses.Base;
using ToDoListApp.DAL.Entity.Identity;
using ToDoListApp.Enums;

namespace ToDoListApp.Controllers
{
    [Route("api/token")]
    [ApiController]
    public class RefreshTokensController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<User> _userManager;

        public RefreshTokensController(IAuthService authService, UserManager<User> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<TokenResponseModel>>> RefreshToken(GetRefreshTokenRequestModel model)
        {
            if (model is null)
            {
                return BadRequest(ApiResult.BadRequest("Invalid client request"));
            }

            try
            {
                var result = await _authService.GetRefreshToken(model);

                return Ok(ApiResult<TokenResponseModel>.Succces(result));
            }
            catch (AccessTokenInvalidException)
            {
                return BadRequest(ApiResult.BadRequest("Invalid access token or refresh token"));
            }
            catch(InvalidPasswordException)
            {
                return BadRequest(ApiResult.BadRequest("Invalid password"));
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

                return Ok(ApiResult.Succces());
            }
            catch (InvalidUserNameException)
            {
                return BadRequest(ApiResult.BadRequest("Invalid user name"));
            }
        }

        [Authorize(Roles = nameof(Roles.Admin))]
        [HttpPost]
        [Route("revoke-all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResult>> RevokeAll()
        {
            await _authService.DeleteTokenAll();

            return Ok(ApiResult.Succces());
        }
    }
}
