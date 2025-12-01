using LoanManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LoanManagementSystem.Data;

public class DbSeeder(
    LoanManagementContext context,
    ILogger<DbSeeder> logger,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager)
{
    private readonly LoanManagementContext _context = context;
    private readonly ILogger<DbSeeder> _logger = logger;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager = roleManager;

    private static readonly Guid AdminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private const string DefaultAdminEmail = "admin@loanms.com";
    private static readonly string[] DefaultRoles = ["Admin", "Customer", "LoanOfficer", "FieldOfficer"];

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.MigrateAsync(cancellationToken);
        await EnsureRolesAsync(cancellationToken);
        await EnsureAdminUserAsync(cancellationToken);
        await EnsureDefaultFeedbackQuestionsAsync(cancellationToken);
    }

    private async Task EnsureRolesAsync(CancellationToken cancellationToken)
    {
        foreach (var roleName in DefaultRoles)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var identityRole = new IdentityRole<Guid>
            {
                Id = Guid.NewGuid(),
                Name = roleName,
                NormalizedName = roleName.ToUpperInvariant()
            };

            await _roleManager.CreateAsync(identityRole);
        }
    }

    private async Task EnsureAdminUserAsync(CancellationToken cancellationToken)
    {
        var adminExists = await _context.Users.AnyAsync(u => u.Role == UserRole.Admin, cancellationToken);
        if (adminExists)
        {
            return;
        }

        var admin = new ApplicationUser
        {
            Id = AdminId,
            UserName = DefaultAdminEmail,
            Email = DefaultAdminEmail,
            PhoneNumber = "0000000000",
            FullName = "System Administrator",
            Role = UserRole.Admin,
            ApprovalStatus = ApprovalStatus.Approved,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(admin, "Admin@123");
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogError("Failed to create default admin user: {Errors}", errors);
            return;
        }

        await _userManager.AddToRoleAsync(admin, "Admin");
        _logger.LogInformation("Default admin user created with email {Email}", DefaultAdminEmail);
    }

    private async Task EnsureDefaultFeedbackQuestionsAsync(CancellationToken cancellationToken)
    {
        if (await _context.FeedbackQuestions.AnyAsync(cancellationToken))
        {
            return;
        }

        var questions = new[]
        {
            new FeedbackQuestion { Question = "How satisfied are you with the loan application process?" },
            new FeedbackQuestion { Question = "Was the field officer helpful during verification?" }
        };

        await _context.FeedbackQuestions.AddRangeAsync(questions, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

