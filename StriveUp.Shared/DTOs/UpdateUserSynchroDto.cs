namespace StriveUp.Shared.DTOs
{
    public class UpdateUserSynchroDto
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool? IsActive { get; set; }
    }
}