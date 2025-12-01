namespace LoanManagementSystem.Models;

public class FeedbackResponse : BaseEntity
{
    public Guid QuestionId { get; set; }
    public FeedbackQuestion? Question { get; set; }

    public Guid CustomerId { get; set; }
    public ApplicationUser? Customer { get; set; }

    public string Answer { get; set; } = string.Empty;
}

