using System.ComponentModel.DataAnnotations;

namespace ToDoListApp.Contracts.Requests
{
    public class RegistrationRequestModel
    {
        [Required]
        public string? Username { get; set; }

        [EmailAddress]
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
