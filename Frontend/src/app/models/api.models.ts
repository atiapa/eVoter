export interface LoginRequest {
  username: string;
  password: string;
  fingerprintTemplate?: string;
  rfidCardNumber?: string;
}

export interface LoginResponse {
  token: string;
  username: string;
  role: string;
  userId: number;
  voterId?: number;
}

export interface RegisterVoterRequest {
  username: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  nationalId: string;
  dateOfBirth: Date;
  address: string;
  phoneNumber: string;
}

export interface Election {
  id: number;
  title: string;
  description: string;
  startDate: Date;
  endDate: Date;
  status: string;
  allowMobileVoting: boolean;
  candidates: Candidate[];
}

export interface Candidate {
  id: number;
  name: string;
  party: string;
  description?: string;
  photoPath?: string;
}

export interface VoteRequest {
  electionId: number;
  candidateId: number;
  isFromMobile: boolean;
}

export interface BiometricRegistrationRequest {
  voterId: number;
  fingerprintTemplate?: string;
  rfidCardNumber?: string;
  authType: string;
}
