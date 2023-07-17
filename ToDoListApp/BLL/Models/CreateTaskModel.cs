using ToDoListApp.Enums;

namespace ToDoListApp.BLL.Models
{
    public class CreateTaskModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public Priority Priority { get; set; }

        public Guid UserId { get; set; }
    }
}
