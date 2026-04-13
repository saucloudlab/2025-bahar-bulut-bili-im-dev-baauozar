using CoreApiService.Application.DTOs;
using CoreApiService.Application.Interfaces;
using CoreApiService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CoreApiService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly IAnalysisRepository _repository;

        public AnalysisController(IAnalysisRepository repository)
        {
            _repository = repository;
        }

     [HttpPost]
public async Task<IActionResult> CreateAnalysis([FromBody] CreateAnalysisDto dto)
{
    // 1. إنشاء الطلب وحفظه في قاعدة البيانات كـ Pending
    var request = new AnalysisRequest
    {
        Id = Guid.NewGuid(),
        ImageUrl = dto.ImageUrl,
        Status = "Pending",
        CreatedAt = DateTime.UtcNow
    };
    await _repository.CreateAsync(request);

    // 2. التواصل مع خدمة البايثون (Vision Service)
    using var client = new HttpClient();
    var aiRequest = new { imageUrl = dto.ImageUrl };
    var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(aiRequest), System.Text.Encoding.UTF8, "application/json");
    
    try 
    {
        // نستخدم اسم الحاوية "vision" للوصول للخدمة عبر شبكة الدوكر الداخلية
        var response = await client.PostAsync("http://vision:8000/api/analyze", content);
        if (response.IsSuccessStatusCode)
        {
            var resultString = await response.Content.ReadAsStringAsync();
            var aiResult = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(resultString);
            
            // 3. تحديث النتيجة في قاعدة البيانات
            request.Status = aiResult.GetProperty("status").GetString() ?? "Completed";
            request.XaiResult = $"Stress Level: {aiResult.GetProperty("stressLevel").GetString()} - {aiResult.GetProperty("xaiResult").GetString()}";
            
            await _repository.UpdateAsync(request);
        }
    }
    catch (Exception)
    {
        request.Status = "Failed to reach Vision Service";
        await _repository.UpdateAsync(request);
    }

    return Ok(new { Message = "Analysis Processed", Id = request.Id, Status = request.Status, XaiResult = request.XaiResult });
}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnalysis(Guid id)
        {
            var result = await _repository.GetByIdAsync(id);
            if (result == null) return NotFound();

            var responseDto = new AnalysisResponseDto
            {
                Id = result.Id,
                ImageUrl = result.ImageUrl,
                Status = result.Status,
                XaiResult = result.XaiResult,
                CreatedAt = result.CreatedAt
            };

            return Ok(responseDto);
        }
    }
}