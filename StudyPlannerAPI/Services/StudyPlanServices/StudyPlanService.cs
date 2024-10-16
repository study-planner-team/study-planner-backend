﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Data;
using StudyPlannerAPI.Models.StudyPlans;

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
    }
}
