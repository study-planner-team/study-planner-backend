namespace StudyPlannerAPI.Models.Users
{
    public class UserPasswordChangeDTO
    {
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
