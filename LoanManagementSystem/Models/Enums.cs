namespace LoanManagementSystem.Models;

public enum UserRole
{
    Admin = 1,
    Customer = 2,
    LoanOfficer = 3,
    FieldOfficer = 4
}

public enum ApprovalStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}

public enum LoanStatus
{
    Draft = 0,
    Submitted = 1,
    UnderReview = 2,
    Approved = 3,
    Rejected = 4
}

public enum VerificationStatus
{
    Pending = 0,
    Assigned = 1,
    InProgress = 2,
    Completed = 3,
    Failed = 4
}

public enum HelpStatus
{
    Open = 0,
    InProgress = 1,
    Closed = 2
}

