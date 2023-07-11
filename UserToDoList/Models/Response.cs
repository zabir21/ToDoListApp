namespace ToDoListApp.Models
{
    public class Response
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
