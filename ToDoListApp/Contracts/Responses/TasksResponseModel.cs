using ToDoListApp.Enums;

namespace ToDoListApp.Contracts.Responses
{
    public class TasksResponseModel
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime Deadline { get; set; }
        public Priority Priority { get; set; }
        public Enums.TaskStatus? StatusTask { get; set; }

        public DateTime? AcceptanceDate { get; set; }
    }
}