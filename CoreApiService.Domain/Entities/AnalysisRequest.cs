namespace CoreApiService.Domain.Entities
{
    public class AnalysisRequest
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string? XaiResult { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}