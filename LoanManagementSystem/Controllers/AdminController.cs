using LoanManagementSystem.DTOs;
using LoanManagementSystem.Models;
using LoanManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoanManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController(
    IUserRepository userRepository,
    ILoanApplicationRepository loanApplicationRepository,
    IBackgroundVerificationRepository backgroundVerificationRepository,
    ILoanVerificationRepository loanVerificationRepository,
    IHelpReportRepository helpReportRepository,
    IFeedbackQuestionRepository feedbackQuestionRepository,
    IFeedbackResponseRepository feedbackResponseRepository) : ControllerBase
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ILoanApplicationRepository _loanApplicationRepository = loanApplicationRepository;
    private readonly IBackgroundVerificationRepository _backgroundVerificationRepository = backgroundVerificationRepository;
    private readonly ILoanVerificationRepository _loanVerificationRepository = loanVerificationRepository;
    private readonly IHelpReportRepository _helpReportRepository = helpReportRepository;
    private readonly IFeedbackQuestionRepository _feedbackQuestionRepository = feedbackQuestionRepository;
    private readonly IFeedbackResponseRepository _feedbackResponseRepository = feedbackResponseRepository;

    [HttpGet("customers")]
    public async Task<IActionResult> GetCustomers([FromQuery] ApprovalStatus? status, CancellationToken cancellationToken)
    {
        var customers = await _userRepository.GetByRoleAsync(UserRole.Customer, status, cancellationToken);
        return Ok(customers);
    }

    [HttpPut("customers/{customerId:guid}/approval")]
    public async Task<IActionResult> SetCustomerApproval(Guid customerId, [FromBody] ApprovalRequest request, CancellationToken cancellationToken)
    {
        var customer = await _userRepository.Query()
            .FirstOrDefaultAsync(u => u.Id == customerId && u.Role == UserRole.Customer, cancellationToken);

        if (customer is null)
        {
            return NotFound(new { message = "Customer not found." });
        }

        customer.ApprovalStatus = request.Approve ? ApprovalStatus.Approved : ApprovalStatus.Rejected;
        _userRepository.Update(customer);
        await _userRepository.SaveChangesAsync(cancellationToken);
        return Ok(customer);
    }

    [HttpGet("officers")]
    public async Task<IActionResult> GetOfficers([FromQuery] ApprovalStatus? status, CancellationToken cancellationToken)
    {
        var officers = await _userRepository.GetOfficersAsync(status, cancellationToken);
        return Ok(officers);
    }

    [HttpPut("officers/{officerId:guid}/approval")]
    public async Task<IActionResult> SetOfficerApproval(Guid officerId, [FromBody] ApprovalRequest request, CancellationToken cancellationToken)
    {
        var officer = await _userRepository.Query()
            .FirstOrDefaultAsync(u => u.Id == officerId && (u.Role == UserRole.LoanOfficer || u.Role == UserRole.FieldOfficer), cancellationToken);

        if (officer is null)
        {
            return NotFound(new { message = "Officer not found." });
        }

        officer.ApprovalStatus = request.Approve ? ApprovalStatus.Approved : ApprovalStatus.Rejected;
        _userRepository.Update(officer);
        await _userRepository.SaveChangesAsync(cancellationToken);
        return Ok(officer);
    }

    [HttpGet("loan-requests")]
    public async Task<IActionResult> GetLoanRequests([FromQuery] LoanStatus? status, CancellationToken cancellationToken)
    {
        var query = _loanApplicationRepository.Query()
            .Include(l => l.Customer)
            .Include(l => l.AssignedOfficer)
            .Include(l => l.BackgroundVerification)
            .Include(l => l.LoanVerification)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(l => l.Status == status.Value);
        }

        var loans = await query
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(cancellationToken);

        return Ok(loans);
    }

    [HttpPost("loan-requests/background/assign")]
    public async Task<IActionResult> AssignBackgroundVerification([FromBody] AssignOfficerRequest request, CancellationToken cancellationToken)
    {
        var officer = await _userRepository.Query()
            .FirstOrDefaultAsync(u => u.Id == request.OfficerId && (u.Role == UserRole.FieldOfficer || u.Role == UserRole.LoanOfficer), cancellationToken);

        if (officer is null)
        {
            return BadRequest(new { message = "Officer not found or invalid role." });
        }

        var loan = await _loanApplicationRepository.Query()
            .Include(l => l.BackgroundVerification)
            .FirstOrDefaultAsync(l => l.Id == request.LoanApplicationId, cancellationToken);

        if (loan is null)
        {
            return NotFound(new { message = "Loan request not found." });
        }

        loan.AssignedOfficerId = request.OfficerId;

        if (loan.BackgroundVerification is null)
        {
            loan.BackgroundVerification = new BackgroundVerification
            {
                LoanApplicationId = loan.Id,
                OfficerId = request.OfficerId,
                Status = VerificationStatus.Assigned,
                Notes = "Background verification assigned."
            };
        }
        else
        {
            loan.BackgroundVerification.OfficerId = request.OfficerId;
            loan.BackgroundVerification.Status = VerificationStatus.Assigned;
        }

        _loanApplicationRepository.Update(loan);
        await _loanApplicationRepository.SaveChangesAsync(cancellationToken);
        return Ok(loan);
    }

    [HttpPost("loan-requests/verification/assign")]
    public async Task<IActionResult> AssignLoanVerification([FromBody] AssignOfficerRequest request, CancellationToken cancellationToken)
    {
        var officer = await _userRepository.Query()
            .FirstOrDefaultAsync(u => u.Id == request.OfficerId && (u.Role == UserRole.FieldOfficer || u.Role == UserRole.LoanOfficer), cancellationToken);

        if (officer is null)
        {
            return BadRequest(new { message = "Officer not found or invalid role." });
        }

        var loan = await _loanApplicationRepository.Query()
            .Include(l => l.LoanVerification)
            .FirstOrDefaultAsync(l => l.Id == request.LoanApplicationId, cancellationToken);

        if (loan is null)
        {
            return NotFound(new { message = "Loan request not found." });
        }

        loan.AssignedOfficerId = request.OfficerId;

        if (loan.LoanVerification is null)
        {
            loan.LoanVerification = new LoanVerification
            {
                LoanApplicationId = loan.Id,
                OfficerId = request.OfficerId,
                Status = VerificationStatus.Assigned,
                VerificationSummary = "Loan verification assigned."
            };
        }
        else
        {
            loan.LoanVerification.OfficerId = request.OfficerId;
            loan.LoanVerification.Status = VerificationStatus.Assigned;
        }

        _loanApplicationRepository.Update(loan);
        await _loanApplicationRepository.SaveChangesAsync(cancellationToken);
        return Ok(loan);
    }

    [HttpGet("background-verifications")]
    public async Task<IActionResult> GetBackgroundVerifications(CancellationToken cancellationToken)
    {
        var verifications = await _backgroundVerificationRepository.Query()
            .Include(b => b.LoanApplication).ThenInclude(l => l.Customer)
            .Include(b => b.Officer)
            .OrderByDescending(b => b.UpdatedAt ?? b.CreatedAt)
            .ToListAsync(cancellationToken);

        return Ok(verifications);
    }

    [HttpDelete("background-verifications/{id:guid}")]
    public async Task<IActionResult> DeleteBackgroundVerification(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _backgroundVerificationRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        _backgroundVerificationRepository.Remove(entity);
        await _backgroundVerificationRepository.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("loan-verifications")]
    public async Task<IActionResult> GetLoanVerifications(CancellationToken cancellationToken)
    {
        var verifications = await _loanVerificationRepository.Query()
            .Include(v => v.LoanApplication).ThenInclude(l => l.Customer)
            .Include(v => v.Officer)
            .OrderByDescending(v => v.UpdatedAt ?? v.CreatedAt)
            .ToListAsync(cancellationToken);

        return Ok(verifications);
    }

    [HttpDelete("loan-verifications/{id:guid}")]
    public async Task<IActionResult> DeleteLoanVerification(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _loanVerificationRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        _loanVerificationRepository.Remove(entity);
        await _loanVerificationRepository.SaveChangesAsync(cancellationToken);
        return NoContent();
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

    [HttpPut("help/{helpId:guid}")]
    public async Task<IActionResult> UpdateHelpReport(Guid helpId, [FromBody] HelpReportUpdateRequest request, CancellationToken cancellationToken)
    {
        var report = await _helpReportRepository.GetByIdAsync(helpId, cancellationToken);
        if (report is null)
        {
            return NotFound();
        }

        report.Title = request.Title;
        report.Message = request.Message;
        report.Status = request.Status;
        report.UpdatedAt = DateTime.UtcNow;

        _helpReportRepository.Update(report);
        await _helpReportRepository.SaveChangesAsync(cancellationToken);
        return Ok(report);
    }

    [HttpDelete("help/{helpId:guid}")]
    public async Task<IActionResult> DeleteHelpReport(Guid helpId, CancellationToken cancellationToken)
    {
        var report = await _helpReportRepository.GetByIdAsync(helpId, cancellationToken);
        if (report is null)
        {
            return NotFound();
        }

        _helpReportRepository.Remove(report);
        await _helpReportRepository.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("feedback/questions")]
    public async Task<IActionResult> GetFeedbackQuestions(CancellationToken cancellationToken)
    {
        var questions = await _feedbackQuestionRepository.Query()
            .Include(q => q.Responses)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync(cancellationToken);

        return Ok(questions);
    }

    [HttpPost("feedback/questions")]
    public async Task<IActionResult> AddFeedbackQuestion([FromBody] FeedbackQuestionRequest request, CancellationToken cancellationToken)
    {
        var question = new FeedbackQuestion
        {
            Question = request.Question,
            IsActive = request.IsActive
        };

        await _feedbackQuestionRepository.AddAsync(question, cancellationToken);
        await _feedbackQuestionRepository.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetFeedbackQuestions), question);
    }

    [HttpPut("feedback/questions/{questionId:guid}")]
    public async Task<IActionResult> UpdateFeedbackQuestion(Guid questionId, [FromBody] FeedbackQuestionRequest request, CancellationToken cancellationToken)
    {
        var question = await _feedbackQuestionRepository.GetByIdAsync(questionId, cancellationToken);
        if (question is null)
        {
            return NotFound();
        }

        question.Question = request.Question;
        question.IsActive = request.IsActive;
        question.UpdatedAt = DateTime.UtcNow;

        _feedbackQuestionRepository.Update(question);
        await _feedbackQuestionRepository.SaveChangesAsync(cancellationToken);
        return Ok(question);
    }

    [HttpDelete("feedback/questions/{questionId:guid}")]
    public async Task<IActionResult> DeleteFeedbackQuestion(Guid questionId, CancellationToken cancellationToken)
    {
        var question = await _feedbackQuestionRepository.GetByIdAsync(questionId, cancellationToken);
        if (question is null)
        {
            return NotFound();
        }

        _feedbackQuestionRepository.Remove(question);
        await _feedbackQuestionRepository.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("feedback/responses")]
    public async Task<IActionResult> GetFeedbackResponses(CancellationToken cancellationToken)
    {
        var responses = await _feedbackResponseRepository.Query()
            .Include(r => r.Customer)
            .Include(r => r.Question)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

        return Ok(responses);
    }
}

