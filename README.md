# Linq Backend Engineer Coding Challenge

A .NET 8 Web API that provides journey planning functionality for flight routes. The API allows users to find optimal journeys between locations based on different criteria such as minimum exchanges, lowest price, and shortest duration.

## 🚀 Features

- **User Authentication**: JWT-based authentication with registration and login
- **Journey Planning**: Find optimal routes between locations
- **Multiple Optimization Criteria**:
  - Minimize exchanges (fewest connections)
  - Minimize total price (cheapest journeys)
  - Minimize total duration (fastest journeys)
- **Pagination Support**: For browsing all possible journeys
- **Memory Caching**: Optimized performance with cached route and flight data
- **Comprehensive API Documentation**: Swagger/OpenAPI integration

## 📋 Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB, Express, or full version)
- Visual Studio 2022 or VS Code

## 🛠️ Installation & Setup

### 1. Clone the Repository
```bash
git clone <repository-url>
cd CodingChallenge
```

### 2. Database Setup
The application uses Entity Framework Core with SQL Server. The database will be created automatically on first run.

**Connection String**: Update `appsettings.json` if needed:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CodingChallenge;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### 3. Run the Application
```bash
cd CodingChallenge
dotnet run
```

The API will be available at:
- **API**: https://localhost:7001
- **Swagger Documentation**: https://localhost:7001/swagger

## 🔐 Authentication

### Register a New User
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "P@ssw0rd!",
  "confirmPassword": "P@ssw0rd!"
}
```

### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "P@ssw0rd!"
}
```

**Response includes JWT token** - use this in the `Authorization: Bearer <token>` header for all subsequent requests.

## 📡 API Endpoints

### Task 1: Minimize Exchanges
Find journeys with the fewest number of connections.

```http
GET /api/journeys/min-exchanges?origin=A&destination=C&order=asc&maxExchanges=3&maxResults=50
Authorization: Bearer <your-jwt-token>
```

**Parameters:**
- `origin` (required): Starting location (e.g., 'A')
- `destination` (required): Target location (e.g., 'C')
- `order` (optional): 'asc' (default) or 'desc'
- `maxExchanges` (optional): Maximum exchanges allowed
- `maxResults` (optional): Maximum results (default: 100)

### Task 2: Minimize Total Price
Find cheapest journeys departing after a specific time.

```http
GET /api/journeys/min-price?origin=A&destination=C&departure=2024-01-15T10:30:00Z&order=asc&maxResults=50
Authorization: Bearer <your-jwt-token>
```

**Parameters:**
- `origin` (required): Starting location
- `destination` (required): Target location
- `departure` (required): ISO 8601 datetime (e.g., 2024-01-15T10:30:00Z)
- `order` (optional): 'asc' (default) or 'desc'
- `maxExchanges` (optional): Maximum exchanges allowed
- `maxResults` (optional): Maximum results (default: 100)

### Task 3: Minimize Total Duration
Find fastest journeys departing after a specific time.

```http
GET /api/journeys/min-duration?origin=A&destination=C&departure=2024-01-15T10:30:00Z&order=asc&maxResults=50
Authorization: Bearer <your-jwt-token>
```

**Parameters:**
- `origin` (required): Starting location
- `destination` (required): Target location
- `departure` (required): ISO 8601 datetime
- `order` (optional): 'asc' (default) or 'desc'
- `maxExchanges` (optional): Maximum exchanges allowed
- `maxResults` (optional): Maximum results (default: 100)

### Task 4: All Journeys
Get all possible journeys with pagination.

```http
GET /api/journeys?page=1&size=50&orderBy=NumberOfExchanges&order=asc&maxExchanges=3
Authorization: Bearer <your-jwt-token>
```

**Parameters:**
- `page` (optional): Page number (default: 1)
- `size` (optional): Page size (default: 100, max: 1000)
- `orderBy` (optional): 'NumberOfExchanges', 'TotalPrice', 'TotalDuration' (default: NumberOfExchanges)
- `order` (optional): 'asc' (default) or 'desc'
- `maxExchanges` (optional): Maximum exchanges allowed

## 📊 Response Format

### Journey Response Example
```json
{
  "origin": "A",
  "destination": "C",
  "departure": "+2 day 4 hour",
  "totalPrice": 450.00,
  "totalDuration": 12,
  "exchanges": 1,
  "flights": [
    {
      "flight_id": 123,
      "provider": "Virgin",
      "from": "A",
      "to": "B",
      "price": 250.00,
      "duration": 4,
      "departure": "2024-01-15T10:30:00Z"
    },
    {
      "flight_id": 456,
      "provider": "British Airways",
      "from": "B",
      "to": "C",
      "price": 200.00,
      "duration": 8,
      "departure": "2024-01-15T14:30:00Z"
    }
  ]
}
```

### Pagination Response Example
```json
{
  "journeys": [...],
  "pagination": {
    "totalJourneys": 1250,
    "currentPage": 1,
    "pageSize": 50,
    "totalPages": 25
  }
}
```

## 🏗️ Architecture

### Project Structure
```
CodingChallenge/
├── Controllers/          # API Controllers
│   ├── AuthController.cs
│   └── JourneyController.cs
├── Data/                 # Data Access Layer
│   ├── AppDbContext.cs
│   └── DbSeeder.cs
├── Models/               # Data Models
│   ├── Entities/         # Database Entities
│   └── DTOs/            # Data Transfer Objects
├── Services/             # Business Logic
│   ├── Interfaces/       # Service Interfaces
│   ├── AuthService.cs
│   └── JourneyService.cs
├── Middleware/           # Custom Middleware
└── Sample Data/          # JSON Data Files
    ├── routes.json
    └── flights.json
```

### Key Technologies
- **.NET 8**: Latest LTS version
- **Entity Framework Core**: ORM for database operations
- **ASP.NET Core Identity**: User authentication and authorization
- **JWT Bearer Tokens**: Stateless authentication
- **Memory Cache**: Performance optimization
- **Swagger/OpenAPI**: API documentation

### Algorithms
- **Breadth-First Search (BFS)**: Used for finding all possible journeys
- **Graph Traversal**: Routes and flights form a directed graph
- **Cycle Prevention**: Prevents infinite loops in journey calculation
- **Temporal Constraints**: Ensures flights are taken in chronological order

## 🔧 Configuration

### JWT Settings
Update `appsettings.json`:
```json
{
  "Jwt": {
    "Key": "your-super-secret-key-here-minimum-16-characters",
    "Issuer": "CodingChallenge",
    "Audience": "CodingChallengeUsers"
  }
}
```

### Database Connection
The application uses SQL Server LocalDB by default. For production, update the connection string in `appsettings.json`.

## 🧪 Testing

### Using Swagger UI
1. Navigate to https://localhost:7001/swagger
2. Register a new user using the `/api/auth/register` endpoint
3. Login using `/api/auth/login` to get a JWT token
4. Click "Authorize" and enter `Bearer <your-token>`
5. Test the journey endpoints

### Using curl
```bash
# Register
curl -X POST "https://localhost:7001/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"P@ssw0rd!","confirmPassword":"P@ssw0rd!"}'

# Login
curl -X POST "https://localhost:7001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"P@ssw0rd!"}'

# Get journeys (replace <token> with actual JWT token)
curl -X GET "https://localhost:7001/api/journeys/min-exchanges?origin=A&destination=C" \
  -H "Authorization: Bearer <token>"
```

## 🚀 Performance Optimizations

1. **Memory Caching**: Routes and flights are cached for 30 minutes
2. **Efficient Algorithms**: BFS with early termination for optimal performance
3. **Pagination**: Limits result sets to prevent memory issues
4. **Cycle Prevention**: Prevents infinite loops in journey calculation
5. **Parameter Validation**: Early validation to avoid unnecessary processing

## 📝 Notes

- **Sample Data**: The application includes sample routes and flights data that is automatically seeded
- **Journey Calculation**: All journeys are calculated on-the-fly using the BFS algorithm
- **Departure Times**: Input uses ISO 8601 format, output uses human-readable format (e.g., "+2 day 4 hour")
- **Authentication**: All journey endpoints require valid JWT authentication
- **Error Handling**: Comprehensive error handling with appropriate HTTP status codes

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is part of the Linq Backend Engineer Coding Challenge. 