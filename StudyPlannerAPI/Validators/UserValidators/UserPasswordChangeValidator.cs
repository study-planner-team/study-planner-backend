using FluentValidation;
using StudyPlannerAPI.Models.Users;

namespace StudyPlannerAPI.Validators.UserValidators
{
    public class UserPasswordChangeValidator : AbstractValidator<UserPasswordChangeDTO>
    {
        public UserPasswordChangeValidator()
        {
            RuleFor(p => p.OldPassword)
                .NotEmpty()
                .WithMessage("Old password is required.");

            RuleFor(p => p.NewPassword)
                .NotEmpty()
                .WithMessage("The password cannot be empty.")
                .MinimumLength(8)
                .WithMessage("The password must be at least 8 characters long.")
                .Must(ContainCapitalLetter)
                .WithMessage("The password must contain at least one uppercase letter.")
                .Must((dto, newPassword) => !newPassword.Equals(dto.OldPassword))
                .WithMessage("The new password cannot be the same as the old password.");
        }

        private bool ContainCapitalLetter(string password)
        {
            return password.Any(char.IsUpper);
        }
    }
}
