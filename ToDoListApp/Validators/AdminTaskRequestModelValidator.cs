using FluentValidation;
using ToDoListApp.Contracts.Requests;

namespace ToDoListApp.Validators
{
    public class AdminTaskRequestModelValidator : AbstractValidator<AdminTaskRequestModel>
    {
        public AdminTaskRequestModelValidator() 
        {
            RuleFor(x => x.Title)
           .NotEmpty()
           .NotNull()
           .Length(5, 50)
           .WithMessage("Title should be between 5 and 50 characters long.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .NotNull()
                .MaximumLength(500)
                .WithMessage("Description can have a maximum of 500 characters.");

            RuleFor(x => x.Deadline)
                .NotEmpty()
                .NotNull()
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Deadline should be a future date and time.");

            RuleFor(x => x.Priority)
                .NotNull()
                .IsInEnum();

            RuleFor(x => x.UserId)
                .NotEmpty()
                .NotNull()
                .Must(BeValidGuid)
                .WithMessage("UserId must be a valid GUID.");
        }

        private bool BeValidGuid(Guid userId)
        {
            return !(userId == Guid.Empty);
        }
    }
}
