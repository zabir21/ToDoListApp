using Microsoft.EntityFrameworkCore;
using ToDoList.DAL.DbContext;
using ToDoListApp.BLL.Exceptions;
using ToDoListApp.BLL.Models;
using ToDoListApp.BLL.Models.Dto;
using ToDoListApp.BLL.Services.Interfaces;
using ToDoListApp.DAL.Entity;

namespace ToDoListApp.BLL.Services
{
    public class TaskService : ITaskService
    {
        private readonly UsersDbContext _context;

        public TaskService(UsersDbContext context)
        {
            _context = context;
        }

        public async Task<TaskDto> Create(CreateTaskModel model)
        {
            var task = new UserTask
            {
                Title = model.Title,
                Description = model.Description,
                Deadline = model.Deadline,
                Priority = model.Priority,
                UserId = model.UserId
            };

            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            return new TaskDto
            {
                Id = task.Id,
                Title = model.Title,
                Description = model.Description,
                Deadline = model.Deadline,
                Priority = model.Priority,
                UserId = model.UserId
            };
        }

        public async Task DeleteById(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                throw new TaskNotFoundException();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TaskDto>> GetAll(Guid? userId = null)
        {
            //если айди задан даст конкретного пользователя если нет то все задачи
            return await _context.Tasks.Where(x => x.UserId == userId || userId == null)
                .Select(x => new TaskDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    Deadline = x.Deadline,
                    Priority = x.Priority,
                    UserId = x.UserId
                }).ToListAsync();
        }

        public async Task<TaskDto> GetById(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                throw new TaskNotFoundException();
            }

            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Deadline = task.Deadline,
                Priority = task.Priority,
                UserId = task.UserId
            };
        }

        public async Task<TaskDto> Update(UpdateTaskModel model)
        {
            var task = await _context.Tasks.FindAsync(model.Id);

            if (task == null)
            {
                throw new TaskNotFoundException();
            }           
            
            task.Title = model.Title ?? task.Title; // ?? если слева null бери правое значение
            task.Description = model.Description ?? task.Description;
            task.Deadline = model.Deadline ?? task.Deadline;
            task.Priority = model.Priority ?? task.Priority;
            task.UserId = model.UserId ?? task.UserId;
            task.StatusTask = model.StatusTask ?? task.StatusTask;
            task.AcceptanceDate = model.AcceptanceDate ?? task.AcceptanceDate;

            await _context.SaveChangesAsync();

            return new TaskDto
            {
                Title = task.Title,
                Description = task.Description,
                Deadline = task.Deadline,
                Priority = task.Priority,
                UserId = task.UserId
            };
           /* try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id)) //одновременное исправление по айди
            {
                тут бросить исключение

            }*/

        }

        //private bool TodoItemExists(Guid id)
        //{
        //    return _context.Tasks.Any(e => e.Id == id);
        //}
    }
}
