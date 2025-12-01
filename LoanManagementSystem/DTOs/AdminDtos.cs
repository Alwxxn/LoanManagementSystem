using LoanManagementSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace LoanManagementSystem.DTOs;

public record ApprovalRequest([Required] bool Approve);

public record HelpReportUpdateRequest(
    [Required, MaxLength(150)] string Title,
    [Required, MaxLength(1000)] string Message,
    HelpStatus Status);

