import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { API_CONFIG } from '../config/api.config';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/api.models';
import { StateService } from './state.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly baseUrl = `${API_CONFIG.baseUrl}/auth`;

  constructor(private http: HttpClient, private stateService: StateService) { }

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/register`, request);
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/login`, request).pipe(
      tap(response => this.stateService.setUser(response))
    );
  }

  logout(): void {
    this.stateService.clearUser();
  }
}
