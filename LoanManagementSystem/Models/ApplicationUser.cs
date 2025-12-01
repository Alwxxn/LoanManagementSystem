using Microsoft.AspNetCore.Identity;

namespace LoanManagementSystem.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Customer;
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<LoanApplication> LoanApplications { get; set; } = new List<LoanApplication>();
    public ICollection<LoanApplication> AssignedLoanApplications { get; set; } = new List<LoanApplication>();
    public ICollection<FeedbackResponse> FeedbackResponses { get; set; } = new List<FeedbackResponse>();
    public ICollection<HelpReport> HelpReports { get; set; } = new List<HelpReport>();
}

