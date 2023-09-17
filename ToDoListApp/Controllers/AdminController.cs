using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using ToDoListApp.BLL.Exceptions;
using ToDoListApp.BLL.Models;
using ToDoListApp.BLL.Models.Dto;
using ToDoListApp.BLL.Services;
using ToDoListApp.BLL.Services.Interfaces;
using ToDoListApp.Contracts.Requests;
using ToDoListApp.Contracts.Responses;
using ToDoListApp.Contracts.Responses.Base;
using ToDoListApp.Enums;

namespace ToDoListApp.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    [Route("api/admin")]
    [ApiController]
    public class AdminController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly ITaskService _taskService;

        public AdminController(IUserService userService, ITaskService taskService) 
  
        {
            _userService = userService;
            _taskService = taskService;
        }

        [HttpPost("roles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult>> AddRoles(Guid id, Roles roles)
        {
            if (!ModelState.IsValid)
                return BadRequest("Failed");

            try
            {
                await _userService.AddRoleToUser(id, roles);

                return Ok();
            }
            catch (UserNotFoundException)
            {
                return BadRequest("User not found");
            }
            catch(RoleNotExistException)
            {
                return BadRequest("Role does not exist");
            }
            catch (RoleNotAddedToUserException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("tasks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TasksResponseModel>>> GetAllTasks()
        {
            var tasks = await _taskService.GetAll();

            return Ok(tasks.Select(x => new TasksResponseModel
            {
                Id = x.Id,
                Title = x.Title,
                Deadline = x.Deadline,
                Description = x.Description,
                Priority = x.Priority
            }).ToList());
        }

        [HttpGet("tasks/{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TasksResponseModel>> GetTaskById(Guid id)
        {
            TaskDto task;
            try
            {
                task = await _taskService.GetById(id);

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

        [HttpPut("tasks/{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TasksResponseModel>> PutUserTask(Guid id, [FromBody] AdminTaskRequestModel request)
        {
            try
            {
              var taskDto = await _taskService.Update(new UpdateTaskModel
                {
                    Id = id,
                    Title = request.Title,
                    Deadline = request.Deadline,
                    Description = request.Description,
                    Priority = request.Priority,
                    UserId = request.UserId
                });

                return Ok(new TasksResponseModel
                {
                    Id = taskDto.Id,
                    Title = taskDto.Title,
                    Deadline = taskDto.Deadline,
                    Description = taskDto.Description,
                    Priority = taskDto.Priority
                });
            }
            catch (TaskNotFoundException)
            {
                return NotFound("Task not found");
            }
        }

        [HttpPost("tasks")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<TasksResponseModel>> PostTask([FromBody] AdminTaskRequestModel request)
        {
            var task = await _taskService.Create(new CreateTaskModel
            {
                Title = request.Title,
                Description = request.Description,
                Deadline = request.Deadline,
                Priority = request.Priority,
                UserId = request.UserId
            });

            return CreatedAtAction(nameof(GetAllTasks), new TasksResponseModel
            {
                Id = task.Id,
                Title = task.Title,
                Deadline = task.Deadline,
                Description = task.Description,
                Priority = task.Priority
            });
        }

        [HttpDelete("tasks/{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTask([FromRoute] Guid id)
        {
            try
            {
                await _taskService.DeleteById(id);

                return Ok();
            }
            catch (TaskNotFoundException)
            {
                return NotFound("Task not found");
            }
        }
    }
}
