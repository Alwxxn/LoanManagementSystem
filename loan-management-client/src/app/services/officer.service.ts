import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api.config';
import {
  BackgroundVerificationModel,
  HelpReportModel,
  LoanApplicationModel,
  LoanStatus,
  LoanVerificationModel,
  VerificationUpdateRequest
} from '../models/api.models';

@Injectable({
  providedIn: 'root'
})
export class OfficerService {
  private readonly baseUrl = `${API_CONFIG.baseUrl}/officer`;

  constructor(private http: HttpClient) { }

  getAssignedLoans(officerId: string, status?: LoanStatus): Observable<LoanApplicationModel[]> {
    let params = new HttpParams();
    if (status !== undefined) {
      params = params.set('status', status.toString());
    }
    return this.http.get<LoanApplicationModel[]>(`${this.baseUrl}/${officerId}/loans`, { params });
  }

  updateBackgroundVerification(officerId: string, request: VerificationUpdateRequest): Observable<BackgroundVerificationModel> {
    return this.http.put<BackgroundVerificationModel>(`${this.baseUrl}/${officerId}/background-verifications`, request);
  }

  updateLoanVerification(officerId: string, request: VerificationUpdateRequest): Observable<LoanVerificationModel> {
    return this.http.put<LoanVerificationModel>(`${this.baseUrl}/${officerId}/loan-verifications`, request);
  }

  viewHelpReports(): Observable<HelpReportModel[]> {
    return this.http.get<HelpReportModel[]>(`${this.baseUrl}/help`);
  }
}
