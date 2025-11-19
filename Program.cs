using Microsoft.OpenApi.Models;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// (اختياري) فرض عناوين URL محددة حتى نعرف أي منفذ يستخدم
builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Student Api", Version = "v1" });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
    c.RoutePrefix = string.Empty; // يخلي Swagger في /
});

app.MapControllers();

// افتح Chrome بعد ما يبدأ التطبيق فعلاً
var swaggerUrl = "https://localhost:5001/"; // غيّره إلى http إذا تحب: "http://localhost:5000/"
app.Lifetime.ApplicationStarted.Register(() =>
{
    try
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "chrome",         // لو Chrome مو في PATH حط المسار الكامل هنا
            Arguments = swaggerUrl,
            UseShellExecute = true
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine("Could not open Chrome automatically: " + ex.Message);
    }
});

app.Run();

