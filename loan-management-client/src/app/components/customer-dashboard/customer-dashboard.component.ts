import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CustomerService } from '../../services/customer.service';
import { AuthService } from '../../services/auth.service';
import { StateService } from '../../services/state.service';
import {
  HelpReportModel,
  LoanApplicationModel,
  LoanStatus,
  VerificationStatus
} from '../../models/api.models';

@Component({
  selector: 'app-customer-dashboard',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './customer-dashboard.component.html',
  styleUrl: './customer-dashboard.component.scss'
})
export class CustomerDashboardComponent implements OnInit {
  activeTab: 'loans' | 'help' | 'feedback' = 'loans';
  loans: LoanApplicationModel[] = [];
  helpReports: HelpReportModel[] = [];

  loanForm: FormGroup;
  helpForm: FormGroup;
  feedbackForm: FormGroup;

  loading = false;
  error = '';
  success = '';

  LoanStatus = LoanStatus;
  VerificationStatus = VerificationStatus;

  constructor(
    private customerService: CustomerService,
    private authService: AuthService,
    private stateService: StateService,
    private fb: FormBuilder,
    private router: Router
  ) {
    this.loanForm = this.fb.group({
      amount: ['', [Validators.required, Validators.min(1000)]],
      tenureMonths: ['', [Validators.required, Validators.min(1), Validators.max(360)]],
      loanType: ['', Validators.required],
      purpose: ['', Validators.required]
    });

    this.helpForm = this.fb.group({
      title: ['', Validators.required],
      message: ['', Validators.required]
    });

    this.feedbackForm = this.fb.group({
      questionId: ['', Validators.required],
      answer: ['', Validators.required]
    });
  }

  ngOnInit() {
    if (!this.stateService.currentUser) {
      this.router.navigate(['/auth']);
      return;
    }
    this.loadData();
  }

  loadData() {
    const userId = this.stateService.currentUser?.userId;
    if (!userId) return;

    this.loading = true;
    this.customerService.getLoanRequests(userId).subscribe({
      next: (data) => {
        this.loans = data;
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
      }
    });

    this.customerService.getHelpReports().subscribe({
      next: (data) => {
        this.helpReports = data;
      },
      error: (err) => console.error(err)
    });
  }

  get isApproved(): boolean {
    return this.stateService.currentUser?.approvalStatus === 1; // ApprovalStatus.Approved
  }

  applyForLoan() {
    if (this.loanForm.invalid || !this.isApproved) return;
    const userId = this.stateService.currentUser?.userId;
    if (!userId) return;

    this.loading = true;
    this.customerService.applyForLoan(userId, this.loanForm.value).subscribe({
      next: () => {
        this.success = 'Loan application submitted successfully!';
        this.loanForm.reset();
        this.loadData();
        this.loading = false;
        setTimeout(() => this.success = '', 3000);
      },
      error: (err) => {
        this.error = err.error?.message || 'Failed to submit loan application';
        this.loading = false;
        setTimeout(() => this.error = '', 3000);
      }
    });
  }

  createHelpReport() {
    if (this.helpForm.invalid) return;
    const userId = this.stateService.currentUser?.userId;
    if (!userId) return;

    this.loading = true;
    this.customerService.createHelpReport(userId, this.helpForm.value).subscribe({
      next: () => {
        this.success = 'Help report created successfully!';
        this.helpForm.reset();
        this.loadData();
        this.loading = false;
        setTimeout(() => this.success = '', 3000);
      },
      error: (err) => {
        this.error = err.error?.message || 'Failed to create help report';
        this.loading = false;
        setTimeout(() => this.error = '', 3000);
      }
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/auth']);
  }
}
