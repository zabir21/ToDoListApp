using Microsoft.AspNetCore.Mvc;
using ToDoListApp.BLL.Exceptions;
using ToDoListApp.BLL.Models.Dto;
using ToDoListApp.BLL.Services.Interfaces;
using ToDoListApp.Contracts.Requests;
using ToDoListApp.Contracts.Responses;
using ToDoListApp.Contracts.Responses.Base;
using ToDoListApp.Enums;

namespace ToDoListApp.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("registeration")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<UserDto>>> Register(RegistrationRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResult.BadRequest("Failed"));

            try
            {
                var result = await _authService.Registeration(model, Roles.User);

                return Ok(ApiResult<UserDto>.Succces(result));
            }
            catch (UserNameAlreadyExistsException)
            {
                return BadRequest(ApiResult.BadRequest("User name exists"));
            }
            catch (EmailAlreadyException)
            {
                return BadRequest(ApiResult.BadRequest("User with this email already exists"));
            }
            catch (UserNotCreatedException)
            {
                return BadRequest(ApiResult.BadRequest("Can't create user"));
            }
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<TokenResponseModel>>> Login(LoginRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResult.BadRequest("Failed"));

            try
            {
                var result = await _authService.Login(model);

                return Ok(ApiResult<TokenResponseModel>.Succces(result));
            }
            catch (UserNotFoundException)
            {
                return BadRequest(ApiResult.BadRequest("User not found"));
            }
            catch (InvalidPasswordException)
            {
                return BadRequest(ApiResult.BadRequest("Invalid password"));
            }         
        }
    }
}
