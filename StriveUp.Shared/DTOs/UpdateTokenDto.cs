﻿using System.Text.Json.Serialization;

namespace StriveUp.Shared.DTOs
{
    public class UpdateTokenDto
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        public DateTime TokenExpiresAt => DateTime.UtcNow.AddSeconds(ExpiresIn);
    }
}