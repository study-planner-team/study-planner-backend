using FluentValidation;
using StudyPlannerAPI.Models.StudyMaterials;

namespace StudyPlannerAPI.Validators.StudyMaterialValidators
{
    public class StudyMaterialDTOValidator : AbstractValidator<StudyMaterialDTO>
    {
        public StudyMaterialDTOValidator() 
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MaximumLength(80)
                .WithMessage("Title must not exceed 80 characters.");

            RuleFor(x => x.Link)
                .NotEmpty()
                .WithMessage("Link is required.")
                .Must(link => Uri.IsWellFormedUriString(link, UriKind.Absolute))
                .WithMessage("Invalid URL format.");
        }
    }
}
