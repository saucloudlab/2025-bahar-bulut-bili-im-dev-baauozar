using CoreApiService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreApiService.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // هذا هو الجدول الذي سيتم إنشاؤه في قاعدة البيانات
        public DbSet<AnalysisRequest> AnalysisRequests { get; set; }
    }
}