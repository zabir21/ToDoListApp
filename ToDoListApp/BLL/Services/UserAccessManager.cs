using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ToDoList.DAL.DbContext;
using ToDoListApp.DAL.Entity.Identity;

namespace ToDoListApp.BLL.Services
{
    public class UserAccessManager
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly UsersDbContext _usersDbContext;

        public UserAccessManager(IHttpContextAccessor httpContext, UsersDbContext usersDbContext)
        {
            _httpContext = httpContext;
            _usersDbContext = usersDbContext;
        }

        public Guid? GetUserId()
        {
            var userId = _httpContext.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(userId, out Guid id)
                ? id
                : null;
        }

        public User? GetCurrentUser()
        {
            var userId = _httpContext.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(userId, out Guid id)
                ? _usersDbContext.Users.FirstOrDefault(x => x.Id == id)
                : null;
        }
    }
}
