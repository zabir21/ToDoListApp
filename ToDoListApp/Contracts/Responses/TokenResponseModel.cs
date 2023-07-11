namespace ToDoListApp.Contracts.Responses
{
    public class TokenResponseModel
    {
        public int StatusCode { get; set; }
        public string? StatusMessage { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
