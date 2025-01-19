using StudyPlannerAPI.Models.Quizes;

namespace StudyPlannerTests.Common.EntityFactories.Quizzes
{
    public static class QuestionFactory
    {
        public static Question CreateQuestion(int questionId, int quizId, string questionText, bool includeOptions = true)
        {
            return new Question
            {
                QuestionId = questionId,
                QuizId = quizId,
                QuestionText = questionText,
                Options = includeOptions ? new List<QuestionOption>
            {
                QuestionOptionFactory.CreateOption(questionId * 10 + 1, questionId, "Option 1", true),
                QuestionOptionFactory.CreateOption(questionId * 10 + 2, questionId, "Option 2", false)
            } : new List<QuestionOption>()
            };
        }
    }

}
