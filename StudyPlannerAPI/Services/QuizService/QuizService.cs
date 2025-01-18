using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Data;
using StudyPlannerAPI.Models.Quizes;
using StudyPlannerAPI.Models.Quizes.RequestDTOs;
using StudyPlannerAPI.Models.Quizes.ResponseDTOs;

namespace StudyPlannerAPI.Services.QuizService
{
    public class QuizService : IQuizService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public QuizService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<QuizResponseDTO> CreateQuizWithQuestions(int studyPlanId, QuizRequestDTO quizDto, int userId)
        {
            var quiz = _mapper.Map<Quiz>(quizDto);

            quiz.CreatedByUserId = userId;
            quiz.StudyPlanId = studyPlanId;

            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            return _mapper.Map<QuizResponseDTO>(quiz);
        }

        public async Task<QuizResponseDTO?> GetQuizById(int quizId)
        {
            var quiz = await _context.Quizzes
              .Include(q => q.Questions)
              .ThenInclude(q => q.Options)
              .FirstOrDefaultAsync(q => q.QuizId == quizId);

            if (quiz == null)
            {
                return null;
            }

            return _mapper.Map<QuizResponseDTO>(quiz);
        }

        public async Task<bool> DeleteQuiz(int quizId)
        {
            var quiz = await _context.Quizzes
              .Include(q => q.Questions)
              .ThenInclude(q => q.Options)
              .FirstOrDefaultAsync(q => q.QuizId == quizId);

            if (quiz == null)
            {
                return false;
            }

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignQuizToUser(int quizId, int userId)
        {
            var existingAssignment = await _context.QuizAssignments
                .FirstOrDefaultAsync(a => a.QuizId == quizId && a.AssignedToUserId == userId);

            if (existingAssignment != null)
            {
                return false; // Quiz already assigned to this user
            }

            var assignment = new QuizAssignment
            {
                QuizId = quizId,
                AssignedToUserId = userId,
            };

            _context.QuizAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<QuizAssignmentResponseDTO>> GetAssignedQuizzes(int userId, int studyPlanId)
        {
            var assignments = await _context.QuizAssignments
                .Include(a => a.Quiz)
                .ThenInclude(q => q.Questions)
                .ThenInclude(q => q.Options)
                .Where(a => a.AssignedToUserId == userId && a.State == QuizState.Assigned && a.Quiz.StudyPlanId == studyPlanId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<QuizAssignmentResponseDTO>>(assignments);
        }

        public async Task<QuizAssignmentResponseDTO?> GetAssignedQuizById(int userId, int quizId)
        {
            var assignment = await _context.QuizAssignments
                .Include(a => a.Quiz)
                .ThenInclude(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(a => a.AssignedToUserId == userId && a.QuizId == quizId);

            if (assignment == null)
                return null;

            return _mapper.Map<QuizAssignmentResponseDTO>(assignment);
        }

        public async Task<IEnumerable<QuizResponseDTO>> GetCreatedQuizzes(int userId, int studyPlanId)
        {
            var quizzes = await _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .Where(q => q.CreatedByUserId == userId && q.StudyPlanId == studyPlanId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<QuizResponseDTO>>(quizzes);
        }

        public Task<IEnumerable<QuizResponseDTO>> GetQuizzesForStudyPlan(int studyPlanId)
        {
            throw new NotImplementedException();
        }

        public async Task<QuizAssignmentResponseDTO?> UpdateQuizScore(int assignmentId, ICollection<UserAnswerDTO> userAnswers)
        {
            var assignment = await _context.QuizAssignments
                .Include(a => a.Quiz)
                .ThenInclude(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

            if (assignment == null)
                return null;

            int correctCount = 0;
            int total = assignment.Quiz.Questions.Count;

            // For each question in the assignment, check if the user’s selectedOption is correct
            foreach (var userAnswer in userAnswers)
            {
                var question = assignment.Quiz.Questions
                    .FirstOrDefault(q => q.QuestionId == userAnswer.QuestionId);

                if (question != null)
                {
                    // The “correct” option is the one with `IsCorrect == true`
                    var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);

                    if (correctOption != null && correctOption.OptionId == userAnswer.SelectedOptionId)
                    {
                        correctCount++;
                    }
                }
            }

            assignment.CorrectAnswers = correctCount;
            assignment.TotalQuestions = total;
            assignment.CompletedOn = DateTime.UtcNow;
            assignment.State = QuizState.Completed;

            _context.QuizAssignments.Update(assignment);
            await _context.SaveChangesAsync();

            return _mapper.Map<QuizAssignmentResponseDTO?>(assignment);
        }

        public async Task<IEnumerable<QuizAssignmentResponseDTO>> GetCompletedQuizzes(int userId, int studyPlanId)
        {
            var assignments = await _context.QuizAssignments
                .Include(a => a.Quiz)
                .ThenInclude(q => q.Questions)
                .ThenInclude(q => q.Options)
                .Where(a => a.AssignedToUserId == userId && a.State == QuizState.Completed && a.Quiz.StudyPlanId == studyPlanId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<QuizAssignmentResponseDTO>>(assignments);
        }
    }
}
