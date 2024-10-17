﻿using System.ComponentModel.DataAnnotations;

namespace StudyPlannerAPI.Models.StudyPlans
{
    public class StudyTopic
    {
        [Key]
        public int TopicId { get; set; }
        public required string Title { get; set; }
        public required double Hours { get; set; }
        public int StudyPlanId { get; set; }
        public StudyPlan StudyPlan { get; set; }
    }
}