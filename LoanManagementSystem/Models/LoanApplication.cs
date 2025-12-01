namespace LoanManagementSystem.Models;

public class LoanApplication : BaseEntity
{
    public Guid CustomerId { get; set; }
    public ApplicationUser? Customer { get; set; }

    public Guid? AssignedOfficerId { get; set; }
    public ApplicationUser? AssignedOfficer { get; set; }

    public decimal Amount { get; set; }
    public int TenureMonths { get; set; }
    public string LoanType { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public LoanStatus Status { get; set; } = LoanStatus.Submitted;

    public BackgroundVerification? BackgroundVerification { get; set; }
    public LoanVerification? LoanVerification { get; set; }
}

