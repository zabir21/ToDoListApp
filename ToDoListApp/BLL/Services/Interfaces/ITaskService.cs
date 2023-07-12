using ToDoListApp.BLL.Models;
using ToDoListApp.BLL.Models.Dto;

namespace ToDoListApp.BLL.Services.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskDto>> GetAll(Guid? userId = null);

        Task<TaskDto> GetById(Guid id);

        Task<TaskDto> Update(UpdateTaskModel model);

        Task DeleteById(Guid id);

        Task<TaskDto> Create(CreateTaskModel model);
    }
}
