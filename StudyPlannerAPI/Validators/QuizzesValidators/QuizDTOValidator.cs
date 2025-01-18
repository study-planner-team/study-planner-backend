using FluentValidation;
using StudyPlannerAPI.Models.Quizes.RequestDTOs;

namespace StudyPlannerAPI.Validators.QuizzesValidators
{
    public class QuizDTOValidator : AbstractValidator<QuizRequestDTO>
    {
        public QuizDTOValidator()
        {
            RuleFor(q => q.Title)
                .NotEmpty()
                .WithMessage("Quiz title cannot be empty.")
                .MinimumLength(5)
                .WithMessage("Quiz title must be at least 5 characters long.");

            RuleFor(q => q.Description)
                .MaximumLength(500)
                .WithMessage("Description cannot exceed 500 characters.");

            RuleForEach(q => q.Questions).ChildRules(questions =>
            {
                questions.RuleFor(q => q.QuestionText)
                    .NotEmpty()
                    .WithMessage("Question text cannot be empty.");

                questions.RuleFor(q => q.Options)
                    .Must(options => options != null && options.Any())
                    .WithMessage("Each question must have at least one option.");

                questions.RuleFor(q => q.Options)
                    .Must(options => options.Any(o => o.IsCorrect))
                    .WithMessage("Each question must have at least one correct option.");

                questions.RuleForEach(q => q.Options).ChildRules(options =>
                {
                    options.RuleFor(o => o.OptionText)
                        .NotEmpty()
                        .WithMessage("Option text cannot be empty.");
                });
            });
        }
    }
}
