using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Student_Api_Project.Authorization;
using System.Diagnostics;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "Student_Api_Project",
            ValidAudience = "StudentApiUser",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("THIS_IS_A_VERY_SECRET_KEY_123456"))
        };
    });


builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName);
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {

        Name = "Authorization",

        Type = SecuritySchemeType.Http,

        Scheme = "Bearer",


        BearerFormat = "JWT",

        In = ParameterLocation.Header,

        Description = "Enter: Bearer {your JWT token}"
    });

 options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
               
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },

            new string[] {}
        }
    });
});

builder.Services.AddCors( options =>
{
    options.AddPolicy("StudentApiCorsPoicy", policy=>
    {
        policy.
        WithOrigins(
            "http://localhost:5000",
            "https://localhost:5001"
            )
             .AllowAnyHeader()
             .AllowAnyMethod();
    });
});
builder.Services.AddSingleton<IAuthorizationHandler, StudentOwnerOrAdminHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("StudentOwnerOrAdmin", policy =>
        policy.Requirements.Add(new StudentOwnerOrAdminRequirement()));
});


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
    c.RoutePrefix = string.Empty; 
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("StudentApiCorsPoicy");

app.MapControllers();


var swaggerUrl = "https://localhost:5001/"; 
app.Lifetime.ApplicationStarted.Register(() =>
{
    try
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "chrome",        
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

