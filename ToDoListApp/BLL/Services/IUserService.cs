using ToDoListApp.Contracts.Requests;
using ToDoListApp.Enum;

namespace ToDoListApp.BLL.Services
{
    public interface IUserService
    {
        Task<AddRolesRequestModel> AddRoleToUser(Guid id, Roles roles);
    }
}