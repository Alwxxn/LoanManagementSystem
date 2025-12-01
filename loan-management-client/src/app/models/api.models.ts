export enum UserRole {
  Admin = 1,
  Customer = 2,
  LoanOfficer = 3,
  FieldOfficer = 4,
}

export enum ApprovalStatus {
  Pending = 0,
  Approved = 1,
  Rejected = 2,
}

export enum LoanStatus {
  Draft = 0,
  Submitted = 1,
  UnderReview = 2,
  Approved = 3,
  Rejected = 4,
}

export enum VerificationStatus {
  Pending = 0,
  Assigned = 1,
  InProgress = 2,
  Completed = 3,
  Failed = 4,
}

export enum HelpStatus {
  Open = 0,
  InProgress = 1,
  Closed = 2,
}

export interface AuthResponse {
  userId: string;
  fullName: string;
  email: string;
  role: UserRole;
  approvalStatus: ApprovalStatus;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
  phoneNumber: string;
  role: UserRole;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoanApplicationRequest {
  amount: number;
  tenureMonths: number;
  loanType: string;
  purpose: string;
}

export interface AssignOfficerRequest {
  loanApplicationId: string;
  officerId: string;
}

export interface VerificationUpdateRequest {
  loanApplicationId: string;
  notes: string;
  status: VerificationStatus;
}

export interface HelpReportRequest {
  title: string;
  message: string;
  status?: HelpStatus;
}

export interface HelpReportUpdateRequest extends HelpReportRequest {
  status: HelpStatus;
}

export interface FeedbackQuestionRequest {
  question: string;
  isActive: boolean;
}

export interface FeedbackResponseRequest {
  questionId: string;
  answer: string;
}

export interface ApprovalRequest {
  approve: boolean;
}

export interface ApplicationUserModel {
  id: string;
  fullName: string;
  email: string;
  phoneNumber: string;
  role: UserRole;
  approvalStatus: ApprovalStatus;
  isActive: boolean;
  createdAt?: string;
}

export interface LoanApplicationModel {
  id: string;
  customerId: string;
  assignedOfficerId?: string;
  amount: number;
  tenureMonths: number;
  loanType: string;
  purpose: string;
  status: LoanStatus;
  createdAt?: string;
  customer?: ApplicationUserModel;
  assignedOfficer?: ApplicationUserModel;
  backgroundVerification?: BackgroundVerificationModel;
  loanVerification?: LoanVerificationModel;
}

export interface BackgroundVerificationModel {
  id: string;
  loanApplicationId: string;
  officerId?: string;
  notes: string;
  status: VerificationStatus;
  completedOn?: string;
  loanApplication?: LoanApplicationModel;
  officer?: ApplicationUserModel;
}

export interface LoanVerificationModel {
  id: string;
  loanApplicationId: string;
  officerId?: string;
  verificationSummary: string;
  status: VerificationStatus;
  completedOn?: string;
  loanApplication?: LoanApplicationModel;
  officer?: ApplicationUserModel;
}

export interface HelpReportModel {
  id: string;
  title: string;
  message: string;
  status: HelpStatus;
  createdById: string;
  updatedById?: string;
  createdBy?: ApplicationUserModel;
  createdAt?: string;
  updatedAt?: string;
}

export interface FeedbackQuestionModel {
  id: string;
  question: string;
  isActive: boolean;
  createdAt?: string;
}

export interface FeedbackResponseModel {
  id: string;
  questionId: string;
  customerId: string;
  answer: string;
  createdAt?: string;
  question?: FeedbackQuestionModel;
  customer?: ApplicationUserModel;
}


