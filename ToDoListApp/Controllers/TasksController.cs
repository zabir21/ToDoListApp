using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.DAL.DbContext;
using ToDoListApp.Contracts.Requests;
using ToDoListApp.DAL.Entity;
using ToDoListApp.Enum;

namespace ToDoListApp.Controllers
{
    //[Authorize]
    [Route("api/tasks")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly UsersDbContext _context;

        public TasksController(UsersDbContext context) 
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserTask>>> GetAllTask()
        {
            if (_context == null)
            {
                return NotFound();
            }

            var taskDay = await _context.Tasks
                .ToListAsync();

            return Ok(taskDay);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserTask>> GetTask(Guid id)
        {
            if (_context == null)
            {
                return NotFound();
            }

            var taskbyId = await _context.Tasks
                .AsNoTracking()
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Description,
                    p.Deadline,
                    p.Priority,
                    UserName = p.Users.Id
                })
                .SingleOrDefaultAsync(x => x.Id == id);

            if (taskbyId == null)
            {
                return NotFound();
            }

            return Ok(taskbyId);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TaskRequestModel>> PutTask(Guid id, TaskRequestModel taskDto)
        {
            var task = _context.Tasks.First(x => x.Id == id);

            if (!string.IsNullOrEmpty(taskDto.Title))
            {
                task.Title = taskDto.Title;
            }

            if (taskDto.Description != null)
            {
                task.Description = taskDto.Description;
            }

            //if (taskDto.UserId != null)
            //{
            //    product.UserId = taskDto.UserId;
            //}
            
            if (taskDto.Deadline != null)
            {
                task.Deadline = taskDto.Deadline;
            }

            if (taskDto.Priority != null)
            {
                task.Priority = taskDto.Priority;
            }

            await _context.SaveChangesAsync();

            return Ok(task);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskRequestModel>> PostTask(TaskRequestModel taskRequest)
        {
            if (await _context.Tasks.FirstOrDefaultAsync(u => u.Title.ToLower() == taskRequest.Title.ToLower()) != null)
            {
                ModelState.AddModelError("TaskTitleExistsException", $"Task with title \"{taskRequest.Title}\" alredy exists!");
                return BadRequest(ModelState);
            }

            if (taskRequest == null)
            {
                return BadRequest("Task is not set");
            }

            if(taskRequest.UserId == Guid.Empty)
            {
                return BadRequest("User is required");
            }

            /*if (taskPayload.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }*/

            UserTask model = new()
            {
                Id = Guid.NewGuid(),
                Title = taskRequest.Title,
                Description = taskRequest.Description,
                Deadline = taskRequest.Deadline,
                Priority = taskRequest.Priority,
                UserId = taskRequest.UserId,
            };

            await _context.Tasks.AddAsync(model);
            await _context.SaveChangesAsync();


            return CreatedAtRoute("Задача создана" , taskRequest);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            var taskToDelete = await _context.Tasks
            .SingleOrDefaultAsync(_ => _.Id == id);

            if (taskToDelete == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(taskToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }       
    }
}
