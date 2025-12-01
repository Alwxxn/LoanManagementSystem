import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api.config';
import {
  ApplicationUserModel,
  ApprovalRequest,
  ApprovalStatus,
  AssignOfficerRequest,
  FeedbackQuestionModel,
  FeedbackQuestionRequest,
  FeedbackResponseModel,
  HelpReportModel,
  HelpReportUpdateRequest,
  LoanApplicationModel,
  LoanStatus,
  BackgroundVerificationModel,
  LoanVerificationModel
} from '../models/api.models';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private readonly baseUrl = `${API_CONFIG.baseUrl}/admin`;

  constructor(private http: HttpClient) { }

  getCustomers(status?: ApprovalStatus): Observable<ApplicationUserModel[]> {
    let params = new HttpParams();
    if (status !== undefined) {
      params = params.set('status', status.toString());
    }
    return this.http.get<ApplicationUserModel[]>(`${this.baseUrl}/customers`, { params });
  }

  setCustomerApproval(customerId: string, request: ApprovalRequest): Observable<ApplicationUserModel> {
    return this.http.put<ApplicationUserModel>(`${this.baseUrl}/customers/${customerId}/approval`, request);
  }

  getOfficers(status?: ApprovalStatus): Observable<ApplicationUserModel[]> {
    let params = new HttpParams();
    if (status !== undefined) {
      params = params.set('status', status.toString());
    }
    return this.http.get<ApplicationUserModel[]>(`${this.baseUrl}/officers`, { params });
  }

  setOfficerApproval(officerId: string, request: ApprovalRequest): Observable<ApplicationUserModel> {
    return this.http.put<ApplicationUserModel>(`${this.baseUrl}/officers/${officerId}/approval`, request);
  }

  getLoanRequests(status?: LoanStatus): Observable<LoanApplicationModel[]> {
    let params = new HttpParams();
    if (status !== undefined) {
      params = params.set('status', status.toString());
    }
    return this.http.get<LoanApplicationModel[]>(`${this.baseUrl}/loan-requests`, { params });
  }

  assignBackgroundVerification(request: AssignOfficerRequest): Observable<LoanApplicationModel> {
    return this.http.post<LoanApplicationModel>(`${this.baseUrl}/loan-requests/background/assign`, request);
  }

  assignLoanVerification(request: AssignOfficerRequest): Observable<LoanApplicationModel> {
    return this.http.post<LoanApplicationModel>(`${this.baseUrl}/loan-requests/verification/assign`, request);
  }

  getBackgroundVerifications(): Observable<BackgroundVerificationModel[]> {
    return this.http.get<BackgroundVerificationModel[]>(`${this.baseUrl}/background-verifications`);
  }

  deleteBackgroundVerification(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/background-verifications/${id}`);
  }

  getLoanVerifications(): Observable<LoanVerificationModel[]> {
    return this.http.get<LoanVerificationModel[]>(`${this.baseUrl}/loan-verifications`);
  }

  deleteLoanVerification(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/loan-verifications/${id}`);
  }

  getHelpReports(): Observable<HelpReportModel[]> {
    return this.http.get<HelpReportModel[]>(`${this.baseUrl}/help`);
  }

  updateHelpReport(helpId: string, request: HelpReportUpdateRequest): Observable<HelpReportModel> {
    return this.http.put<HelpReportModel>(`${this.baseUrl}/help/${helpId}`, request);
  }

  deleteHelpReport(helpId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/help/${helpId}`);
  }

  getFeedbackQuestions(): Observable<FeedbackQuestionModel[]> {
    return this.http.get<FeedbackQuestionModel[]>(`${this.baseUrl}/feedback/questions`);
  }

  addFeedbackQuestion(request: FeedbackQuestionRequest): Observable<FeedbackQuestionModel> {
    return this.http.post<FeedbackQuestionModel>(`${this.baseUrl}/feedback/questions`, request);
  }

  updateFeedbackQuestion(questionId: string, request: FeedbackQuestionRequest): Observable<FeedbackQuestionModel> {
    return this.http.put<FeedbackQuestionModel>(`${this.baseUrl}/feedback/questions/${questionId}`, request);
  }

  deleteFeedbackQuestion(questionId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/feedback/questions/${questionId}`);
  }

  getFeedbackResponses(): Observable<FeedbackResponseModel[]> {
    return this.http.get<FeedbackResponseModel[]>(`${this.baseUrl}/feedback/responses`);
  }
}
