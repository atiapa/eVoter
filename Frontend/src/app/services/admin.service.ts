import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private apiUrl = 'http://localhost:5000/api/admin';

  constructor(private http: HttpClient) {}

  getAllElections(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/elections`);
  }

  createElection(election: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/elections`, election);
  }

  updateElectionStatus(id: number, status: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/elections/${id}/status`, JSON.stringify(status), {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  getAllVoters(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/voters`);
  }

  verifyVoter(id: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/voters/${id}/verify`, {});
  }

  getElectionResults(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/elections/${id}/results`);
  }
}
