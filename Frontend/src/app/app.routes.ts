import { Routes } from '@angular/router';
import { LoginComponent } from './components/auth/login.component';
import { VotingComponent } from './components/voting/voting.component';
import { AdminDashboardComponent } from './components/admin/admin-dashboard.component';
import { authGuard, adminGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'voting', component: VotingComponent, canActivate: [authGuard] },
  { path: 'admin', component: AdminDashboardComponent, canActivate: [adminGuard] },
];
