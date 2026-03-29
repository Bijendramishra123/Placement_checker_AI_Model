var builder = WebApplication.CreateBuilder(args);

// ======================
// Add services to container
// ======================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ======================
// CORS Configuration (React connect ke liye)
// ======================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ======================
// Build app
// ======================
var app = builder.Build();

// ======================
// Middleware pipeline
// ======================

// Swagger enable (har environment me)
app.UseSwagger();
app.UseSwaggerUI();

// CORS enable
app.UseCors("AllowFrontend");

// ❌ IMPORTANT: HTTPS redirection remove for Render
// app.UseHttpsRedirection();

// Authorization
app.UseAuthorization();

// Controllers map
app.MapControllers();

// ======================
// Run application (Render compatible)
// ======================
app.Run("http://0.0.0.0:10000");