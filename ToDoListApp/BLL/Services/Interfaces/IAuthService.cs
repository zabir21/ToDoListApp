using ToDoListApp.BLL.Models.Dto;
using ToDoListApp.Contracts.Requests;
using ToDoListApp.Contracts.Responses;
using ToDoListApp.Enums;

namespace ToDoListApp.BLL.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserDto> Registeration(RegistrationRequestModel model, Roles roles);
        Task<TokenResponseModel> Login(LoginRequestModel model);
        Task<TokenResponseModel> GetRefreshToken(GetRefreshTokenRequestModel model);
        Task DeleteToken(string username);
        Task DeleteTokenAll();
    }
}
