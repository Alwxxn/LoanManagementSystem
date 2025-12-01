using LoanManagementSystem.Data;
using LoanManagementSystem.Models;

namespace LoanManagementSystem.Repositories;

public interface ILoanApplicationRepository : IRepository<LoanApplication> { }

public interface IBackgroundVerificationRepository : IRepository<BackgroundVerification> { }

public interface ILoanVerificationRepository : IRepository<LoanVerification> { }

public interface IHelpReportRepository : IRepository<HelpReport> { }

public interface IFeedbackQuestionRepository : IRepository<FeedbackQuestion> { }

public interface IFeedbackResponseRepository : IRepository<FeedbackResponse> { }

public class LoanApplicationRepository(LoanManagementContext context) : Repository<LoanApplication>(context), ILoanApplicationRepository;

public class BackgroundVerificationRepository(LoanManagementContext context) : Repository<BackgroundVerification>(context), IBackgroundVerificationRepository;

public class LoanVerificationRepository(LoanManagementContext context) : Repository<LoanVerification>(context), ILoanVerificationRepository;

public class HelpReportRepository(LoanManagementContext context) : Repository<HelpReport>(context), IHelpReportRepository;

public class FeedbackQuestionRepository(LoanManagementContext context) : Repository<FeedbackQuestion>(context), IFeedbackQuestionRepository;

public class FeedbackResponseRepository(LoanManagementContext context) : Repository<FeedbackResponse>(context), IFeedbackResponseRepository;


