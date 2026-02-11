import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VotingService } from '../../services/voting.service';
import { Election, Candidate } from '../../models/api.models';

@Component({
  selector: 'app-voting',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="voting-container">
      <header>
        <h1>Active Elections</h1>
        <button (click)="logout()" class="btn-logout">Logout</button>
      </header>
      
      <div *ngIf="elections.length === 0" class="no-elections">
        <p>No active elections at this time.</p>
      </div>
      
      <div class="elections-grid">
        <div *ngFor="let election of elections" class="election-card">
          <h2>{{ election.title }}</h2>
          <p class="description">{{ election.description }}</p>
          <p class="dates">
            <strong>Period:</strong> {{ election.startDate | date }} - {{ election.endDate | date }}
          </p>
          
          <div class="candidates">
            <h3>Candidates</h3>
            <div *ngFor="let candidate of election.candidates" class="candidate-item">
              <div class="candidate-info">
                <h4>{{ candidate.name }}</h4>
                <p>{{ candidate.party }}</p>
                <p *ngIf="candidate.description" class="candidate-desc">{{ candidate.description }}</p>
              </div>
              <button 
                (click)="vote(election.id, candidate.id)" 
                class="btn-vote"
                [disabled]="hasVotedMap[election.id]">
                {{ hasVotedMap[election.id] ? 'Already Voted' : 'Vote' }}
              </button>
            </div>
          </div>
        </div>
      </div>
      
      <div *ngIf="message" [class]="messageClass">
        {{ message }}
      </div>
    </div>
  `,
  styles: [`
    .voting-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 20px;
    }
    
    header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 30px;
    }
    
    h1 {
      color: #333;
    }
    
    .btn-logout {
      padding: 10px 20px;
      background: #dc3545;
      color: white;
      border: none;
      border-radius: 5px;
      cursor: pointer;
    }
    
    .btn-logout:hover {
      background: #c82333;
    }
    
    .no-elections {
      text-align: center;
      padding: 60px 20px;
      background: #f8f9fa;
      border-radius: 10px;
    }
    
    .elections-grid {
      display: grid;
      gap: 30px;
    }
    
    .election-card {
      background: white;
      padding: 30px;
      border-radius: 10px;
      box-shadow: 0 2px 10px rgba(0,0,0,0.1);
    }
    
    .election-card h2 {
      color: #667eea;
      margin-bottom: 15px;
    }
    
    .description {
      color: #666;
      margin-bottom: 15px;
    }
    
    .dates {
      color: #888;
      font-size: 14px;
      margin-bottom: 25px;
    }
    
    .candidates h3 {
      color: #333;
      margin-bottom: 20px;
      border-bottom: 2px solid #667eea;
      padding-bottom: 10px;
    }
    
    .candidate-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 20px;
      margin-bottom: 15px;
      background: #f8f9fa;
      border-radius: 8px;
      transition: transform 0.2s;
    }
    
    .candidate-item:hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(0,0,0,0.1);
    }
    
    .candidate-info h4 {
      margin: 0 0 5px 0;
      color: #333;
    }
    
    .candidate-info p {
      margin: 0;
      color: #666;
      font-size: 14px;
    }
    
    .candidate-desc {
      margin-top: 10px !important;
      font-style: italic;
    }
    
    .btn-vote {
      padding: 10px 30px;
      background: #28a745;
      color: white;
      border: none;
      border-radius: 5px;
      cursor: pointer;
      font-weight: 600;
      transition: background 0.3s;
    }
    
    .btn-vote:hover:not(:disabled) {
      background: #218838;
    }
    
    .btn-vote:disabled {
      background: #6c757d;
      cursor: not-allowed;
    }
    
    .success-message {
      background: #d4edda;
      color: #155724;
      padding: 15px;
      border-radius: 5px;
      margin-top: 20px;
      text-align: center;
    }
    
    .error-message {
      background: #f8d7da;
      color: #721c24;
      padding: 15px;
      border-radius: 5px;
      margin-top: 20px;
      text-align: center;
    }
  `]
})
export class VotingComponent implements OnInit {
  elections: Election[] = [];
  hasVotedMap: { [key: number]: boolean } = {};
  message = '';
  messageClass = '';

  constructor(private votingService: VotingService) {}

  ngOnInit(): void {
    this.loadElections();
  }

  loadElections(): void {
    this.votingService.getActiveElections().subscribe({
      next: (elections) => {
        this.elections = elections;
        elections.forEach(election => {
          this.checkIfVoted(election.id);
        });
      },
      error: (error) => {
        console.error('Error loading elections:', error);
      }
    });
  }

  checkIfVoted(electionId: number): void {
    this.votingService.hasVoted(electionId).subscribe({
      next: (hasVoted) => {
        this.hasVotedMap[electionId] = hasVoted;
      }
    });
  }

  vote(electionId: number, candidateId: number): void {
    const request = {
      electionId,
      candidateId,
      isFromMobile: false
    };

    this.votingService.castVote(request).subscribe({
      next: () => {
        this.message = 'Vote cast successfully!';
        this.messageClass = 'success-message';
        this.hasVotedMap[electionId] = true;
        setTimeout(() => this.message = '', 3000);
      },
      error: (error) => {
        this.message = error.error?.message || 'Failed to cast vote';
        this.messageClass = 'error-message';
        setTimeout(() => this.message = '', 3000);
      }
    });
  }

  logout(): void {
    // Will be implemented with router
  }
}
