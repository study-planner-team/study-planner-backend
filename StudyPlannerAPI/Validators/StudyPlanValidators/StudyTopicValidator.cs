using FluentValidation;
using StudyPlannerAPI.Models.StudyTopics;

namespace StudyPlannerAPI.Validators.StudyPlanValidators
{
    public class StudyTopicValidator : AbstractValidator<StudyTopicDTO>
    {
        public StudyTopicValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required.");

            RuleFor(x => x.Hours)
                .GreaterThan(0)
                .WithMessage("Hours must be greater than zero.");
        }
    }
}