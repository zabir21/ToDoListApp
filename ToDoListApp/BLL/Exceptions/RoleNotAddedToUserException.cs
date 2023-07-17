namespace ToDoListApp.BLL.Exceptions
{
    public class RoleNotAddedToUserException: Exception
    {
        public RoleNotAddedToUserException(string message): base(message)
        {
            
        }

        public RoleNotAddedToUserException()
        {
            
        }
    }
}
