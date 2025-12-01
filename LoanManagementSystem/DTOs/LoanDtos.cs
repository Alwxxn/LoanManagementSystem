using LoanManagementSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace LoanManagementSystem.DTOs;

public record LoanApplicationRequest(
    [Required] decimal Amount,
    [Required, Range(1, 360)] int TenureMonths,
    [Required, MaxLength(100)] string LoanType,
    [MaxLength(500)] string Purpose);

public record AssignOfficerRequest(
    [Required] Guid LoanApplicationId,
    [Required] Guid OfficerId);

public record VerificationUpdateRequest(
    [Required] Guid LoanApplicationId,
    [Required] string Notes,
    VerificationStatus Status);

public record HelpReportRequest(
    [Required, MaxLength(150)] string Title,
    [Required, MaxLength(1000)] string Message,
    HelpStatus Status = HelpStatus.Open);

public record FeedbackQuestionRequest(
    [Required, MaxLength(300)] string Question,
    bool IsActive = true);

public record FeedbackResponseRequest(
    [Required] Guid QuestionId,
    [Required, MaxLength(1000)] string Answer);

