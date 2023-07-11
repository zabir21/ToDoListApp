namespace ToDoListApp.Contracts.Requests
{
    public class AddRolesRequestModel
    {
        public Guid Id { get; set; }
        public string Role { get; set; }

        public int  Status { get; set; }
        public string Message { get; set; }

    }
}
