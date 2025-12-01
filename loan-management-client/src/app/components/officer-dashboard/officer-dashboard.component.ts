import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { OfficerService } from '../../services/officer.service';
import { AuthService } from '../../services/auth.service';
import { StateService } from '../../services/state.service';
import {
  LoanApplicationModel,
  LoanStatus,
  VerificationStatus,
  VerificationUpdateRequest
} from '../../models/api.models';

@Component({
  selector: 'app-officer-dashboard',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './officer-dashboard.component.html',
  styleUrl: './officer-dashboard.component.scss'
})
export class OfficerDashboardComponent implements OnInit {
  loans: LoanApplicationModel[] = [];
  selectedLoan: LoanApplicationModel | null = null;
  verificationForm: FormGroup;

  loading = false;
  error = '';
  success = '';

  LoanStatus = LoanStatus;
  VerificationStatus = VerificationStatus;

  constructor(
    private officerService: OfficerService,
    private authService: AuthService,
    private stateService: StateService,
    private fb: FormBuilder,
    private router: Router
  ) {
    this.verificationForm = this.fb.group({
      status: ['', Validators.required],
      notes: ['', Validators.required]
    });
  }

  ngOnInit() {
    if (!this.stateService.currentUser) {
      this.router.navigate(['/auth']);
      return;
    }
    this.loadLoans();
  }

  loadLoans() {
    const userId = this.stateService.currentUser?.userId;
    if (!userId) return;

    this.loading = true;
    this.officerService.getAssignedLoans(userId).subscribe({
      next: (data) => {
        this.loans = data;
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
      }
    });
  }

  selectLoan(loan: LoanApplicationModel) {
    this.selectedLoan = loan;
    this.verificationForm.reset();

    // Pre-fill if existing verification data
    if (this.isBackgroundVerification() && loan.backgroundVerification) {
      this.verificationForm.patchValue({
        status: loan.backgroundVerification.status,
        notes: loan.backgroundVerification.notes
      });
    } else if (this.isLoanVerification() && loan.loanVerification) {
      this.verificationForm.patchValue({
        status: loan.loanVerification.status,
        notes: loan.loanVerification.verificationSummary
      });
    }
  }

  updateVerification() {
    if (this.verificationForm.invalid || !this.selectedLoan) return;
    const userId = this.stateService.currentUser?.userId;
    if (!userId) return;

    this.loading = true;
    const request: VerificationUpdateRequest = {
      loanApplicationId: this.selectedLoan.id,
      status: Number(this.verificationForm.value.status),
      notes: this.verificationForm.value.notes
    };

    const update$: Observable<any> = this.isBackgroundVerification()
      ? this.officerService.updateBackgroundVerification(userId, request)
      : this.officerService.updateLoanVerification(userId, request);

    update$.subscribe({
      next: () => {
        this.success = 'Verification updated successfully!';
        this.selectedLoan = null;
        this.loadLoans();
        this.loading = false;
        setTimeout(() => this.success = '', 3000);
      },
      error: (err) => {
        this.error = err.error?.message || 'Failed to update verification';
        this.loading = false;
        setTimeout(() => this.error = '', 3000);
      }
    });
  }

  // Helper to determine if current officer is doing background or loan verification
  // This logic depends on how roles are assigned. Assuming role is available in user object.
  isBackgroundVerification(): boolean {
    // In a real app, we'd check the specific role or assignment type.
    // For now, let's assume if backgroundVerification is assigned to this user, it's that.
    const userId = this.stateService.currentUser?.userId;
    return this.selectedLoan?.backgroundVerification?.officerId === userId;
  }

  isLoanVerification(): boolean {
    const userId = this.stateService.currentUser?.userId;
    return this.selectedLoan?.loanVerification?.officerId === userId;
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/auth']);
  }
}
