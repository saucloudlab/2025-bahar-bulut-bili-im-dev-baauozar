using System;

namespace CoreApiService.Application.DTOs
{
    public class AnalysisResponseDto
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? XaiResult { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}