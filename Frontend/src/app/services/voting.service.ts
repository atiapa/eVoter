import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Election, VoteRequest } from '../models/api.models';

@Injectable({
  providedIn: 'root'
})
export class VotingService {
  private apiUrl = 'http://localhost:5000/api/voting';

  constructor(private http: HttpClient) {}

  getActiveElections(): Observable<Election[]> {
    return this.http.get<Election[]>(`${this.apiUrl}/elections`);
  }

  getElection(id: number): Observable<Election> {
    return this.http.get<Election>(`${this.apiUrl}/elections/${id}`);
  }

  castVote(request: VoteRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/vote`, request);
  }

  hasVoted(electionId: number): Observable<boolean> {
    return this.http.get<boolean>(`${this.apiUrl}/has-voted/${electionId}`);
  }
}
