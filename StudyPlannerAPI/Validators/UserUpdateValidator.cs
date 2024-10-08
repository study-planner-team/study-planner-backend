using FluentValidation;
using StudyPlannerAPI.Models.Users;

namespace StudyPlannerAPI.Validators
{
    public class UserUpdateValidator : AbstractValidator<UserUpdateDTO>
    {
        public UserUpdateValidator()
        {
            RuleFor(u => u.Username)
                .NotEmpty()
                .MinimumLength(3)
                .WithMessage("Minimalna długość nazwy użytkownika to 3 znaki");

            RuleFor(u => u.Email)
               .NotEmpty()
               .WithMessage("Adres email nie może być pusty")
               .EmailAddress()
               .WithMessage("Nieprawidłowy format adresu email");

            RuleFor(u => u.IsPublic)
                .NotNull()
                .WithMessage("Pole IsPublic nie może być puste");
        }
    }
}
