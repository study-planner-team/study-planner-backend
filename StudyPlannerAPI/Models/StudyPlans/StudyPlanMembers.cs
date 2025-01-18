using StudyPlannerAPI.Models.Users;
using System.ComponentModel.DataAnnotations;

namespace StudyPlannerAPI.Models.StudyPlans
{
    public class StudyPlanMembers
    {
        [Key]
        public int MembershipId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int StudyPlanId { get; set; }
        public StudyPlan StudyPlan { get; set; }

        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;
    }
}
