using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Data;
using StudyPlannerAPI.Models.Badges;
using StudyPlannerAPI.Models.StudySessions;

namespace StudyPlannerAPI.Services.BadgeService
{
    public class BadgeService : IBadgeService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public BadgeService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<BadgeResponseDTO>> GetUserBadges(int userId)
        {
            var badges = await _context.Badges.ToListAsync();

            var badgeDTOs = _mapper.Map<List<BadgeResponseDTO>>(badges);

            foreach (var badgeDTO in badgeDTOs)
            {
                badgeDTO.Earned = await _context.UserBadges.AnyAsync(ub => ub.UserId == userId && ub.BadgeId == badgeDTO.BadgeId);
            }

            return badgeDTOs;
        }

        public async Task AssignBadgesToUser(int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
            if (!userExists) return;

            // Fetch existing badge assignments
            var existingBadgeIds = await _context.UserBadges
                .Where(ub => ub.UserId == userId)
                .Select(ub => ub.BadgeId)
                .ToListAsync();

            var earnedBadges = new List<UserBadge>();

            // Badge 1: First Steps
            if (!existingBadgeIds.Contains(1) && await HasCreatedAnyStudyPlans(userId))
                earnedBadges.Add(new UserBadge { UserId = userId, BadgeId = 1 });

            // Badge 2: Quiz Genius
            if (!existingBadgeIds.Contains(2) && await HasCreatedEnoughQuizzes(userId, 10))
                earnedBadges.Add(new UserBadge { UserId = userId, BadgeId = 2 });

            // Badge 3: Team Player
            if (!existingBadgeIds.Contains(3) && await HasJoinedEnoughGroupPlans(userId, 3))
                earnedBadges.Add(new UserBadge { UserId = userId, BadgeId = 3 });

            // Badge 4: Consistency Master
            if (!existingBadgeIds.Contains(4) && await HasConsecutiveStudyDays(userId, 7))
                earnedBadges.Add(new UserBadge { UserId = userId, BadgeId = 4 });

            // Badge 5: Planner Enthusiast
            if (!existingBadgeIds.Contains(5) && await HasCreatedEnoughStudyPlans(userId, 5))
                earnedBadges.Add(new UserBadge { UserId = userId, BadgeId = 5 });

            // Badge 6: Time Keeper
            if (!existingBadgeIds.Contains(6) && await HasStudiedEnoughHours(userId, 50))
                earnedBadges.Add(new UserBadge { UserId = userId, BadgeId = 6 });

            // Badge 7: Knowledge Sharer
            if (!existingBadgeIds.Contains(7) && await HasSharedAnyStudyPlans(userId))
                earnedBadges.Add(new UserBadge { UserId = userId, BadgeId = 7 });

            // Badge 8: Quiz Creator
            if (!existingBadgeIds.Contains(8) && await HasCreatedEnoughQuizzes(userId, 10))
                earnedBadges.Add(new UserBadge { UserId = userId, BadgeId = 8 });

            if (earnedBadges.Count > 0)
            {
                _context.UserBadges.AddRange(earnedBadges);
                await _context.SaveChangesAsync();
            }
        }

        private async Task<bool> HasCreatedAnyStudyPlans(int userId)
        {
            return await _context.StudyPlans.AnyAsync(sp => sp.UserId == userId);
        }

        private async Task<bool> HasCreatedEnoughQuizzes(int userId, int count)
        {
            return await _context.Quizzes.CountAsync(q => q.CreatedByUserId == userId) >= count;
        }

        private async Task<bool> HasJoinedEnoughGroupPlans(int userId, int count)
        {
            return await _context.StudyPlanMembers
                .Where(spm => spm.UserId == userId &&
                              !_context.StudyPlans.Any(sp => sp.StudyPlanId == spm.StudyPlanId && sp.UserId == userId))
                .CountAsync() >= count;
        }

        private async Task<bool> HasConsecutiveStudyDays(int userId, int days)
        {
            return await CalculateConsecutiveStudyDays(userId) >= days;
        }

        private async Task<bool> HasCreatedEnoughStudyPlans(int userId, int count)
        {
            return await _context.StudyPlans.CountAsync(sp => sp.UserId == userId) >= count;
        }

        private async Task<bool> HasStudiedEnoughHours(int userId, double hours)
        {
            var totalHours = (await _context.StudySessions
                .Where(ss => ss.UserId == userId && ss.Status == StudySessionStatus.Completed && ss.ActualDuration.HasValue)
                .Select(ss => ss.ActualDuration.Value)
                .ToListAsync())
                .Sum(duration => duration.TotalHours);

            return totalHours >= hours;
        }

        private async Task<bool> HasSharedAnyStudyPlans(int userId)
        {
            return await _context.StudyPlans.AnyAsync(sp => sp.UserId == userId && sp.IsPublic);
        }


        private async Task<int> CalculateConsecutiveStudyDays(int userId)
        {
            var sessions = await _context.StudySessions
                .Where(ss => ss.UserId == userId && ss.Status == StudySessionStatus.Completed)
                .OrderBy(ss => ss.Date)
                .Select(ss => ss.Date)
                .ToListAsync();

            int maxConsecutiveDays = 0;
            int currentStreak = 0;
            DateTime? previousDate = null;

            foreach (var date in sessions)
            {
                if (previousDate.HasValue && (date - previousDate.Value).TotalDays == 1)
                {
                    currentStreak++;
                }
                else
                {
                    currentStreak = 1;
                }

                maxConsecutiveDays = Math.Max(maxConsecutiveDays, currentStreak);
                previousDate = date;
            }

            return maxConsecutiveDays;
        }

    }
}
