using StudyPlannerAPI.Models.Quizes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlannerTests.Common.EntityFactories.Quizzes
{
    public static class QuestionOptionFactory
    {
        public static QuestionOption CreateOption(int optionId, int questionId, string optionText, bool isCorrect)
        {
            return new QuestionOption
            {
                OptionId = optionId,
                QuestionId = questionId,
                OptionText = optionText,
                IsCorrect = isCorrect
            };
        }
    }

}
