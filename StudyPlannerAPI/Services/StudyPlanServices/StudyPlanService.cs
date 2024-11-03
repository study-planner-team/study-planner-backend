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

            // Add the creator as a member of this study plan
            var member = new StudyPlanMembers
            {
                UserId = userId,
                StudyPlan = studyPlan,
                JoinedDate = DateTime.UtcNow
            };

            _context.StudyPlanMembers.Add(member);

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

        public async Task<IEnumerable<StudyPlanResponseDTO>> GetPublicStudyPlans(int userId)
        {
            var studyPlans = await _context.StudyPlans
                .Include(sp => sp.User)
                .Where(sp => sp.IsPublic == true && sp.UserId != userId) // Exclude plans where the user is the owner
                .Where(sp => !_context.StudyPlanMembers
                    .Any(m => m.StudyPlanId == sp.StudyPlanId && m.UserId == userId)) // Exclude plans where the user is a member
                .ToListAsync();

            return _mapper.Map<IEnumerable<StudyPlanResponseDTO>>(studyPlans);
        }

        public async Task<IEnumerable<StudyPlanResponseDTO>> GetJoinedStudyPlansForUser(int userId)
        {
            var joinedPlans = await _context.StudyPlans
                .Where(sp => _context.StudyPlanMembers
                    .Any(m => m.StudyPlanId == sp.StudyPlanId && m.UserId == userId) // User is a member
                    && sp.UserId != userId) // Exclude plans where the user is the owner
                .ToListAsync();

            return _mapper.Map<IEnumerable<StudyPlanResponseDTO>>(joinedPlans);
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
            // Get the Users from StudyPlanMembers
            var members = await _context.StudyPlanMembers.Where(m => m.StudyPlanId == studyPlanId).Select(m => m.User).ToListAsync(); 

            return _mapper.Map<IEnumerable<UserResponseDTO>>(members);
        }

        public async Task<bool> LeavePublicStudyPlan(int userId, int studyPlanId)
        {
            var member = await _context.StudyPlanMembers.FirstOrDefaultAsync(m => m.UserId == userId && m.StudyPlanId == studyPlanId);

            if (member == null)
            {
                return false; 
            }

            _context.StudyPlanMembers.Remove(member);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ChangeStudyPlanOwner(int currentOwnerId, int studyPlanId, int newOwnerId)
        {
            var studyPlan = await _context.StudyPlans.FirstOrDefaultAsync(sp => sp.StudyPlanId == studyPlanId && sp.UserId == currentOwnerId);

            if (studyPlan == null)
            {
                return false;
            }

            // Check if the new owner is a member of the study plan
            bool isMember = await _context.StudyPlanMembers.AnyAsync(spm => spm.StudyPlanId == studyPlanId && spm.UserId == newOwnerId);

            if (!isMember)
            {
                return false; // New owner must be a member of the study plan
            }

            // Update the owner
            studyPlan.UserId = newOwnerId;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
