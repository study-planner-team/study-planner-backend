using FluentValidation;
using StudyPlannerAPI.Models.Users;

namespace StudyPlannerAPI.Validators.UserValidators
{
    public class UserRegistrationValidator : AbstractValidator<UserRegistrationDTO>
    {
        public UserRegistrationValidator()
        {
            RuleFor(u => u.Username)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(30)
                .WithMessage("The username must be at least 3 characters long.");

            RuleFor(u => u.Password)
                .NotEmpty()
                .MinimumLength(8)
                .WithMessage("The password must be at least 8 characters long.")
                .Must(ContainCapitalLetter)
                .WithMessage("The password must contain at least one uppercase letter.");

            RuleFor(u => u.Email)
               .NotEmpty()
               .WithMessage("The email address cannot be empty.")
               .EmailAddress()
               .WithMessage("The email address format is invalid.");
        }

        private bool ContainCapitalLetter(string password)
        {
            return password.Any(char.IsUpper);
        }
    }
}
