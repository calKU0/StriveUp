namespace StriveUp.Shared.DTOs
{
    public class ErrorResponse
    {
        public string? Message { get; set; }
        public string? Details { get; set; }
        public List<string>? Errors { get; set; }
    }
}