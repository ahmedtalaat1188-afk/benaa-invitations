using Microsoft.EntityFrameworkCore;
using BenaaInvitations.API.Models;
using BenaaInvitations.API.Data;
using BenaaInvitations.API.Services;
using BenaaInvitations.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Multi-tenancy & Services
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IStudentsService, StudentsService>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddHttpClient<IAIDesignService, OpenAIDesignService>();


// Configure Email Settings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));


// Configure Database (Production SQL Server / Development SQLite)
var connectionString = builder.Configuration.GetConnectionString("BenaaConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options => {
    if (!string.IsNullOrEmpty(connectionString) && connectionString.Contains("Server=")) {
        options.UseSqlServer(connectionString);
    } else {
        // Fallback to SQLite for local development
        options.UseSqlite(connectionString ?? "Data Source=benaa.db");
    }
});

// Configure CORS
const string CorsPolicy = "AllowBenaaApp";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
// Enabled Swagger for Production to allow remote debugging
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Benaa Invitations API V1");
    c.RoutePrefix = string.Empty; // Set Swagger at the root
});

app.UseHttpsRedirection();

// Multi-tenancy middleware MUST be before routing/auth to set context
app.UseMiddleware<TenantMiddleware>();

app.UseCors(CorsPolicy);
app.UseAuthorization();
app.MapControllers();

// Ensure Database is Migrated successfully
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // context.Database.EnsureCreated(); // Old way: skips migrations
    context.Database.Migrate(); // New way: follows migration history
}

app.Run();
