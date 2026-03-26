# Movie App Minimal API

## Overview

Movie App Minimal API is a backend project built with ASP.NET Core Minimal API and .NET 8. It provides CRUD endpoints for managing movies, actors, genres, and comments, and includes support for image uploads, SQL Server persistence, Swagger documentation, AutoMapper, Dapper, and output caching.

This project is designed as a learning-focused backend application that demonstrates how to organize a small API using endpoints, DTOs, repositories, services, and dependency injection instead of placing everything in a single file.

## Features

- Manage genres with create, read, update, delete, and search endpoints
- Manage actors with create, read, update, delete, and search endpoints
- Manage movies with create, read, update, and delete endpoints
- Manage comments for individual movies
- Upload actor photos and movie posters through a file storage abstraction
- Use Azure Blob Storage as the active file storage implementation
- Return paginated list data from repository methods
- Add output caching to GET endpoints
- Explore and test the API with Swagger in development

## Tech Stack

- .NET 8
- ASP.NET Core Minimal API
- SQL Server / LocalDB
- Dapper
- Entity Framework Core
- AutoMapper
- Azure Blob Storage
- Swagger / Swashbuckle

## Project Structure

- `Program.cs`
  - Configures services, middleware, dependency injection, route groups, Swagger, CORS, static files, and output caching
- `Endpoints/`
  - Contains grouped Minimal API endpoint mappings for genres, actors, movies, and comments
- `DTOs/`
  - Request and response models used to separate API contracts from entity models
- `Entity/`
  - Domain/entity classes such as `Movie`, `Actor`, `Genre`, and `Comment`
- `Repositories/`
  - Data access layer using Dapper and stored procedures
- `Services/`
  - File storage abstraction with Azure and local implementations
- `Utilities/`
  - AutoMapper profile configuration
- `ApplicationDbContext.cs`
  - EF Core database context configuration
- `MinimalApiMovie.sql`
  - SQL script for database creation and stored procedures

## API Modules

### Genres

Base route: `/genres`

- `GET /genres/`
- `GET /genres/{id}`
- `GET /genres/searchByName/{name}`
- `POST /genres/createGenre`
- `PUT /genres/updateGenre/{id}`
- `DELETE /genres/deleteGenre/{id}`

### Actors

Base route: `/actors`

- `GET /actors/`
- `GET /actors/{id}`
- `GET /actors/searchByName/{name}`
- `POST /actors/createActor`
- `PUT /actors/updateActor/{id}`
- `DELETE /actors/deleteActor/{id}`

### Movies

Base route: `/movies`

- `GET /movies/`
- `GET /movies/{id}`
- `POST /movies/createMovie`
- `PUT /movies/updateMovie/{id}`
- `DELETE /movies/deleteMovie/{id}`

### Comments

Base route: `/movie/{movieId}/comment`

- `GET /movie/{movieId}/comment/`
- `GET /movie/{movieId}/comment/{id}`
- `POST /movie/{movieId}/comment/`
- `PUT /movie/{movieId}/comment/{id}`
- `DELETE /movie/{movieId}/comment/{id}`

## Architecture Summary

The application starts in `Program.cs`, where services are registered and route groups are mapped. Each feature area has its own endpoint class inside `Endpoints/`. These endpoint methods receive DTOs, use AutoMapper to map them to entity models, and call repository interfaces for persistence.

The repository layer uses Dapper with SQL Server stored procedures to perform database operations. The application also includes an EF Core `ApplicationDbContext`, which configures the SQL Server connection and basic entity model rules.

For file uploads, the app uses an `IFileStorage` abstraction. The active dependency injection setup uses `AzureFileStorage`, while `LocalFileStorage` is also present in the repo as an alternate implementation.

## How To Run

### Prerequisites

- .NET 8 SDK
- SQL Server LocalDB
- Azure Storage account if you want the current Azure file upload flow to work as configured

### Local Setup

1. Clone the repository.
2. Open the project folder:

```powershell
cd E:\movieApp\Movie_App_MinimalApi
```

3. Restore dependencies:

```powershell
dotnet restore
```

4. Create the database objects using the SQL script:

- Run `E:\movieApp\MinimalApiMovie.sql` in SQL Server Management Studio or another SQL Server tool.

5. Review configuration in `appsettings.json`.

Note:
The current repo contains connection strings directly in configuration. For real-world use, secrets should be moved to user secrets or environment variables.

6. Start the app:

```powershell
dotnet run
```

7. Open Swagger in the browser:

- `http://localhost:5186/swagger`
- or `https://localhost:7244/swagger`

## Configuration Notes

The repo currently uses:

- `DefaultConnection` for SQL Server / LocalDB
- `azureconnectionstring` for Azure Blob Storage

Because the active storage service is `AzureFileStorage`, image upload endpoints depend on a valid Azure Blob Storage connection string unless you switch the implementation to `LocalFileStorage`.

## What This Project Demonstrates

This project shows practical backend development skills for a junior developer or fresh graduate:

- Building APIs with ASP.NET Core Minimal API
- Structuring a project using endpoints, repositories, services, DTOs, and interfaces
- Using dependency injection
- Working with SQL Server
- Using Dapper with stored procedures
- Using AutoMapper to separate DTOs from entities
- Adding Swagger for API documentation
- Handling image uploads with cloud storage
- Applying output caching to read-heavy endpoints

## Current Limitations

This repo already demonstrates useful backend concepts, but it still has some areas that could be improved for a stronger production-style portfolio project:

- No authentication or authorization
- No automated tests yet
- No global exception-handling middleware
- No request validation attributes or validation pipeline
- Secrets are stored in configuration
- README screenshots and API examples are not included yet
- No deployment instructions or hosted demo

## Interview Talking Points

If you present this project in an interview, you can describe it like this:

> I built a .NET 8 Minimal API for a movie catalog system. It supports CRUD operations for movies, actors, genres, and comments. I used DTOs, AutoMapper, repository abstractions, Dapper with SQL Server stored procedures, Swagger for documentation, and Azure Blob Storage for image uploads. I also added output caching and organized the API by feature using route groups.

## Next Improvements

Good next steps for this project include:

- Add JWT authentication and role-based authorization
- Add validation for DTOs and file uploads
- Add unit and integration tests
- Move secrets out of source control
- Add global exception handling and consistent error responses
- Add Docker support
- Add CI with GitHub Actions
- Deploy the API and attach a live demo link

## Author

Built as a backend practice and portfolio project using ASP.NET Core Minimal API.
