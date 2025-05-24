namespace StriveUp.Shared.DTOs
{
    public class UserSynchroDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int SynchroProviderId { get; set; }
        public string SynchroProviderName { get; set; }
        public string IconUrl { get; set; }
        public bool IsActive { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime TokenExpiresAt { get; set; }
    }
}