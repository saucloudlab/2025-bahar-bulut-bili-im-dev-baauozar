using CoreApiService.Domain.Entities;

namespace CoreApiService.Application.Interfaces
{
    public interface IAnalysisRepository
    {
        Task<AnalysisRequest> CreateAsync(AnalysisRequest request);
        Task<AnalysisRequest?> GetByIdAsync(Guid id);
        Task UpdateAsync(AnalysisRequest request);
    }
}