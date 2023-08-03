using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoListApp.BLL.Exceptions;
using ToDoListApp.BLL.Models;
using ToDoListApp.BLL.Services;
using ToDoListApp.BLL.Services.Interfaces;
using ToDoListApp.Contracts.Requests;
using ToDoListApp.Contracts.Responses;

namespace ToDoListApp.Controllers
{
    [Authorize]
    [Route("api/user-tasks")]
    [ApiController]
    public class UsersTasksController : BaseApiController
    {
        private readonly ITaskService _taskService;
        private readonly UserAccessManager _userAccessManager;

        public UsersTasksController(ITaskService taskService, UserAccessManager userAccessManager)
        {
            _taskService = taskService;
            _userAccessManager = userAccessManager;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TasksResponseModel>>> GetAllTasksUser()
        {
            var userId = _userAccessManager.GetUserId();

            var taskDay = await _taskService.GetAll(userId);

            return Ok(taskDay);
        }

        [HttpGet("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TasksResponseModel>> GetUserTaskById(Guid id)
        {
            try
            {
               var task = await _taskService.GetById(id);

                return Ok(new TasksResponseModel
                {
                    Id = task.Id,
                    Title = task.Title,
                    Deadline = task.Deadline,
                    Description = task.Description,
                    Priority = task.Priority
                });
            }
            catch (TaskNotFoundException)
            {
                return NotFound("Task not found");
            }
        }

        [HttpPut("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TasksResponseModel>> UpdateUserTask(Guid id, [FromBody] UserTaskRequestModel request)
        {          
            try
            {
                var taskDto = await _taskService.Update(new UpdateTaskModel
                {
                    Id = id,
                    StatusTask = request.StatusTask,
                    AcceptanceDate = request.AcceptanceDate
                });

                return Ok(new TasksResponseModel
                {
                    Id = taskDto.Id,
                    Title = taskDto.Title,
                    Deadline = taskDto.Deadline,
                    Description = taskDto.Description,
                    Priority = taskDto.Priority,
                    StatusTask = taskDto.StatusTask,
                    AcceptanceDate = taskDto.AcceptanceDate
                });
            }
            catch (TaskNotFoundException)
            {
                return NotFound("Task not found");
            }
        }      
    }
}
