using CoreApiService.Application.Interfaces;
using CoreApiService.Infrastructure.Data;
using CoreApiService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// إضافة خدمات الـ Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ربط قاعدة البيانات
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// حقن الاعتماديات (Dependency Injection)
builder.Services.AddScoped<IAnalysisRepository, AnalysisRepository>();

var app = builder.Build();


    app.UseSwagger();
    app.UseSwaggerUI();


app.UseAuthorization();
app.MapControllers();
// بناء جداول قاعدة البيانات تلقائياً عند تشغيل التطبيق
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}
app.Run();