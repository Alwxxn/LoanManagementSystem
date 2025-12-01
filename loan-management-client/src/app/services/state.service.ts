import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { AuthResponse } from '../models/api.models';

const STORAGE_KEY = 'loan-ms-user';

@Injectable({
  providedIn: 'root',
})
export class StateService {
  private readonly userSubject = new BehaviorSubject<AuthResponse | null>(this.loadFromStorage());
  readonly user$ = this.userSubject.asObservable();

  get currentUser(): AuthResponse | null {
    return this.userSubject.value;
  }

  setUser(user: AuthResponse): void {
    this.userSubject.next(user);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(user));
  }

  clearUser(): void {
    this.userSubject.next(null);
    localStorage.removeItem(STORAGE_KEY);
  }

  private loadFromStorage(): AuthResponse | null {
    const stored = localStorage.getItem(STORAGE_KEY);
    if (!stored) {
      return null;
    }
    try {
      return JSON.parse(stored) as AuthResponse;
    } catch {
      localStorage.removeItem(STORAGE_KEY);
      return null;
    }
  }
}


