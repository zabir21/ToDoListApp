using ToDoListApp.Contracts.Requests;
using ToDoListApp.Enums;

namespace ToDoListApp.BLL.Services.Interfaces
{
    public interface IUserService
    {
        Task AddRoleToUser(Guid id, Roles roles);
    }
}