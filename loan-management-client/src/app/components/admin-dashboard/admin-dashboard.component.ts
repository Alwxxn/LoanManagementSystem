import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AdminService } from '../../services/admin.service';
import { AuthService } from '../../services/auth.service';
import { StateService } from '../../services/state.service';
import {
  ApplicationUserModel,
  ApprovalStatus,
  FeedbackQuestionModel,
  HelpReportModel,
  HelpStatus,
  LoanApplicationModel,
  LoanStatus,
  UserRole
} from '../../models/api.models';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.scss'
})
export class AdminDashboardComponent implements OnInit {
  activeTab: 'customers' | 'officers' | 'loans' | 'help' | 'feedback' = 'customers';

  customers: ApplicationUserModel[] = [];
  officers: ApplicationUserModel[] = [];
  loans: LoanApplicationModel[] = [];
  helpReports: HelpReportModel[] = [];
  feedbackQuestions: FeedbackQuestionModel[] = [];

  feedbackForm: FormGroup;
  assignForm: FormGroup;

  loading = false;
  error = '';
  success = '';

  ApprovalStatus = ApprovalStatus;
  LoanStatus = LoanStatus;
  HelpStatus = HelpStatus;
  UserRole = UserRole;

  selectedLoan: LoanApplicationModel | null = null;

  constructor(
    private adminService: AdminService,
    private authService: AuthService,
    private stateService: StateService,
    private fb: FormBuilder,
    private router: Router
  ) {
    this.feedbackForm = this.fb.group({
      question: ['', Validators.required],
      isActive: [true]
    });

    this.assignForm = this.fb.group({
      officerId: ['', Validators.required]
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
    this.loading = true;
    switch (this.activeTab) {
      case 'customers':
        this.adminService.getCustomers().subscribe(data => {
          this.customers = data;
          this.loading = false;
        });
        break;
      case 'officers':
        this.adminService.getOfficers().subscribe(data => {
          this.officers = data;
          this.loading = false;
        });
        break;
      case 'loans':
        this.adminService.getLoanRequests().subscribe(data => {
          this.loans = data;
          // Also load officers for assignment dropdown
          this.adminService.getOfficers(ApprovalStatus.Approved).subscribe(officers => {
            this.officers = officers;
            this.loading = false;
          });
        });
        break;
      case 'help':
        this.adminService.getHelpReports().subscribe(data => {
          this.helpReports = data;
          this.loading = false;
        });
        break;
      case 'feedback':
        this.adminService.getFeedbackQuestions().subscribe(data => {
          this.feedbackQuestions = data;
          this.loading = false;
        });
        break;
    }
  }

  switchTab(tab: any) {
    this.activeTab = tab;
    this.loadData();
  }

  approveUser(user: ApplicationUserModel, approve: boolean) {
    const action = user.role === UserRole.Customer
      ? this.adminService.setCustomerApproval(user.id, { approve })
      : this.adminService.setOfficerApproval(user.id, { approve });

    action.subscribe({
      next: () => {
        this.success = `User ${approve ? 'approved' : 'rejected'} successfully`;
        this.loadData();
        setTimeout(() => this.success = '', 3000);
      },
      error: (err) => this.error = err.error?.message || 'Action failed'
    });
  }

  selectLoanForAssignment(loan: LoanApplicationModel) {
    this.selectedLoan = loan;
    this.assignForm.reset();
  }

  assignOfficer() {
    if (this.assignForm.invalid || !this.selectedLoan) return;

    const request = {
      loanApplicationId: this.selectedLoan.id,
      officerId: this.assignForm.value.officerId
    };

    // Determine if we are assigning for background or loan verification
    // Logic: If background is pending/null, assign that. If background done, assign loan verification.
    // For simplicity, let's assume we assign both or based on status.
    // Actually, the backend has separate endpoints.
    // Let's assume we assign background verification first.

    const assign$ = !this.selectedLoan.backgroundVerification || this.selectedLoan.backgroundVerification.status === 0 // Pending
      ? this.adminService.assignBackgroundVerification(request)
      : this.adminService.assignLoanVerification(request);

    assign$.subscribe({
      next: () => {
        this.success = 'Officer assigned successfully';
        this.selectedLoan = null;
        this.loadData();
        setTimeout(() => this.success = '', 3000);
      },
      error: (err) => this.error = err.error?.message || 'Assignment failed'
    });
  }

  addFeedbackQuestion() {
    if (this.feedbackForm.invalid) return;

    this.adminService.addFeedbackQuestion(this.feedbackForm.value).subscribe({
      next: () => {
        this.success = 'Question added successfully';
        this.feedbackForm.reset({ isActive: true });
        this.loadData();
        setTimeout(() => this.success = '', 3000);
      },
      error: (err) => this.error = err.error?.message || 'Failed to add question'
    });
  }

  deleteQuestion(id: string) {
    if (!confirm('Are you sure?')) return;

    this.adminService.deleteFeedbackQuestion(id).subscribe({
      next: () => {
        this.loadData();
      },
      error: (err) => this.error = err.error?.message || 'Failed to delete question'
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/auth']);
  }
}
