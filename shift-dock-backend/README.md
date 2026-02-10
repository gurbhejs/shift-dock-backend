# ShiftDock Backend API

ASP.NET Core 10 Web API for ShiftDock workforce management system.

## Tech Stack

- **Framework**: .NET 10
- **Database**: PostgreSQL with Entity Framework Core
- **Authentication**: AWS Cognito + JWT Bearer tokens
- **Architecture**: Clean Architecture (Domain, Application, Infrastructure, API)
- **ORM**: Entity Framework Core 10
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **Documentation**: Swagger/OpenAPI

## Project Structure

```
ShiftDock.sln
├── src/
│   ├── ShiftDock.Domain/           # Domain entities and enums
│   ├── ShiftDock.Application/      # Business logic, DTOs, interfaces
│   ├── ShiftDock.Infrastructure/   # Data access, repositories, external services
│   └── ShiftDock.API/              # API controllers, middleware, configuration
```

## Prerequisites

- .NET 10 SDK
- PostgreSQL 14+
- AWS Cognito User Pool (for production authentication)

## Setup

### 1. Clone and Restore Packages

```bash
cd shift-dock-backend
dotnet restore
```

### 2. Configure Database

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Port=5432;Database=ShiftDockDb;Username=postgres;Password=yourpassword"
  }
}
```

### 3. Development Mode (Mock Authentication)

**For local development**, the API automatically uses a **Mock Auth Service** that bypasses AWS Cognito:

- ✅ **No AWS credentials required**
- ✅ **Use OTP: `123456` for all phone numbers**
- ✅ **JWT tokens are generated locally**

When running in Development mode (default), you'll see:
```
🔧 [DEV MODE] Using Mock Authentication Service (OTP: 123456)
```

**Test authentication flow:**
1. Send OTP: `POST /api/auth/send-otp` with any phone number
2. Verify OTP: `POST /api/auth/verify-otp` with phone and OTP `123456`
3. Sign up: `POST /api/auth/signup` (if user doesn't exist)

### 4. Production Setup (AWS Cognito)

For production deployment, configure AWS Cognito in `appsettings.Production.json`:

```json
{
  "AwsSettings": {
    "Region": "us-east-1",
    "UserPoolId": "your-user-pool-id",
    "ClientId": "your-app-client-id",
    "ClientSecret": "your-app-client-secret"
  }
}
```

The API will automatically use the real `AuthService` with AWS Cognito in production.

### 5. Update JWT Secret

Change the JWT secret key in `appsettings.json` (must be at least 32 characters):

```json
{
  "JwtSettings": {
    "Secret": "YourSuperSecretKeyForJWT-ChangeThisInProduction"
  }
}
```

### 6. Run Database Migrations

```bash
cd src/ShiftDock.API
dotnet ef migrations add InitialCreate --project ../ShiftDock.Infrastructure
dotnet ef database update
```

### 7. Run the API

```bash
dotnet run --project src/ShiftDock.API
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

## API Endpoints

### Authentication
- `POST /api/auth/send-otp` - Send OTP to phone number
- `POST /api/auth/verify-otp` - Verify OTP and get JWT token
- `POST /api/auth/signup` - Register new user
- `POST /api/auth/resend-otp` - Resend OTP
- `POST /api/auth/refresh` - Refresh access token
- `POST /api/auth/signout` - Sign out user

### Organizations
- `POST /api/organizations` - Create organization
- `GET /api/organizations/my-organizations` - Get user's organizations
- `GET /api/organizations/{orgId}` - Get organization details
- `PUT /api/organizations/{orgId}` - Update organization
- `POST /api/organizations/join` - Request to join organization
- `GET /api/organizations/{orgId}/join-requests` - Get pending requests
- `POST /api/organizations/{orgId}/join-requests/{requestId}` - Approve/reject request

### Users
- `GET /api/organizations/{orgId}/members` - Get organization members
- `POST /api/organizations/{orgId}/members` - Add worker
- `PUT /api/organizations/{orgId}/members/{userId}` - Update worker
- `PATCH /api/organizations/{orgId}/members/{userId}/status` - Update status
- `DELETE /api/organizations/{orgId}/members/{userId}` - Remove worker
- `GET /api/users/me` - Get current user profile
- `PUT /api/users/me` - Update current user profile

### Projects
- `GET /api/organizations/{orgId}/projects` - Get projects
- `GET /api/organizations/{orgId}/projects/{projectId}` - Get project details
- `POST /api/organizations/{orgId}/projects` - Create project
- `PUT /api/organizations/{orgId}/projects/{projectId}` - Update project
- `DELETE /api/organizations/{orgId}/projects/{projectId}` - Delete project
- `GET /api/addresses/suggestions` - Get address suggestions

### Worker Assignments
- `GET /api/organizations/{orgId}/projects/{projectId}/assignments` - Get assignments
- `POST /api/organizations/{orgId}/projects/{projectId}/assignments` - Assign workers
- `PUT /api/organizations/{orgId}/projects/{projectId}/assignments/{id}` - Update assignment
- `DELETE /api/organizations/{orgId}/projects/{projectId}/assignments/{id}` - Remove assignment
- `GET /api/organizations/{orgId}/projects/{projectId}/shifts/{date}/assignments` - Get shift assignments

### Dashboard
- `GET /api/organizations/{orgId}/dashboard/stats` - Get dashboard statistics
- `GET /api/organizations/{orgId}/dashboard/today-shifts` - Get today's shifts
- `GET /api/organizations/{orgId}/dashboard/pending-requests` - Get pending requests count

### Notifications
- `GET /api/notifications` - Get user notifications
- `PATCH /api/notifications/{id}/read` - Mark as read
- `PATCH /api/notifications/read-all` - Mark all as read

### Reports
- `GET /api/organizations/{orgId}/reports/attendance` - Worker attendance report
- `GET /api/organizations/{orgId}/reports/projects/{projectId}` - Project report

### Settings
- `GET /api/users/me/notification-preferences` - Get notification preferences
- `PUT /api/users/me/notification-preferences` - Update preferences

## Development

### Build Solution

```bash
dotnet build
```

### Run Tests

```bash
dotnet test
```

### Add Migration

```bash
dotnet ef migrations add MigrationName --project src/ShiftDock.Infrastructure --startup-project src/ShiftDock.API
```

### Update Database

```bash
dotnet ef database update --project src/ShiftDock.API
```

## Configuration

### CORS

Update `appsettings.json` to configure allowed origins:

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "https://yourdomain.com"
    ]
  }
}
```

### JWT Token Expiration

Default is 24 hours (1440 minutes). Update in `appsettings.json`:

```json
{
  "JwtSettings": {
    "ExpirationMinutes": 1440
  }
}
```

## Production Deployment

1. Update `appsettings.Production.json` with production values
2. Set environment variables for sensitive data:
   - `ConnectionStrings__PostgreSQL`
   - `JwtSettings__SecretKey`
   - `AwsSettings__ClientSecret`
3. Enable HTTPS
4. Configure proper CORS origins
5. Set up logging (Application Insights, Serilog, etc.)
6. Configure rate limiting
7. Enable response compression

## License

Proprietary - All rights reserved

## Support

For support, contact: admin@shiftdock.com
