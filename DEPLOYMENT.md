# Deployment Guide - eVoter System

This guide provides step-by-step instructions for deploying the eVoter system to production.

## Prerequisites

### Infrastructure Requirements
- Web server (IIS, Nginx, or Azure App Service)
- SQL Server 2019+ or Azure SQL Database
- SSL/TLS certificate
- Domain name
- (Optional) Android/iOS developer accounts for mobile apps

### Tools Required
- .NET 8.0 SDK
- Node.js 18+
- SQL Server Management Studio (optional)
- Azure CLI (for Azure deployment)

## Pre-Deployment Checklist

- [ ] Security review completed
- [ ] All tests passing
- [ ] Database migration scripts tested
- [ ] SSL certificate obtained
- [ ] Domain DNS configured
- [ ] Backup strategy defined
- [ ] Monitoring tools configured
- [ ] Environment variables prepared

## Backend Deployment

### 1. Configure Production Settings

#### Environment Variables
Create environment variables for sensitive data:

```bash
# Linux/macOS
export ConnectionStrings__DefaultConnection="Server=prod-server;Database=eVoterDb;..."
export Jwt__Key="your-secure-64-character-key-here"
export Jwt__Issuer="eVoter"
export Jwt__Audience="eVoter"

# Windows
setx ConnectionStrings__DefaultConnection "Server=prod-server;Database=eVoterDb;..."
setx Jwt__Key "your-secure-64-character-key-here"
```

#### Update appsettings.Production.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "yourdomain.com,www.yourdomain.com"
}
```

### 2. Build and Publish

```bash
cd Backend/eVoter.API
dotnet publish -c Release -o ./publish
```

### 3. Database Deployment

#### Generate Migration Script
```bash
cd Backend/eVoter.API
dotnet ef migrations script -o migration.sql
```

#### Apply to Production Database
```sql
-- Review the migration.sql file
-- Then execute on production database
-- USE eVoterDb;
-- Execute migration.sql content
```

### 4. Deploy to IIS (Windows)

1. Install IIS with ASP.NET Core hosting bundle
2. Create application pool (.NET CLR Version: No Managed Code)
3. Create website pointing to publish folder
4. Configure bindings (HTTPS on port 443)
5. Set environment variables in IIS

### 5. Deploy to Azure App Service

```bash
# Login to Azure
az login

# Create resource group
az group create --name eVoter-rg --location eastus

# Create app service plan
az appservice plan create --name eVoter-plan --resource-group eVoter-rg --sku B1

# Create web app
az webapp create --name evoter-api --resource-group eVoter-rg --plan eVoter-plan --runtime "DOTNET|8.0"

# Configure app settings
az webapp config appsettings set --name evoter-api --resource-group eVoter-rg \
  --settings Jwt__Key="your-key" ConnectionStrings__DefaultConnection="your-connection"

# Deploy
cd Backend/eVoter.API
az webapp up --name evoter-api --resource-group eVoter-rg
```

## Frontend Deployment

### 1. Configure Production API URL

Update `src/app/services/*.service.ts`:

```typescript
private apiUrl = 'https://api.yourdomain.com/api';
```

Or use environment files:

#### src/environments/environment.prod.ts
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://api.yourdomain.com/api'
};
```

### 2. Build for Production

```bash
cd Frontend
npm run build
```

### 3. Deploy to Static Hosting

#### Deploy to Azure Static Web Apps
```bash
# Install SWA CLI
npm install -g @azure/static-web-apps-cli

# Deploy
swa deploy ./dist/Frontend \
  --resource-group eVoter-rg \
  --app-name evoter-web \
  --env production
```

#### Deploy to Netlify
```bash
# Install Netlify CLI
npm install -g netlify-cli

# Deploy
cd Frontend
netlify deploy --prod --dir=dist/Frontend/browser
```

#### Deploy to IIS/Nginx

Copy contents of `Frontend/dist/Frontend/browser` to web root.

**Nginx Configuration:**
```nginx
server {
    listen 80;
    server_name yourdomain.com;
    root /var/www/evoter;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }

    location /api {
        proxy_pass https://api.yourdomain.com;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }
}
```

## Mobile App Deployment

### 1. Configure Production API

Update Capacitor configuration:

```typescript
// capacitor.config.ts
import { CapacitorConfig } from '@capacitor/cli';

const config: CapacitorConfig = {
  appId: 'io.evoter.app',
  appName: 'eVoter',
  webDir: 'dist/frontend/browser',
  server: {
    url: 'https://yourdomain.com',
    cleartext: false
  }
};

export default config;
```

### 2. Build Mobile Apps

```bash
cd Frontend

# Build web assets
npm run build

# Sync with native projects
npx cap sync

# Android
npx cap open android
# In Android Studio: Build > Generate Signed Bundle/APK

# iOS
npx cap open ios
# In Xcode: Product > Archive
```

### 3. Publish to App Stores

#### Google Play Store
1. Create app listing at Google Play Console
2. Upload signed APK/AAB
3. Complete store listing
4. Submit for review

#### Apple App Store
1. Create app record in App Store Connect
2. Upload IPA via Xcode or Transporter
3. Complete App Store information
4. Submit for review

## Post-Deployment

### 1. Smoke Testing

Test critical paths:
- [ ] User registration
- [ ] Login (password, biometric, RFID)
- [ ] View elections
- [ ] Cast vote
- [ ] Admin login
- [ ] Create election
- [ ] View results

### 2. Monitoring Setup

#### Application Insights (Azure)
```bash
# Add Application Insights to API
az monitor app-insights component create \
  --app evoter-insights \
  --location eastus \
  --resource-group eVoter-rg
```

#### Configure Logging
Update `appsettings.Production.json`:
```json
{
  "ApplicationInsights": {
    "InstrumentationKey": "your-key"
  }
}
```

### 3. Performance Optimization

- [ ] Enable response compression
- [ ] Configure CDN for static assets
- [ ] Enable database query caching
- [ ] Configure connection pooling
- [ ] Set up load balancing

### 4. Security Hardening

- [ ] Configure firewall rules
- [ ] Enable DDoS protection
- [ ] Set up WAF (Web Application Firewall)
- [ ] Configure rate limiting
- [ ] Enable SQL Server TDE
- [ ] Implement IP whitelisting for admin access
- [ ] Set up security headers

### 5. Backup Configuration

#### SQL Server Backup
```sql
-- Full backup
BACKUP DATABASE eVoterDb
TO DISK = 'C:\Backups\eVoterDb_Full.bak'
WITH FORMAT, NAME = 'Full Backup of eVoterDb';

-- Automated backup job (create via SQL Server Agent)
```

#### Azure SQL Automated Backup
Configured automatically with point-in-time restore up to 35 days.

## Maintenance

### Regular Tasks
- Daily: Monitor application logs
- Daily: Check database backups
- Weekly: Review security alerts
- Weekly: Check application performance
- Monthly: Apply security patches
- Monthly: Review audit logs
- Quarterly: Conduct security audit

### Update Procedure
1. Test updates in staging environment
2. Notify users of planned maintenance
3. Create backup
4. Deploy updates
5. Run smoke tests
6. Monitor for issues
7. Rollback if necessary

## Rollback Procedure

### Backend Rollback
```bash
# Azure App Service
az webapp deployment slot swap --name evoter-api --resource-group eVoter-rg \
  --slot staging --target-slot production

# Or restore previous deployment
az webapp deployment source config-zip --name evoter-api --resource-group eVoter-rg \
  --src previous-version.zip
```

### Database Rollback
```sql
-- Restore from backup
RESTORE DATABASE eVoterDb
FROM DISK = 'C:\Backups\eVoterDb_Full.bak'
WITH REPLACE;
```

## Troubleshooting

### Common Issues

1. **500 Internal Server Error**
   - Check application logs
   - Verify database connection
   - Check environment variables

2. **Database Connection Timeout**
   - Check connection string
   - Verify firewall rules
   - Check SQL Server status

3. **JWT Token Issues**
   - Verify JWT secret key is set
   - Check token expiration
   - Verify claims configuration

4. **CORS Errors**
   - Update CORS policy in Program.cs
   - Add production domain to allowed origins

5. **Mobile App Not Connecting**
   - Check API URL in capacitor.config.ts
   - Verify SSL certificate is valid
   - Check mobile network permissions

## Support

For deployment issues:
1. Check application logs
2. Review SECURITY.md
3. Consult README.md
4. Create GitHub issue

---

**Document Version**: 1.0  
**Last Updated**: 2026-02-11
