namespace StriveUp.Shared.DTOs
{
    public class JwtResponse
    {
        public string? Token { get; set; }
        public string RefreshToken { get; set; }
    }
}