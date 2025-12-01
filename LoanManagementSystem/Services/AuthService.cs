using LoanManagementSystem.DTOs;
using LoanManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LoanManagementSystem.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}

public class AuthService(UserManager<ApplicationUser> userManager) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userManager.Users
            .AnyAsync(u => u.Email == request.Email.ToLowerInvariant(), cancellationToken);

        if (existingUser)
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        var entity = new ApplicationUser
        {
            UserName = request.Email.ToLowerInvariant(),
            Email = request.Email.ToLowerInvariant(),
            PhoneNumber = request.PhoneNumber,
            FullName = request.FullName,
            Role = request.Role,
            ApprovalStatus = request.Role == UserRole.Customer ? ApprovalStatus.Approved : ApprovalStatus.Pending,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(entity, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException(errors);
        }

        await _userManager.AddToRoleAsync(entity, request.Role.ToString());

        return new AuthResponse(entity.Id, entity.FullName, entity.Email!, entity.Role, entity.ApprovalStatus);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant(), cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var validPassword = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!validPassword)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        return new AuthResponse(user.Id, user.FullName, user.Email!, user.Role, user.ApprovalStatus);
    }
}
