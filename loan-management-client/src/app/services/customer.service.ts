import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api.config';
import {
  FeedbackResponseModel,
  FeedbackResponseRequest,
  HelpReportModel,
  HelpReportRequest,
  LoanApplicationModel,
  LoanApplicationRequest
} from '../models/api.models';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private readonly baseUrl = `${API_CONFIG.baseUrl}/customer`;

  constructor(private http: HttpClient) { }

  applyForLoan(customerId: string, request: LoanApplicationRequest): Observable<LoanApplicationModel> {
    return this.http.post<LoanApplicationModel>(`${this.baseUrl}/${customerId}/loans`, request);
  }

  getLoanRequests(customerId: string): Observable<LoanApplicationModel[]> {
    return this.http.get<LoanApplicationModel[]>(`${this.baseUrl}/${customerId}/loans`);
  }

  getHelpReports(): Observable<HelpReportModel[]> {
    return this.http.get<HelpReportModel[]>(`${this.baseUrl}/help`);
  }

  createHelpReport(customerId: string, request: HelpReportRequest): Observable<HelpReportModel> {
    return this.http.post<HelpReportModel>(`${this.baseUrl}/${customerId}/help`, request);
  }

  addFeedback(customerId: string, request: FeedbackResponseRequest): Observable<FeedbackResponseModel> {
    return this.http.post<FeedbackResponseModel>(`${this.baseUrl}/${customerId}/feedback`, request);
  }
}
