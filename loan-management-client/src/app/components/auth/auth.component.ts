import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { UserRole } from '../../models/api.models';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.scss'
})
export class AuthComponent {
  isLogin = true;
  authForm: FormGroup;
  error = '';
  loading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.authForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      fullName: [''],
      phoneNumber: [''],
      role: [UserRole.Customer]
    });
  }

  toggleMode() {
    this.isLogin = !this.isLogin;
    this.error = '';
    this.authForm.reset({ role: UserRole.Customer });

    if (this.isLogin) {
      this.authForm.get('fullName')?.clearValidators();
      this.authForm.get('phoneNumber')?.clearValidators();
    } else {
      this.authForm.get('fullName')?.setValidators([Validators.required]);
      this.authForm.get('phoneNumber')?.setValidators([Validators.required]);
    }
    this.authForm.get('fullName')?.updateValueAndValidity();
    this.authForm.get('phoneNumber')?.updateValueAndValidity();
  }

  onSubmit() {
    if (this.authForm.invalid) return;

    this.loading = true;
    this.error = '';

    if (this.isLogin) {
      const { email, password } = this.authForm.value;
      this.authService.login({ email, password }).subscribe({
        next: (res) => {
          this.navigateBasedOnRole(res.role);
        },
        error: (err) => {
          this.error = err.error?.message || 'Login failed';
          this.loading = false;
        }
      });
    } else {
      const registerData = {
        ...this.authForm.value,
        role: Number(this.authForm.value.role)
      };
      this.authService.register(registerData).subscribe({
        next: () => {
          this.isLogin = true;
          this.loading = false;
          this.error = 'Registration successful! Please login.';
        },
        error: (err) => {
          this.error = err.error?.message || 'Registration failed';
          this.loading = false;
        }
      });
    }
  }

  private navigateBasedOnRole(role: UserRole) {
    switch (role) {
      case UserRole.Admin:
        this.router.navigate(['/admin']);
        break;
      case UserRole.Customer:
        this.router.navigate(['/customer']);
        break;
      case UserRole.LoanOfficer:
      case UserRole.FieldOfficer:
        this.router.navigate(['/officer']);
        break;
    }
  }
}
