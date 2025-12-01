import { Routes } from '@angular/router';
import { AuthComponent } from './components/auth/auth.component';
import { AdminDashboardComponent } from './components/admin-dashboard/admin-dashboard.component';
import { CustomerDashboardComponent } from './components/customer-dashboard/customer-dashboard.component';
import { OfficerDashboardComponent } from './components/officer-dashboard/officer-dashboard.component';

export const routes: Routes = [
  { path: '', redirectTo: 'auth', pathMatch: 'full' },
  { path: 'auth', component: AuthComponent },
  { path: 'admin', component: AdminDashboardComponent },
  { path: 'customer', component: CustomerDashboardComponent },
  { path: 'officer', component: OfficerDashboardComponent },
  // Fallback route
  { path: '**', redirectTo: 'auth' }
];
