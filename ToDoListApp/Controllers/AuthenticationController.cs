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
    public class AuthenticationController : BaseApiController
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
                return BadRequest("Failed");

            try
            {
                var result = await _authService.Registeration(model, Roles.User);

                return Ok(result);
            }
            catch (UserNameAlreadyExistsException)
            {
                return BadRequest(ErrorCode.UserNameAlredyExist, "User name exists");
            }
            catch (EmailAlreadyException)
            {
                return BadRequest(ErrorCode.EmailAlreadyExist, "User with this email already exists");
            }
            catch (UserNotCreatedException)
            {
                return BadRequest(ErrorCode.UserNotCreated, "Can't create user");
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResult<TokenResponseModel>>> Login(LoginRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Failed");

            try
            {
                var result = await _authService.Login(model);

                return Ok(result);
            }
            catch (UserNotFoundException)
            {
                return BadRequest(ErrorCode.UserNotFound, "User not found");
            }
            catch (InvalidPasswordException)
            {
                return BadRequest(ErrorCode.InvalidPasword, "Invalid password");
            }         
        }
    }
}
