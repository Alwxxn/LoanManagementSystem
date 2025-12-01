using LoanManagementSystem.DTOs;
using LoanManagementSystem.Models;
using LoanManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoanManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController(
    IUserRepository userRepository,
    ILoanApplicationRepository loanApplicationRepository,
    IHelpReportRepository helpReportRepository,
    IFeedbackQuestionRepository feedbackQuestionRepository,
    IFeedbackResponseRepository feedbackResponseRepository) : ControllerBase
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ILoanApplicationRepository _loanApplicationRepository = loanApplicationRepository;
    private readonly IHelpReportRepository _helpReportRepository = helpReportRepository;
    private readonly IFeedbackQuestionRepository _feedbackQuestionRepository = feedbackQuestionRepository;
    private readonly IFeedbackResponseRepository _feedbackResponseRepository = feedbackResponseRepository;

    [HttpPost("{customerId:guid}/loans")]
    public async Task<IActionResult> ApplyForLoan(
        Guid customerId,
        [FromBody] LoanApplicationRequest request,
        CancellationToken cancellationToken)
    {
        var customer = await _userRepository.Query()
            .FirstOrDefaultAsync(u => u.Id == customerId && u.Role == UserRole.Customer, cancellationToken);

        if (customer is null)
        {
            return NotFound(new { message = "Customer not found." });
        }

        if (customer.ApprovalStatus != ApprovalStatus.Approved)
        {
            return BadRequest(new { message = "Customer is not approved yet." });
        }

        var loan = new LoanApplication
        {
            CustomerId = customerId,
            Amount = request.Amount,
            TenureMonths = request.TenureMonths,
            Purpose = request.Purpose,
            LoanType = request.LoanType,
            Status = LoanStatus.Submitted
        };

        await _loanApplicationRepository.AddAsync(loan, cancellationToken);
        await _loanApplicationRepository.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetLoanRequests), new { customerId }, loan);
    }

    [HttpGet("{customerId:guid}/loans")]
    public async Task<IActionResult> GetLoanRequests(Guid customerId, CancellationToken cancellationToken)
    {
        var exists = await _userRepository.Query()
            .AnyAsync(u => u.Id == customerId && u.Role == UserRole.Customer, cancellationToken);

        if (!exists)
        {
            return NotFound(new { message = "Customer not found." });
        }

        var loans = await _loanApplicationRepository.Query()
            .Include(l => l.AssignedOfficer)
            .Include(l => l.BackgroundVerification)
            .Include(l => l.LoanVerification)
            .Where(l => l.CustomerId == customerId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(cancellationToken);

        return Ok(loans);
    }

    [HttpGet("help")]
    public async Task<IActionResult> GetHelpReports(CancellationToken cancellationToken)
    {
        var reports = await _helpReportRepository.Query()
            .Include(h => h.CreatedBy)
            .OrderByDescending(h => h.UpdatedAt ?? h.CreatedAt)
            .ToListAsync(cancellationToken);

        return Ok(reports);
    }

    [HttpPost("{customerId:guid}/help")]
    public async Task<IActionResult> CreateHelpReport(
        Guid customerId,
        [FromBody] HelpReportRequest request,
        CancellationToken cancellationToken)
    {
        var exists = await _userRepository.Query()
            .AnyAsync(u => u.Id == customerId && u.Role == UserRole.Customer, cancellationToken);

        if (!exists)
        {
            return NotFound(new { message = "Customer not found." });
        }

        var report = new HelpReport
        {
            Title = request.Title,
            Message = request.Message,
            Status = HelpStatus.Open,
            CreatedById = customerId
        };

        await _helpReportRepository.AddAsync(report, cancellationToken);
        await _helpReportRepository.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetHelpReports), report);
    }

    [HttpPost("{customerId:guid}/feedback")]
    public async Task<IActionResult> AddFeedback(
        Guid customerId,
        [FromBody] FeedbackResponseRequest request,
        CancellationToken cancellationToken)
    {
        var customer = await _userRepository.Query()
            .FirstOrDefaultAsync(u => u.Id == customerId && u.Role == UserRole.Customer, cancellationToken);

        if (customer is null)
        {
            return NotFound(new { message = "Customer not found." });
        }

        var questionExists = await _feedbackQuestionRepository.Query()
            .AnyAsync(q => q.Id == request.QuestionId && q.IsActive, cancellationToken);

        if (!questionExists)
        {
            return BadRequest(new { message = "Feedback question is invalid." });
        }

        var response = new FeedbackResponse
        {
            QuestionId = request.QuestionId,
            CustomerId = customerId,
            Answer = request.Answer
        };

        await _feedbackResponseRepository.AddAsync(response, cancellationToken);
        await _feedbackResponseRepository.SaveChangesAsync(cancellationToken);

        return Ok(response);
    //Email: admin@loanms.com
    //Password: Admin@123
    }
}

