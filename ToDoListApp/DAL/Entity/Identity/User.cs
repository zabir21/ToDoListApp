using Microsoft.AspNetCore.Identity;

namespace ToDoListApp.DAL.Entity.Identity
{
    public class User : IdentityUser<Guid>
    {
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public List<UserTask> Tasks { get; set; } = new List<UserTask>();
    }
}
