using CoreApiService.Application.Interfaces;
using CoreApiService.Domain.Entities;
using CoreApiService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CoreApiService.Infrastructure.Repositories
{
    public class AnalysisRepository : IAnalysisRepository
    {
        private readonly AppDbContext _context;

        public AnalysisRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AnalysisRequest> CreateAsync(AnalysisRequest request)
        {
            _context.AnalysisRequests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<AnalysisRequest?> GetByIdAsync(Guid id)
        {
            return await _context.AnalysisRequests.FindAsync(id);
        }

        public async Task UpdateAsync(AnalysisRequest request)
        {
            _context.AnalysisRequests.Update(request);
            await _context.SaveChangesAsync();
        }
    }
}