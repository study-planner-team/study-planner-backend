using FluentValidation;
using StudyPlannerAPI.Models.Users;

namespace StudyPlannerAPI.Validators.UserValidators
{
    public class UserUpdateValidator : AbstractValidator<UserUpdateDTO>
    {
        public UserUpdateValidator()
        {
            RuleFor(u => u.Username)
                .NotEmpty()
                .MinimumLength(3)
                .WithMessage("The username must be at least 3 characters long.");

            RuleFor(u => u.Email)
               .NotEmpty()
               .WithMessage("The email address cannot be empty.")
               .EmailAddress()
               .WithMessage("The email address format is invalid.");

            RuleFor(u => u.IsPublic)
                .NotNull()
                .WithMessage("The IsPublic field cannot be null.");
        }
    }
}
