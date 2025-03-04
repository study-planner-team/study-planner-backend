﻿using StudyPlannerAPI.Models.Quizes.RequestDTOs;
using StudyPlannerAPI.Models.Quizes;
using StudyPlannerAPI.Models.Quizes.ResponseDTOs;

namespace StudyPlannerAPI.Services.QuizService
{
    public interface IQuizService
    {
        public Task<QuizResponseDTO> CreateQuizWithQuestions(int studyPlanId, QuizRequestDTO quizDto, int userId);
        public Task<QuizResponseDTO?> GetQuizById(int quizId);
        public Task<IEnumerable<QuizResponseDTO>> GetQuizzesForStudyPlan(int studyPlanId);
        public Task<bool> DeleteQuiz(int quizId);
        public Task<bool> AssignQuizToUser(int quizId, int userId);
        public Task<IEnumerable<QuizAssignmentResponseDTO>> GetAssignedQuizzes(int userId, int studyPlanId);
        public Task<QuizAssignmentResponseDTO?> GetAssignedQuizById(int userId, int quizId);
        public Task<IEnumerable<QuizResponseDTO>> GetCreatedQuizzes(int userId, int studyPlanId);
        public Task<QuizAssignmentResponseDTO?> UpdateQuizScore(int assignmentId, ICollection<UserAnswerDTO> userAnswers);
        public Task<IEnumerable<QuizAssignmentResponseDTO>> GetCompletedQuizzes(int userId, int studyPlanId);
    }
}
