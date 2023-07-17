using System.Security.Claims;
using ToDoListApp.DAL.Entity.Identity;

namespace ToDoListApp.BLL.Services
{
    public class UserAccessManager
    {
        private readonly IHttpContextAccessor _httpContext;

        public UserAccessManager(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public Guid? GetUserId()
        {
            var userId = _httpContext.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(userId, out Guid id)
                ? id
                : null;
        }
    }
}
