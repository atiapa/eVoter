import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { LoginRequest } from '../../models/api.models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="login-container">
      <div class="login-card">
        <h2>eVoter - Login</h2>
        
        <form (ngSubmit)="onLogin()">
          <div class="form-group">
            <label for="username">Username</label>
            <input 
              type="text" 
              id="username" 
              name="username" 
              [(ngModel)]="loginRequest.username" 
              required>
          </div>
          
          <div class="form-group">
            <label for="password">Password</label>
            <input 
              type="password" 
              id="password" 
              name="password" 
              [(ngModel)]="loginRequest.password" 
              required>
          </div>
          
          <div class="biometric-section">
            <h3>Biometric Authentication (Optional)</h3>
            
            <div class="form-group">
              <label for="fingerprint">Fingerprint Template</label>
              <input 
                type="text" 
                id="fingerprint" 
                name="fingerprint" 
                [(ngModel)]="loginRequest.fingerprintTemplate"
                placeholder="Scan fingerprint...">
            </div>
            
            <div class="form-group">
              <label for="rfid">RFID Card Number</label>
              <input 
                type="text" 
                id="rfid" 
                name="rfid" 
                [(ngModel)]="loginRequest.rfidCardNumber"
                placeholder="Scan RFID card...">
            </div>
          </div>
          
          <div *ngIf="errorMessage" class="error-message">
            {{ errorMessage }}
          </div>
          
          <button type="submit" class="btn-primary">Login</button>
        </form>
        
        <div class="register-link">
          <p>Don't have an account? <a routerLink="/register">Register here</a></p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .login-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: 100vh;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      padding: 20px;
    }
    
    .login-card {
      background: white;
      padding: 40px;
      border-radius: 10px;
      box-shadow: 0 10px 40px rgba(0,0,0,0.1);
      width: 100%;
      max-width: 500px;
    }
    
    h2 {
      text-align: center;
      color: #333;
      margin-bottom: 30px;
    }
    
    h3 {
      font-size: 16px;
      color: #666;
      margin-top: 20px;
      margin-bottom: 15px;
    }
    
    .form-group {
      margin-bottom: 20px;
    }
    
    label {
      display: block;
      margin-bottom: 5px;
      color: #555;
      font-weight: 500;
    }
    
    input {
      width: 100%;
      padding: 12px;
      border: 1px solid #ddd;
      border-radius: 5px;
      font-size: 14px;
      box-sizing: border-box;
    }
    
    input:focus {
      outline: none;
      border-color: #667eea;
    }
    
    .biometric-section {
      background: #f8f9fa;
      padding: 20px;
      border-radius: 5px;
      margin: 20px 0;
    }
    
    .error-message {
      background: #fee;
      color: #c33;
      padding: 10px;
      border-radius: 5px;
      margin-bottom: 20px;
      text-align: center;
    }
    
    .btn-primary {
      width: 100%;
      padding: 12px;
      background: #667eea;
      color: white;
      border: none;
      border-radius: 5px;
      font-size: 16px;
      font-weight: 600;
      cursor: pointer;
      transition: background 0.3s;
    }
    
    .btn-primary:hover {
      background: #5568d3;
    }
    
    .register-link {
      text-align: center;
      margin-top: 20px;
      color: #666;
    }
    
    .register-link a {
      color: #667eea;
      text-decoration: none;
      font-weight: 500;
    }
    
    .register-link a:hover {
      text-decoration: underline;
    }
  `]
})
export class LoginComponent {
  loginRequest: LoginRequest = {
    username: '',
    password: ''
  };
  errorMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onLogin(): void {
    this.errorMessage = '';
    
    this.authService.login(this.loginRequest).subscribe({
      next: (response) => {
        if (response.role === 'Admin') {
          this.router.navigate(['/admin']);
        } else {
          this.router.navigate(['/voting']);
        }
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Login failed. Please check your credentials.';
      }
    });
  }
}
