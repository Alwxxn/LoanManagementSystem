namespace LoanManagementSystem.Models;

public class BackgroundVerification : BaseEntity
{
    public Guid LoanApplicationId { get; set; }
    public LoanApplication? LoanApplication { get; set; }

    public Guid? OfficerId { get; set; }
    public ApplicationUser? Officer { get; set; }

    public string Notes { get; set; } = string.Empty;
    public VerificationStatus Status { get; set; } = VerificationStatus.Pending;
    public DateTime? CompletedOn { get; set; }
}

