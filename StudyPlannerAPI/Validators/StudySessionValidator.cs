using FluentValidation;
using StudyPlannerAPI.Models.StudySessions;
using StudyPlannerAPI.Validators.StudyPlanValidators;

namespace StudyPlannerAPI.Validators
{
    public class StudySessionValidator : AbstractValidator<StudySessionDTO>
    {
        public StudySessionValidator()
        {
            RuleFor(x => x.StartDate)
                .LessThan(x => x.EndDate)
                .WithMessage("Start date must be before end date.");

            RuleFor(x => x.SessionsPerDay)
                .GreaterThan(0)
                .WithMessage("Sessions per day must be greater than zero.");

            RuleFor(x => x.SessionLength)
                .GreaterThan(0)
                .WithMessage("Session length must be greater than zero.");

            RuleFor(x => x.StudyStartTime)
                .LessThan(x => x.StudyEndTime)
                .WithMessage("Study start time must be before study end time.");

            RuleFor(x => x.PreferredStudyDays)
                .NotEmpty()
                .WithMessage("At least one preferred study day must be selected.");

        }
    }
}
