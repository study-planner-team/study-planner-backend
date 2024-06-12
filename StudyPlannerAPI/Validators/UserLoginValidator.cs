using FluentValidation;
using StudyPlannerAPI.Models.DTO;

namespace StudyPlannerAPI.Validators
{
    public class UserLoginValidator : AbstractValidator<UserLoginDTO>
    {
        public UserLoginValidator()
        {
            RuleFor(u => u.Username)
                .NotEmpty()
                .WithMessage("Nazwa użytkownika nie może być pusta");

            RuleFor(u => u.Password)
                .NotEmpty()
                .WithMessage("Hasło nie może być puste");
        }
    }
}
