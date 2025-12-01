namespace LoanManagementSystem.Models;

public class FeedbackQuestion : BaseEntity
{
    public string Question { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public ICollection<FeedbackResponse> Responses { get; set; } = new List<FeedbackResponse>();
}

