namespace StriveUp.Shared.DTOs.Profile
{
    public class UserProfileDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Avatar { get; set; }
        public int CurrentXP { get; set; }
        public int LevelNumber { get; set; }
        public int LevelTotalXP { get; set; }
        public string? Bio { get; set; }
        public double? Weight { get; set; }
        public double? Height { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public List<MedalDto>? Medals { get; set; }
        public List<SimpleUserDto>? Followers { get; set; }
        public List<SimpleUserDto>? Following { get; set; }
    }
}