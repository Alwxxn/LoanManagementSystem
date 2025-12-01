namespace LoanManagementSystem.Models;

public class HelpReport : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public HelpStatus Status { get; set; } = HelpStatus.Open;

    public Guid CreatedById { get; set; }
    public ApplicationUser? CreatedBy { get; set; }

    public Guid? UpdatedById { get; set; }
    public ApplicationUser? UpdatedBy { get; set; }
}

