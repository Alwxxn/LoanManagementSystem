using LoanManagementSystem.DTOs;
using LoanManagementSystem.Models;
using LoanManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoanManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OfficerController(
    IUserRepository userRepository,
    ILoanApplicationRepository loanApplicationRepository,
    IBackgroundVerificationRepository backgroundVerificationRepository,
    ILoanVerificationRepository loanVerificationRepository,
    IHelpReportRepository helpReportRepository) : ControllerBase
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ILoanApplicationRepository _loanApplicationRepository = loanApplicationRepository;
    private readonly IBackgroundVerificationRepository _backgroundVerificationRepository = backgroundVerificationRepository;
    private readonly ILoanVerificationRepository _loanVerificationRepository = loanVerificationRepository;
    private readonly IHelpReportRepository _helpReportRepository = helpReportRepository;

    [HttpGet("{officerId:guid}/loans")]
    public async Task<IActionResult> GetAssignedLoans(
        Guid officerId,
        [FromQuery] LoanStatus? status,
        CancellationToken cancellationToken)
    {
        var officer = await _userRepository.Query()
            .FirstOrDefaultAsync(u => u.Id == officerId && (u.Role == UserRole.LoanOfficer || u.Role == UserRole.FieldOfficer), cancellationToken);

        if (officer is null)
        {
            return NotFound(new { message = "Officer not found." });
        }

        var query = _loanApplicationRepository.Query()
            .Include(l => l.Customer)
            .Include(l => l.BackgroundVerification)
            .Include(l => l.LoanVerification)
            .Where(l => l.AssignedOfficerId == officerId);

        if (status.HasValue)
        {
            query = query.Where(l => l.Status == status);
        }

        var loans = await query
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(cancellationToken);

        return Ok(loans);
    }

    [HttpPut("{officerId:guid}/background-verifications")]
    public async Task<IActionResult> UpdateBackgroundVerification(
        Guid officerId,
        [FromBody] VerificationUpdateRequest request,
        CancellationToken cancellationToken)
    {
        var verification = await _backgroundVerificationRepository.Query()
            .Include(b => b.LoanApplication)
            .FirstOrDefaultAsync(b => b.LoanApplicationId == request.LoanApplicationId, cancellationToken);

        if (verification is null)
        {
            return NotFound(new { message = "Background verification not found." });
        }

        if (verification.OfficerId != officerId)
        {
            return Forbid();
        }

        verification.Notes = request.Notes;
        verification.Status = request.Status;
        verification.CompletedOn = request.Status is VerificationStatus.Completed or VerificationStatus.Failed
            ? DateTime.UtcNow
            : null;

        _backgroundVerificationRepository.Update(verification);

        if (verification.LoanApplication is { } loan && request.Status == VerificationStatus.Completed)
        {
            loan.Status = LoanStatus.UnderReview;
            _loanApplicationRepository.Update(loan);
        }

        await _backgroundVerificationRepository.SaveChangesAsync(cancellationToken);
        return Ok(verification);
    }

    [HttpPut("{officerId:guid}/loan-verifications")]
    public async Task<IActionResult> UpdateLoanVerification(
        Guid officerId,
        [FromBody] VerificationUpdateRequest request,
        CancellationToken cancellationToken)
    {
        var verification = await _loanVerificationRepository.Query()
            .Include(v => v.LoanApplication)
            .FirstOrDefaultAsync(v => v.LoanApplicationId == request.LoanApplicationId, cancellationToken);

        if (verification is null)
        {
            return NotFound(new { message = "Loan verification not found." });
        }

        if (verification.OfficerId != officerId)
        {
            return Forbid();
        }

        verification.VerificationSummary = request.Notes;
        verification.Status = request.Status;
        verification.CompletedOn = request.Status is VerificationStatus.Completed or VerificationStatus.Failed
            ? DateTime.UtcNow
            : null;

        _loanVerificationRepository.Update(verification);

        if (verification.LoanApplication is { } loan && request.Status == VerificationStatus.Completed)
        {
            loan.Status = LoanStatus.Approved;
            _loanApplicationRepository.Update(loan);
        }

        if (verification.LoanApplication is { } rejectedLoan && request.Status == VerificationStatus.Failed)
        {
            rejectedLoan.Status = LoanStatus.Rejected;
            _loanApplicationRepository.Update(rejectedLoan);
        }

        await _loanVerificationRepository.SaveChangesAsync(cancellationToken);
        return Ok(verification);
    }

    [HttpGet("help")]
    public async Task<IActionResult> ViewHelpReports(CancellationToken cancellationToken)
    {
        var reports = await _helpReportRepository.Query()
            .Include(h => h.CreatedBy)
            .OrderByDescending(h => h.UpdatedAt ?? h.CreatedAt)
            .ToListAsync(cancellationToken);

        return Ok(reports);
    }
}

