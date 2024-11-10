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
                .WithMessage("Minimalna długość nazwy użytkownika to 3 znaki");

            RuleFor(u => u.Password)
                .NotEmpty()
                .MinimumLength(8)
                .WithMessage("Minimalna długość hasła to 8 znaków")
                .Must(ContainCapitalLetter)
                .WithMessage("Hasło musi zawierać przynajmniej jedną wielką literę");

            RuleFor(u => u.Email)
               .NotEmpty()
               .WithMessage("Adres email nie może być pusty")
               .EmailAddress()
               .WithMessage("Nieprawidłowy format adresu email");
        }

        private bool ContainCapitalLetter(string password)
        {
            return password.Any(char.IsUpper);
        }
    }
}
