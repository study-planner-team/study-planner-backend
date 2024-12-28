using FluentValidation;
using StudyPlannerAPI.Models.Users;

namespace StudyPlannerAPI.Validators.UserValidators
{
    public class UserLoginValidator : AbstractValidator<UserLoginDTO>
    {
        public UserLoginValidator()
        {
            RuleFor(u => u.Username)
                .NotEmpty()
                .WithMessage("The username cannot be empty.");

            RuleFor(u => u.Password)
                .NotEmpty()
                .WithMessage("The password cannot be empty.");
        }
    }
}
