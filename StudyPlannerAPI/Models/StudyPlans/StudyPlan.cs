﻿using StudyPlannerAPI.Models.StudySessions;
using StudyPlannerAPI.Models.Users;
using System.ComponentModel.DataAnnotations;

namespace StudyPlannerAPI.Models.StudyPlans
{
    public class StudyPlan
    {
        [Key]
        public int StudyPlanId { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsPublic { get; set; }
        public bool IsArchived { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<StudySession> StudySessions { get; set; }
    }
}
