using System;

namespace Application.Features.ShareLinks.DTOs
{
    public class ShareLinkDto
    {
        public string Token { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool IsRevoked { get; set; }
    }
}
