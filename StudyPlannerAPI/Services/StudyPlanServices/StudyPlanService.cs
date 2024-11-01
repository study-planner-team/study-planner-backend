using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Data;
using StudyPlannerAPI.Models.StudyPlans;
using StudyPlannerAPI.Models.Users;

namespace StudyPlannerAPI.Services.StudyPlanServices
{
    public class StudyPlanService : IStudyPlanService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public StudyPlanService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<StudyPlanResponseDTO> CreateStudyPlan(int userId, StudyPlanDTO studyPlanDTO)
        {
            var studyPlan = _mapper.Map<StudyPlan>(studyPlanDTO);
            studyPlan.UserId = userId;

            _context.StudyPlans.Add(studyPlan);
            await _context.SaveChangesAsync();

            return _mapper.Map<StudyPlanResponseDTO>(studyPlan);
        }

        public async Task<bool> DeleteStudyPlan(int planId)
        {
            var studyPlan = await _context.StudyPlans.FirstOrDefaultAsync(sp => sp.StudyPlanId == planId);

            if (studyPlan == null)
                return false;

            _context.StudyPlans.Remove(studyPlan);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<StudyPlanResponseDTO?> GetStudyPlanById(int planId)
        {
            var studyPlan = await _context.StudyPlans.Include(sp => sp.User).FirstOrDefaultAsync(sp => sp.StudyPlanId == planId);

            if (studyPlan == null)
                return null;

            return _mapper.Map<StudyPlanResponseDTO>(studyPlan);
        }

        public async Task<IEnumerable<StudyPlanResponseDTO>> GetStudyPlansForUser(int userId)
        {
            var studyPlans = await _context.StudyPlans.Where(sp => sp.UserId == userId && sp.IsArchived == false).ToListAsync();

            return _mapper.Map<IEnumerable<StudyPlanResponseDTO>>(studyPlans);
        }

        public async Task<StudyPlanResponseDTO?> UpdateStudyPlan(int planId, StudyPlanDTO studyPlanDTO)
        {
            var studyPlan = await _context.StudyPlans.FirstOrDefaultAsync(sp => sp.StudyPlanId == planId);

            if (studyPlan == null)
                return null;

            studyPlan.Title = studyPlanDTO.Title;
            studyPlan.Description = studyPlanDTO.Description;
            studyPlan.Category = studyPlanDTO.Category;
            studyPlan.StartDate = studyPlanDTO.StartDate;
            studyPlan.EndDate = studyPlanDTO.EndDate;
            studyPlan.IsPublic = studyPlanDTO.IsPublic;

            await _context.SaveChangesAsync();

            return _mapper.Map<StudyPlanResponseDTO>(studyPlan);
        }
        public async Task<StudyPlanResponseDTO> ArchiveStudyPlan(int planId)
        {
            var studyPlan = await _context.StudyPlans.FindAsync(planId);

            if (studyPlan == null)
                return null;

            studyPlan.IsArchived = true;
            await _context.SaveChangesAsync();

            return _mapper.Map<StudyPlanResponseDTO>(studyPlan);
        }
        public async Task<StudyPlanResponseDTO?> UnarchiveStudyPlan(int planId)
        {
            var studyPlan = await _context.StudyPlans.FindAsync(planId);

            if (studyPlan == null)
                return null;

            studyPlan.IsArchived = false;
            await _context.SaveChangesAsync();

            return _mapper.Map<StudyPlanResponseDTO>(studyPlan);
        }

        public async Task<IEnumerable<StudyPlanResponseDTO>> GetArchivedStudyPlansForUser(int userId)
        {
            var archivedPlans = await _context.StudyPlans
                .Where(sp => sp.UserId == userId && sp.IsArchived)
                .ToListAsync();

            return _mapper.Map<IEnumerable<StudyPlanResponseDTO>>(archivedPlans);
        }

        public async Task<IEnumerable<StudyPlanResponseDTO>> GetPublicStudyPlans()
        {
            var studyPlans = await _context.StudyPlans.Include(sp => sp.User).Where(sp => sp.IsPublic == true).ToListAsync();

            return _mapper.Map<IEnumerable<StudyPlanResponseDTO>>(studyPlans);
        }

        public async Task<bool> JoinPublicStudyPlan(int userId, int studyPlanId)
        {
            // Check if the study plan is public
            var studyPlan = await _context.StudyPlans.FirstOrDefaultAsync(sp => sp.StudyPlanId == studyPlanId && sp.IsPublic);

            if (studyPlan == null)
                return false; // Study plan not found or is not public

            // Check if the user is already a member
            var existingMembership = await _context.StudyPlanMembers
                .FirstOrDefaultAsync(m => m.UserId == userId && m.StudyPlanId == studyPlanId);

            if (existingMembership != null)
                return false; // User is already a member

            // Create a new membership
            var membership = new StudyPlanMembers
            {
                UserId = userId,
                StudyPlanId = studyPlanId
            };

            _context.StudyPlanMembers.Add(membership);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<UserResponseDTO>> GetStudyPlanMembers(int studyPlanId)
        {
            var members = await _context.StudyPlanMembers
                .Where(m => m.StudyPlanId == studyPlanId)
                .Select(m => m.User) // Get the User entity from StudyPlanMembership
                .ToListAsync();

            // Use AutoMapper to map the list of Users to StudyPlanMemberDTOs
            return _mapper.Map<List<UserResponseDTO>>(members);
        }
    }
}
