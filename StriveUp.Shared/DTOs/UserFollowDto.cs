namespace StriveUp.Shared.DTOs
{
    public class UserFollowDto
    {
        public string UserId { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string? Avatar { get; set; }
        public bool IsFollowed { get; set; }
    }
}