﻿namespace StriveUp.Shared.DTOs
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TimeAgo { get; set; }
        public string? ActorId { get; set; }
        public string? ActorName { get; set; }
        public string? ActorAvatar { get; set; }
        public string RedirectUrl { get; set; }
    }
}