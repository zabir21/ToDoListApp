namespace ToDoListApp.Contracts.Requests
{
    public class GetRefreshTokenRequestModel
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
