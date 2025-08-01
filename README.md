# Product Management App

A full-featured product management application built with ASP.NET Core, Entity Framework Core, and Identity for user authentication and authorization.

## Features

- User registration with email confirmation
- Secure login/logout
- User profile management with profile picture upload
- Product categories management (CRUD operations)
- Products management (CRUD operations)
- Soft delete functionality
- RESTful API endpoints
- JWT authentication

## Prerequisites

- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [SQLite](https://sqlite.org/download.html) (for development)
- [Node.js](https://nodejs.org/) (for frontend assets)

## Getting Started

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd ProductManagementApp
   ```

2. **Configure the application**
   - Copy `appsettings.Development.json.example` to `appsettings.Development.json`
   - Update the connection string and other settings as needed

3. **Apply database migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access the application**
   - Web interface: `https://localhost:5001`
   - API documentation: `https://localhost:5237/swagger`

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and get JWT token
- `POST /api/auth/logout` - Logout

### Profile
- `GET /api/profile` - Get current user profile
- `PUT /api/profile` - Update profile
- `POST /api/profile/change-password` - Change password

### Product Categories
- `GET /api/productcategories` - Get all categories
- `GET /api/productcategories/{id}` - Get category by ID
- `POST /api/productcategories` - Create new category
- `PUT /api/productcategories/{id}` - Update category
- `DELETE /api/productcategories/{id}` - Soft delete category

### Products
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create new product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Soft delete product

## Project Structure

```
ProductManagementApp/
├── Controllers/         # API controllers
├── Data/                # Database context and migrations
├── Models/              # Domain models
├── Pages/               # Razor pages (if any)
├── Services/            # Business logic services
├── wwwroot/             # Static files
├── Program.cs           # Application entry point
└── README.md            # This file
```

## Configuration

The application can be configured using the following environment variables or `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app.db"
  },
  "Jwt": {
    "Key": "your-secret-key-here",
    "Issuer": "ProductManagementApp",
    "Audience": "ProductManagementAppUsers",
    "ExpireDays": 7
  },
  "EmailSettings": {
    "FromEmail": "noreply@yourdomain.com",
    "SmtpServer": "smtp.yourprovider.com",
    "Port": 587,
    "Username": "your-email@yourdomain.com",
    "Password": "your-email-password"
  }
}
```

## Security Considerations

- Always use HTTPS in production
- Store sensitive information in environment variables or a secure secret manager
- Keep dependencies up to date
- Implement rate limiting for API endpoints
- Use proper CORS policies

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- [.NET Core](https://dotnet.microsoft.com/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)
