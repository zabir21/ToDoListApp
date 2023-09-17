using FluentValidation;
using ToDoListApp.Contracts.Responses;

namespace ToDoListApp.Validators
{
    public class TokenResponseModelValidator : AbstractValidator<TokenResponseModel>
    {
        public TokenResponseModelValidator() 
        {
            RuleFor(x => x.AccessToken)
                .NotEmpty()
                .NotNull()
                .WithMessage("Access token is required.");

            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .NotNull()
                .WithMessage("Refresh token is required.");
        }
    }
}
