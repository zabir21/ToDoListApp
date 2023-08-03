using FluentValidation;
using ToDoListApp.Contracts.Requests;

namespace ToDoListApp.Validators
{
    public class GetRefreshTokenRequestModelValidator : AbstractValidator<GetRefreshTokenRequestModel>
    {
        public GetRefreshTokenRequestModelValidator()
        {
            RuleFor(x => x.AccessToken)
                .NotEmpty()
                .NotNull()
                .Length(100)
                .WithMessage("Access token should be exactly 100 characters long.");

            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .NotNull()
                .Length(200)
                .WithMessage("Refresh token should be exactly 200 characters long.")
                .Matches("^[a-zA-Z0-9]+$")
                .WithMessage("Refresh token can only contain letters and digits.");
        }
    }
}
