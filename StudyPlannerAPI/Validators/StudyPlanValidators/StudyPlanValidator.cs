using FluentValidation;
using StudyPlannerAPI.Models.StudyPlans;

namespace StudyPlannerAPI.Validators.StudyPlanValidators
{
    public class StudyPlanValidator : AbstractValidator<StudyPlanDTO>
    {
        public StudyPlanValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.");

            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("Start date is required.");

            RuleFor(x => x.EndDate)
                .NotEmpty()
                .WithMessage("End date is required.")
                .GreaterThan(x => x.StartDate)
                .WithMessage("End date must be later than start date.");
        }
    }
}