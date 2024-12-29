using StudyPlannerAPI.Models.Quizes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlannerTests.Common.EntityFactories.Quizzes
{
    public static class QuizFactory
    {
        public static Quiz CreateQuiz(int quizId, int studyPlanId, int createdByUserId, string title, string? description = null, bool includeQuestions = true)
        {
            return new Quiz
            {
                QuizId = quizId,
                StudyPlanId = studyPlanId,
                CreatedByUserId = createdByUserId,
                Title = title,
                Description = description,
                Questions = includeQuestions ? new List<Question>
            {
                QuestionFactory.CreateQuestion(quizId * 10 + 1, quizId, "What is AI?", true),
                QuestionFactory.CreateQuestion(quizId * 10 + 2, quizId, "What is ML?")
            } : new List<Question>()
            };
        }
    }
}
