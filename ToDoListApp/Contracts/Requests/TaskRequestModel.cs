using ToDoListApp.Enum;

namespace ToDoListApp.Contracts.Requests
{
    public class TaskRequestModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public Priority Priority { get; set; }

        public Guid UserId { get; set; }
    }
}
