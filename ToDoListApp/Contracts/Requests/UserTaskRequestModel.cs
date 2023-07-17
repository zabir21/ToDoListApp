
namespace ToDoListApp.Contracts.Requests
{
    public class UserTaskRequestModel
    {
        public Enums.TaskStatus StatusTask { get; set; }

        public DateTime AcceptanceDate { get; set; }
    }
}
