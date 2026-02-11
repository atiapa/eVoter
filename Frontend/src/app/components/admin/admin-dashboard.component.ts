import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../services/admin.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="admin-container">
      <header>
        <h1>Admin Dashboard</h1>
        <button (click)="logout()" class="btn-logout">Logout</button>
      </header>
      
      <div class="tabs">
        <button 
          [class.active]="activeTab === 'elections'" 
          (click)="activeTab = 'elections'; loadElections()">
          Elections
        </button>
        <button 
          [class.active]="activeTab === 'voters'" 
          (click)="activeTab = 'voters'; loadVoters()">
          Voters
        </button>
        <button 
          [class.active]="activeTab === 'create'" 
          (click)="activeTab = 'create'">
          Create Election
        </button>
      </div>
      
      <div class="content">
        <!-- Elections Tab -->
        <div *ngIf="activeTab === 'elections'" class="elections-section">
          <h2>All Elections</h2>
          <table *ngIf="elections.length > 0">
            <thead>
              <tr>
                <th>ID</th>
                <th>Title</th>
                <th>Status</th>
                <th>Start Date</th>
                <th>End Date</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let election of elections">
                <td>{{ election.id }}</td>
                <td>{{ election.title }}</td>
                <td>{{ election.status }}</td>
                <td>{{ election.startDate | date }}</td>
                <td>{{ election.endDate | date }}</td>
                <td>
                  <select (change)="updateElectionStatus(election.id, $event)" [value]="election.status">
                    <option value="Draft">Draft</option>
                    <option value="Active">Active</option>
                    <option value="Completed">Completed</option>
                    <option value="Cancelled">Cancelled</option>
                  </select>
                  <button (click)="viewResults(election.id)" class="btn-sm">Results</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        
        <!-- Voters Tab -->
        <div *ngIf="activeTab === 'voters'" class="voters-section">
          <h2>Registered Voters</h2>
          <table *ngIf="voters.length > 0">
            <thead>
              <tr>
                <th>ID</th>
                <th>Name</th>
                <th>National ID</th>
                <th>Email</th>
                <th>Verified</th>
                <th>Biometric</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let voter of voters">
                <td>{{ voter.id }}</td>
                <td>{{ voter.firstName }} {{ voter.lastName }}</td>
                <td>{{ voter.nationalId }}</td>
                <td>{{ voter.email }}</td>
                <td>{{ voter.isVerified ? 'Yes' : 'No' }}</td>
                <td>{{ voter.hasBiometric ? 'Yes' : 'No' }}</td>
                <td>
                  <button 
                    *ngIf="!voter.isVerified" 
                    (click)="verifyVoter(voter.id)" 
                    class="btn-sm btn-success">
                    Verify
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        
        <!-- Create Election Tab -->
        <div *ngIf="activeTab === 'create'" class="create-section">
          <h2>Create New Election</h2>
          <form (ngSubmit)="createElection()">
            <div class="form-group">
              <label>Title</label>
              <input type="text" [(ngModel)]="newElection.title" name="title" required>
            </div>
            
            <div class="form-group">
              <label>Description</label>
              <textarea [(ngModel)]="newElection.description" name="description" rows="3"></textarea>
            </div>
            
            <div class="form-group">
              <label>Start Date</label>
              <input type="datetime-local" [(ngModel)]="newElection.startDate" name="startDate" required>
            </div>
            
            <div class="form-group">
              <label>End Date</label>
              <input type="datetime-local" [(ngModel)]="newElection.endDate" name="endDate" required>
            </div>
            
            <div class="form-group">
              <label>
                <input type="checkbox" [(ngModel)]="newElection.allowMobileVoting" name="allowMobile">
                Allow Mobile Voting
              </label>
            </div>
            
            <h3>Candidates</h3>
            <div *ngFor="let candidate of newElection.candidates; let i = index" class="candidate-entry">
              <input type="text" [(ngModel)]="candidate.name" [name]="'candidateName' + i" placeholder="Name" required>
              <input type="text" [(ngModel)]="candidate.party" [name]="'candidateParty' + i" placeholder="Party" required>
              <button type="button" (click)="removeCandidate(i)" class="btn-danger-sm">Remove</button>
            </div>
            
            <button type="button" (click)="addCandidate()" class="btn-secondary">Add Candidate</button>
            <button type="submit" class="btn-primary">Create Election</button>
          </form>
        </div>
        
        <div *ngIf="message" [class]="messageClass">
          {{ message }}
        </div>
      </div>
    </div>
  `,
  styles: [`
    .admin-container {
      max-width: 1400px;
      margin: 0 auto;
      padding: 20px;
    }
    
    header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 30px;
    }
    
    .tabs {
      display: flex;
      gap: 10px;
      margin-bottom: 30px;
    }
    
    .tabs button {
      padding: 12px 24px;
      background: #f8f9fa;
      border: 2px solid #dee2e6;
      border-radius: 5px;
      cursor: pointer;
      font-weight: 600;
      transition: all 0.3s;
    }
    
    .tabs button.active {
      background: #667eea;
      color: white;
      border-color: #667eea;
    }
    
    .content {
      background: white;
      padding: 30px;
      border-radius: 10px;
      box-shadow: 0 2px 10px rgba(0,0,0,0.1);
    }
    
    table {
      width: 100%;
      border-collapse: collapse;
      margin-top: 20px;
    }
    
    th, td {
      padding: 12px;
      text-align: left;
      border-bottom: 1px solid #dee2e6;
    }
    
    th {
      background: #f8f9fa;
      font-weight: 600;
    }
    
    .form-group {
      margin-bottom: 20px;
    }
    
    label {
      display: block;
      margin-bottom: 5px;
      font-weight: 500;
    }
    
    input, textarea, select {
      width: 100%;
      padding: 10px;
      border: 1px solid #ddd;
      border-radius: 5px;
      font-size: 14px;
    }
    
    .candidate-entry {
      display: flex;
      gap: 10px;
      margin-bottom: 10px;
    }
    
    .candidate-entry input {
      flex: 1;
    }
    
    .btn-primary, .btn-secondary {
      padding: 12px 24px;
      border: none;
      border-radius: 5px;
      cursor: pointer;
      font-weight: 600;
      margin-right: 10px;
      margin-top: 10px;
    }
    
    .btn-primary {
      background: #667eea;
      color: white;
    }
    
    .btn-secondary {
      background: #6c757d;
      color: white;
    }
    
    .btn-sm {
      padding: 6px 12px;
      font-size: 12px;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      margin-right: 5px;
    }
    
    .btn-success {
      background: #28a745;
      color: white;
    }
    
    .btn-danger-sm {
      padding: 6px 12px;
      background: #dc3545;
      color: white;
      border: none;
      border-radius: 4px;
      cursor: pointer;
    }
    
    .btn-logout {
      padding: 10px 20px;
      background: #dc3545;
      color: white;
      border: none;
      border-radius: 5px;
      cursor: pointer;
    }
    
    .success-message {
      background: #d4edda;
      color: #155724;
      padding: 15px;
      border-radius: 5px;
      margin-top: 20px;
    }
    
    .error-message {
      background: #f8d7da;
      color: #721c24;
      padding: 15px;
      border-radius: 5px;
      margin-top: 20px;
    }
  `]
})
export class AdminDashboardComponent implements OnInit {
  activeTab = 'elections';
  elections: any[] = [];
  voters: any[] = [];
  message = '';
  messageClass = '';
  
  newElection = {
    title: '',
    description: '',
    startDate: '',
    endDate: '',
    allowMobileVoting: true,
    candidates: [
      { name: '', party: '', description: '' }
    ]
  };

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.loadElections();
  }

  loadElections(): void {
    this.adminService.getAllElections().subscribe({
      next: (data) => this.elections = data,
      error: (error) => console.error('Error loading elections:', error)
    });
  }

  loadVoters(): void {
    this.adminService.getAllVoters().subscribe({
      next: (data) => this.voters = data,
      error: (error) => console.error('Error loading voters:', error)
    });
  }

  updateElectionStatus(id: number, event: any): void {
    const status = event.target.value;
    this.adminService.updateElectionStatus(id, status).subscribe({
      next: () => {
        this.message = 'Election status updated successfully';
        this.messageClass = 'success-message';
        setTimeout(() => this.message = '', 3000);
      },
      error: (error) => {
        this.message = 'Failed to update election status';
        this.messageClass = 'error-message';
      }
    });
  }

  verifyVoter(id: number): void {
    this.adminService.verifyVoter(id).subscribe({
      next: () => {
        this.message = 'Voter verified successfully';
        this.messageClass = 'success-message';
        this.loadVoters();
        setTimeout(() => this.message = '', 3000);
      },
      error: (error) => {
        this.message = 'Failed to verify voter';
        this.messageClass = 'error-message';
      }
    });
  }

  addCandidate(): void {
    this.newElection.candidates.push({ name: '', party: '', description: '' });
  }

  removeCandidate(index: number): void {
    this.newElection.candidates.splice(index, 1);
  }

  createElection(): void {
    this.adminService.createElection(this.newElection).subscribe({
      next: () => {
        this.message = 'Election created successfully';
        this.messageClass = 'success-message';
        this.resetForm();
        this.activeTab = 'elections';
        this.loadElections();
      },
      error: (error) => {
        this.message = 'Failed to create election';
        this.messageClass = 'error-message';
      }
    });
  }

  resetForm(): void {
    this.newElection = {
      title: '',
      description: '',
      startDate: '',
      endDate: '',
      allowMobileVoting: true,
      candidates: [{ name: '', party: '', description: '' }]
    };
  }

  viewResults(id: number): void {
    this.adminService.getElectionResults(id).subscribe({
      next: (results) => {
        alert(JSON.stringify(results, null, 2));
      }
    });
  }

  logout(): void {
    // Will be implemented with router
  }
}
