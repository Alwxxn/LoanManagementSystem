using LoanManagementSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace LoanManagementSystem.DTOs;

public record RegisterRequest(
    [Required, MaxLength(150)] string FullName,
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password,
    [Required, Phone] string PhoneNumber,
    [Required] UserRole Role);

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);

public record AuthResponse(Guid UserId, string FullName, string Email, UserRole Role, ApprovalStatus ApprovalStatus);

