using FluentValidation;
using ToDoListApp.Contracts.Responses;

namespace ToDoListApp.Validators
{
    public class TasksResponseModelValidator : AbstractValidator<TasksResponseModel>
    {
        public TasksResponseModelValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .NotNull()
                .WithMessage("Task ID is required.");

            RuleFor(x => x.Title)
                .NotEmpty()
                .NotNull()
                .WithMessage("Title is required.")
                .Length(5, 50)
                .WithMessage("Title should be between 5 and 50 characters long.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .NotNull()
                .WithMessage("Description is required.")
                .MaximumLength(500)
                .WithMessage("Description can have a maximum of 500 characters.");

            RuleFor(x => x.Deadline)
                .NotEmpty()
                .NotNull()
                .WithMessage("Deadline is required.")
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Deadline should be a future date and time.");

            RuleFor(x => x.Priority)
                .NotNull()
                .IsInEnum()
                .WithMessage("Invalid task priority.");

            RuleFor(x => x.StatusTask)
                .IsInEnum()
                .WithMessage("Invalid task status.");

            RuleFor(x => x.AcceptanceDate)
                .LessThan(DateTime.UtcNow)
                .WithMessage("Acceptance date should be a past date and time.")
                .Must(BeBusinessDay)
                .WithMessage("Acceptance date should be a business day.");
        }

        private bool BeBusinessDay(DateTime? acceptanceDate)
        {
            if (acceptanceDate.HasValue)
            {
                // Check if the acceptance date falls on a weekend (Saturday or Sunday)
                return acceptanceDate.Value.DayOfWeek != DayOfWeek.Saturday && acceptanceDate.Value.DayOfWeek != DayOfWeek.Sunday;
            }

            return true; // Acceptance date can be null
        }
    }
}
