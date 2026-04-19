using CoreApiService.Application.Interfaces;
using CoreApiService.Infrastructure.Data;
using CoreApiService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Prometheus; // تأكد من وجود هذه المكتبة

var builder = WebApplication.CreateBuilder(args);

// 1. إضافة خدمات الـ Controllers والسواقر
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
// 2. ربط قاعدة البيانات
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. حقن الاعتماديات (Dependency Injection)
builder.Services.AddScoped<IAnalysisRepository, AnalysisRepository>();

var app = builder.Build();

// ==========================================
// الترتيب هنا حساس جداً (Middleware Pipeline)
// ==========================================

// 4. بناء جداول قاعدة البيانات تلقائياً عند تشغيل التطبيق
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// 5. تفعيل السواقر (يجب أن يكون في البداية ليعمل بشكل صحيح)
app.UseSwagger();
app.UseSwaggerUI();

// 6. تفعيل التوجيه (Routing) - شرط أساسي لعمل Prometheus
app.UseRouting();

// 7. تفعيل حساسات Prometheus لمراقبة الطلبات
// (يجب أن تكون بين UseRouting و MapControllers)
app.UseHttpMetrics();

app.UseAuthorization();

// 8. تعيين المسارات (Endpoints)
app.MapControllers(); // مسارات الـ API الخاصة بك
app.UseCors("AllowAll");
app.MapMetrics();     // فتح مسار /metrics لـ Prometheus

app.Run();