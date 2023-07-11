using ToDoListApp.Contracts.Requests;
using ToDoListApp.Contracts.Responses;
using ToDoListApp.Enum;

namespace ToDoListApp.BLL.Services
{
    public interface IAuthService
    {
        Task<(int, string)> Registeration(RegistrationRequestModel model, Roles roles);
        Task<TokenResponseModel> Login(LoginRequestModel model);
        Task<TokenResponseModel> GetRefreshToken(GetRefreshTokenRequestModel model);
    }
}
