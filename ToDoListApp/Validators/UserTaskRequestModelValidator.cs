using FluentValidation;
using ToDoListApp.Contracts.Requests;

namespace ToDoListApp.Validators
{
    public class UserTaskRequestModelValidator : AbstractValidator<UserTaskRequestModel>
    {
        public UserTaskRequestModelValidator() 
        {
            RuleFor(x => x.StatusTask)
                .NotNull()
                .IsInEnum()
                .WithMessage("Invalid task status.");

            RuleFor(x => x.AcceptanceDate)
                .NotEmpty()
                .NotNull()
                .LessThan(DateTime.UtcNow)
                .WithMessage("Acceptance date should be a past date and time.");

            //.GreaterThan(DateTime.UtcNow.AddDays(-7))
            //.WithMessage("Acceptance date should not be more than 7 days in the past.")
            //.Must(BeBusinessDay)
            //.WithMessage("Acceptance date should be a business day.");
        }
        //private bool BeBusinessDay(DateTime acceptanceDate)
        //{
        //    // Check if the acceptance date falls on a weekend (Saturday or Sunday)
        //    return acceptanceDate.DayOfWeek != DayOfWeek.Saturday && acceptanceDate.DayOfWeek != DayOfWeek.Sunday;
        //}
    }
}
