
using LoanManagementSystem.Data;
using LoanManagementSystem.Models;
using LoanManagementSystem.Repositories;
using LoanManagementSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LoanManagementSystem;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<LoanManagementContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

        builder.Services
            .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<LoanManagementContext>()
            .AddDefaultTokenProviders();

        // Generic repository
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Typed repositories
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ILoanApplicationRepository, LoanApplicationRepository>();
        builder.Services.AddScoped<IBackgroundVerificationRepository, BackgroundVerificationRepository>();
        builder.Services.AddScoped<ILoanVerificationRepository, LoanVerificationRepository>();
        builder.Services.AddScoped<IHelpReportRepository, HelpReportRepository>();
        builder.Services.AddScoped<IFeedbackQuestionRepository, FeedbackQuestionRepository>();
        builder.Services.AddScoped<IFeedbackResponseRepository, FeedbackResponseRepository>();

        // Services
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<DbSeeder>();

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
            await seeder.SeedAsync();
        }

        app.UseHttpsRedirection();
        
        app.UseCors(policy => policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());

        app.UseAuthorization();
        app.MapControllers();

        await app.RunAsync();
    }
}
