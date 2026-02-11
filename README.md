# eVoter - Electronic Voting System

A secure, scalable, and user-friendly electronic voting system built with ASP.NET Core Web API, Entity Framework, MS SQL Server, Angular, and Capacitor.

## Features

### Backend (ASP.NET Core Web API)
- **Secure Authentication**: JWT-based authentication with support for:
  - Username/Password login
  - Biometric fingerprint authentication
  - RFID card authentication
- **Role-Based Authorization**: Admin and Voter roles
- **RESTful API**: Complete API for voting operations
- **Entity Framework Core**: Database ORM with MS SQL Server
- **Audit Logging**: Track all system activities

### Frontend (Angular + Capacitor)
- **Responsive Design**: Works on desktop and mobile devices
- **Admin Dashboard**: 
  - Manage elections
  - View and verify voters
  - Access election results
- **Voter Interface**:
  - View active elections
  - Cast votes securely
  - One vote per election enforcement
- **Mobile Support**: Capacitor integration for native mobile apps

### Security Features
- Password hashing with bcrypt
- JWT token authentication
- HTTPS support
- Vote encryption and hashing
- Audit trail for all operations
- One vote per election constraint

## Technology Stack

### Backend
- ASP.NET Core 8.0 Web API
- Entity Framework Core 8.0
- MS SQL Server
- JWT Authentication
- Swagger/OpenAPI

### Frontend
- Angular 21
- Capacitor (iOS & Android support)
- TypeScript
- RxJS
- Standalone Components

## Prerequisites

- .NET 8.0 SDK
- Node.js 18+ and npm
- MS SQL Server (LocalDB or full installation)
- Visual Studio 2022 or VS Code (recommended)

## Installation & Setup

### 1. Clone the Repository
```bash
git clone https://github.com/atiapa/eVoter.git
cd eVoter
```

### 2. Backend Setup

#### Configure Database Connection
Update the connection string in `Backend/eVoter.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=eVoterDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

#### Create Database Migrations
```bash
cd Backend/eVoter.API
dotnet ef migrations add InitialCreate --project ../eVoter.Data
dotnet ef database update
```

#### Run the Backend
```bash
cd Backend/eVoter.API
dotnet run
```

The API will be available at `https://localhost:5001` (HTTPS) and `http://localhost:5000` (HTTP).

### 3. Frontend Setup

#### Install Dependencies
```bash
cd Frontend
npm install
```

#### Update API URL
Update the API URL in service files if needed (default is `http://localhost:5000/api`).

#### Run Development Server
```bash
npm start
```

The application will be available at `http://localhost:4200`.

#### Build for Production
```bash
npm run build
```

### 4. Mobile App Setup (Optional)

#### Add Mobile Platforms
```bash
cd Frontend
npx cap add android
npx cap add ios
```

#### Sync with Mobile Platforms
```bash
npm run build
npx cap sync
```

#### Open in Platform IDE
```bash
npx cap open android  # For Android Studio
npx cap open ios      # For Xcode
```

## API Documentation

Once the backend is running, visit `http://localhost:5000/swagger` to view the interactive API documentation.

### Key Endpoints

#### Authentication
- `POST /api/auth/login` - Login with username/password or biometric
- `POST /api/auth/register` - Register a new voter
- `POST /api/auth/register-biometric` - Register biometric data

#### Voting
- `GET /api/voting/elections` - Get active elections
- `POST /api/voting/vote` - Cast a vote
- `GET /api/voting/has-voted/{electionId}` - Check if voted

#### Admin
- `POST /api/admin/elections` - Create an election
- `PUT /api/admin/elections/{id}/status` - Update election status
- `GET /api/admin/voters` - Get all voters
- `PUT /api/admin/voters/{id}/verify` - Verify a voter
- `GET /api/admin/elections/{id}/results` - Get election results

## Default Users

For testing, you'll need to create an admin user manually in the database or via the registration endpoint with the role set to `Admin`.

## Architecture

### Backend Layers
1. **eVoter.API**: Web API controllers and configuration
2. **eVoter.Core**: Domain models, enums, and interfaces
3. **eVoter.Data**: EF Core context, repositories, and migrations

### Frontend Structure
- **Components**: UI components (auth, admin, voting)
- **Services**: HTTP services for API communication
- **Guards**: Route protection
- **Interceptors**: HTTP request/response handling
- **Models**: TypeScript interfaces

## Database Schema

### Main Tables
- **Users**: User accounts with authentication
- **Voters**: Voter profile information
- **BiometricData**: Fingerprint and RFID data
- **Elections**: Election details
- **Candidates**: Candidates for elections
- **Votes**: Cast votes (encrypted)
- **AuditLogs**: System activity logs

## Security Considerations

1. **Change JWT Secret**: Update the JWT secret key in `appsettings.json` before deployment
2. **Use HTTPS**: Always use HTTPS in production
3. **Secure Database**: Use strong database credentials
4. **Biometric Integration**: Implement actual biometric SDK integration
5. **RFID Integration**: Implement actual RFID reader integration
6. **Vote Encryption**: Enhance vote encryption for production use
7. **Rate Limiting**: Implement rate limiting on API endpoints

## Deployment

### Backend Deployment
1. Publish the API:
   ```bash
   cd Backend/eVoter.API
   dotnet publish -c Release
   ```
2. Deploy to IIS, Azure App Service, or Docker container
3. Update connection strings and JWT settings
4. Run database migrations on production database

### Frontend Deployment
1. Build for production:
   ```bash
   cd Frontend
   npm run build
   ```
2. Deploy the `dist` folder to web server (IIS, Nginx, Azure Static Web Apps)
3. Configure reverse proxy for API calls

### Mobile App Deployment
1. Build Android APK/AAB or iOS IPA
2. Submit to Google Play Store or Apple App Store
3. Follow platform-specific guidelines

## Testing

### Backend Tests
```bash
cd Backend
dotnet test
```

### Frontend Tests
```bash
cd Frontend
npm test
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License.

## Support

For issues and questions, please create an issue in the GitHub repository.

## Roadmap

- [ ] Email verification for voter registration
- [ ] SMS notifications for voting confirmations
- [ ] Multi-language support
- [ ] Enhanced reporting and analytics
- [ ] Blockchain integration for vote verification
- [ ] Live election monitoring dashboard
- [ ] Voter identity verification via government databases
- [ ] Integration with actual biometric SDKs
- [ ] Integration with RFID readers
- [ ] Advanced audit trail and compliance reporting

## Acknowledgments

Built with modern web technologies for secure and accessible voting.
