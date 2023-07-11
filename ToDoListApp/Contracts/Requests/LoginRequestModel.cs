using System.ComponentModel.DataAnnotations;

namespace ToDoListApp.Contracts.Requests
{
    public class LoginRequestModel
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
