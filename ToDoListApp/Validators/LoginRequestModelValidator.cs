using FluentValidation;
using ToDoListApp.Contracts.Requests;

namespace ToDoListApp.Validators
{
    public class LoginRequestModelValidator : AbstractValidator<LoginRequestModel>
    {
        public LoginRequestModelValidator()
        {
            RuleFor(x => x.Username)
            .NotEmpty()
            .NotNull()
            .MinimumLength(4)
            .MaximumLength(20)
            .Matches("^[a-zA-Z0-9]+$")
            .WithMessage("Username can only contain letters and digits.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .NotNull()
                .MinimumLength(6)
                .MaximumLength(20)
                .Matches("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9]).{8,}$")
                .WithMessage("Password should contain at least one uppercase letter, one lowercase letter, one digit, and have a minimum length of 6.");
        }
    }
}
