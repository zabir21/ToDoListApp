using FluentValidation;
using ToDoListApp.Contracts.Requests;

namespace ToDoListApp.Validators
{
    public class RegistrationRequestModelValidator : AbstractValidator<RegistrationRequestModel>
    {
        public RegistrationRequestModelValidator()
        {
            RuleFor(x => x.Username)
                .NotNull()
                .NotEmpty()
                .MinimumLength(4)
                .MaximumLength(20)
                .Matches("^[a-zA-Z0-9]+$")
                .WithMessage("Username can only contain letters and digits.");

            RuleFor(x => x.Password)
                .NotNull()
                .NotEmpty()
                .MinimumLength(6)
                .MaximumLength(20)
                .Matches("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9]).{8,}$")
                .WithMessage("Password should contain at least one uppercase letter, one lowercase letter, one digit, and have a minimum length of 6.");

            RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress();
        }
    }
}
