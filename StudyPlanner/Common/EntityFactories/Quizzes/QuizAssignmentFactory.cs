using StudyPlannerAPI.Models.Quizes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlannerTests.Common.EntityFactories.Quizzes
{
    public static class QuizAssignmentFactory
    {
        public static QuizAssignment CreateAssignment(int assignmentId, int quizId, int assignedToUserId, QuizState state = QuizState.Assigned)
        {
            return new QuizAssignment
            {
                AssignmentId = assignmentId,
                QuizId = quizId,
                AssignedToUserId = assignedToUserId,
                State = state,
                AssignedOn = DateTime.UtcNow
            };
        }
    }

}
