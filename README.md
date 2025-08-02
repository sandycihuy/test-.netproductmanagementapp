# Product Management Application

A web-based product management system built with ASP.NET Core 6, Entity Framework Core, and SQLite database.

## Features

- **User Authentication**: Register, Login, and Profile Management
- **Product Management**: Create, Read, Update, and Soft Delete products
- **Category Management**: Create, Read, Update, and Soft Delete product categories
- **Profile Management**: Edit profile information and upload profile pictures
- **Responsive UI**: Modern, responsive design using Bootstrap 5

## Technology Stack

- **Backend**: ASP.NET Core 6
- **Database**: SQLite with Entity Framework Core
- **Frontend**: Razor Pages with Bootstrap 5
- **Authentication**: ASP.NET Core Identity
- **ORM**: Entity Framework Core

## Prerequisites

- .NET 6 SDK
- Visual Studio 2022 or VS Code
- Git

## Installation & Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd ProductManagementApp
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run database migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access the application**
   - Open your browser and navigate to: `http://localhost:5237`
   - Swagger API documentation: `http://localhost:5237/swagger`

## Default Users

The application comes with pre-seeded users:

**Admin User:**
- Email: `admin@gmail.com`
- Password: `Admin@123`

**Regular User:**
- Email: `gabriellak@gmail.com`
- Password: `Gabriella@123`

## Database Schema

The application uses a relational database with the following entities:

- **Users**: User authentication and profile information
- **ProductCategories**: Product category management
- **Products**: Product information and management

## API Endpoints

The application provides RESTful API endpoints for:

- Authentication (Login/Register)
- User Profile Management
- Product CRUD Operations
- Category CRUD Operations

## Project Structure

```
ProductManagementApp/
├── Controllers/          # API Controllers
├── Data/                # Database context and migrations
├── Models/              # Entity models
├── Pages/               # Razor Pages
├── Services/            # Business logic services
├── wwwroot/             # Static files (CSS, JS, images)
└── Program.cs           # Application entry point
```

## Development

### Adding New Migrations
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Running Tests
```bash
dotnet test
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## License

This project is licensed under the MIT License.
