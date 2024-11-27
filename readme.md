# StreetService API

This project implements a service in .NET with a REST API for managing street data. It provides endpoints to create, delete, and update street geometries, stored in a PostgreSQL database with PostGIS. The application uses Entity Framework Core for data persistence.

## Task Breakdown and Implementation

1. **Create a service in .NET with a REST API for 'Street' resource**  
   The first task involved creating a .NET service that exposes a REST API to manage a `Street` resource. A `Street` has the following properties:
   - **Name**: The name of the street.
   - **Geometry**: A GeoJSON-based geometry that defines the shape of the street.
   - **Capacity**: The number of vehicles that can use the street within a minute.

   We designed the API with the following endpoints:
   - **POST /streets**: Creates a new street resource.
   - **DELETE /streets/{id}**: Deletes a street resource by its ID.

   We used **ASP.NET Core** for the web service and **Entity Framework (EF) Core** for interacting with the database.

2. **Storing street data in PostgreSQL with PostGIS**  
   The street data is stored in a **PostgreSQL** database, and we made use of the **PostGIS** extension to handle spatial data (geometry).  
   We used **EF Core** to interact with the database, and the model for the street resource was created as follows:
   - **Street.cs**: The model represents the street entity with properties like `Name`, `Geometry`, and `Capacity`.
   - We created a **DbContext** class (`AppDbContext.cs`) to handle database operations and migrations.
   - The database schema was generated using **EF migrations**, which allowed us to manage changes to the database structure.

3. **Implement an endpoint to add a single point to the geometry of an existing street**  
   We implemented an endpoint to add a single point to the geometry of an existing street, either at the beginning or the end, depending on the user's choice.  
   The logic for determining where to add the point was based on the geometry's existing coordinates:
   - If the point was to be added at the beginning, it was appended to the start of the `LineString`.
   - If the point was to be added at the end, it was appended to the end of the `LineString`.

   This endpoint was designed to handle race conditions that could occur when multiple requests try to modify the geometry of the same street simultaneously. We used **transactions** and **optimistic concurrency** control to ensure data consistency and avoid conflicts.

4. **Add a feature flag for deciding where the operation is performed**  
   We added a hidden feature flag to control whether the point addition operation would be executed:
   - **On the database level**: Using **PostGIS** functions for spatial data manipulation directly in the database.
   - **Within the backend code**: Implementing the logic algorithmically within the application code.

   The feature flag allows us to toggle between these two approaches based on specific requirements or performance considerations. This was achieved through a configuration setting in the **appsettings.json** file.

5. **Create a Docker file and a Kubernetes manifest for deployment**  
   To deploy the application in a containerized environment, we created the following:
   - **Dockerfile**: This file builds the .NET API into a Docker container. It installs the necessary dependencies and sets up the environment for running the API.
   - **Kubernetes Manifest**: We created a Kubernetes deployment manifest to deploy the application as a service with **3 replicas**. This ensures high availability and load balancing across the replicas.

   The manifest includes:
   - **Deployment**: Defines the number of replicas (3) and the container image for the application.
   - **Service**: Exposes the API to other services within the Kubernetes cluster.

6. **Create a Docker Compose file for local testing**  
   To simplify local development and testing, we added a **docker-compose.yml** file. This file defines the services needed to run the application, including:
   - The **API** service (the .NET application container).
   - The **PostgreSQL** service with **PostGIS** enabled.

   The `docker-compose.yml` file allows us to quickly start up the environment with the following command:

   ```bash
   docker-compose up -d
   ```

   This setup allows us to test the application locally before deploying it to production.

### Conclusion
By following these tasks, we created a full-stack solution that uses **.NET Core**, **EF Core**, **PostgreSQL**, **PostGIS**, **Docker**, and **Kubernetes** to provide a scalable and reliable API for managing street data. The API includes robust handling for spatial data and offers flexibility through a feature flag, enabling us to switch between database and backend processing for spatial operations.

## Project Organization

```plaintext
+--- StreetService.API
|   +--- Controllers
|   |   +--- StreetController.cs
|   +--- Properties
|   |   +--- launchSettings.json
|   |   +--- appsettings.Development.json
|   |   +--- appsettings.json
|   |   +--- dockerfile
|   |   +--- Program.cs
|   |   +--- StreetService.API.csproj
|   |   +--- StreetService.API.http
+--- StreetService.Core
|   +--- Interfaces
|   |   +--- IStreetRepository.cs
|   +--- Models
|   |   +--- Street.cs
|   +--- Class1.cs
|   +--- StreetService.Core.csproj
+--- StreetService.Infrastructure
|   +--- Data
|   |   +--- AppDbContext.cs
|   +--- Migrations
|   |   +--- 20241125111231_InitialCreate.cs
|   |   +--- 20241125111231_InitialCreate.Designer.cs
|   |   +--- 20241127095108_AddPointToStreetGeometry.cs
|   |   +--- 20241127095108_AddPointToStreetGeometry.Designer.cs
|   |   +--- AppDbContextModelSnapshot.cs
|   +--- Repositories
|   |   +--- StreetRepository.cs
|   +--- Class1.cs
|   +--- DependencyInjection.cs
|   +--- StreetService.Infrastructure.csproj
+--- Dock.test
+--- docker-compose.yml
+--- Dockerfile
+--- entrypoint.sh
+--- Readme
+--- StreetService.sln
+--- tree.ps1
```

## Overview

The `StreetService` project is a microservices-based application for managing street data. It consists of three main layers: **API**, **Core**, and **Infrastructure**. This project leverages Docker for containerization and EF Core for database management.

## Architecture

### 1. **StreetService.API** (Presentation Layer)
The API layer is responsible for handling HTTP requests and interacting with the business logic. It contains controllers that expose endpoints for creating, reading, updating, and deleting street data.

- **Controllers**
  - `StreetController.cs`: Handles HTTP requests related to street entities.
- **Properties**
  - `appsettings.json`: Contains configuration settings for the application.
  - `launchSettings.json`: Defines how the application is launched.
- **Program.cs**: The main entry point of the application, setting up dependency injection and the web host.
- **dockerfile**: Contains the instructions to build the Docker image for the API.
  
### 2. **StreetService.Core** (Business Logic Layer)
This layer contains business logic and domain models. It holds interfaces and models necessary for the operation of the application.

- **Interfaces**
  - `IStreetRepository.cs`: Defines the repository contract for interacting with street data.
- **Models**
  - `Street.cs`: Defines the `Street` entity model used throughout the application.

### 3. **StreetService.Infrastructure** (Data Layer)
This layer is responsible for data access and persistence. It contains the EF Core context and repository implementation.

- **Data**
  - `AppDbContext.cs`: EF Core database context responsible for interacting with the database.
- **Migrations**
  - `20241125111231_InitialCreate.cs`: Initial database migration.
  - `20241127095108_AddPointToStreetGeometry.cs`: Migration for adding geometry data.
- **Repositories**
  - `StreetRepository.cs`: Implements the repository pattern for `Street` entities.
- **DependencyInjection.cs**: Sets up dependency injection for the infrastructure layer.

### 4. **Shared**
- **docker-compose.yml**: Defines services for running the entire application stack, including the API and PostgreSQL container.
- **Dockerfile**: Builds the Docker image for the project.
- **entrypoint.sh**: Custom entrypoint script to start the application.


## Getting Started

These instructions will help you get the project up and running on your local machine.

### Prerequisites

- **.NET SDK 8.0** or higher
- **Docker** and **Docker Compose** installed
- **PostgreSQL** with **PostGIS** extension

### Step 1: Create Docker Image for PostgreSQL

To run a PostgreSQL database with PostGIS, use the following command to create the Docker image:

```bash
docker run --name postgres-db -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=yourpassword -e POSTGRES_DB=streetservice -p 5432:5432 -d postgis/postgis:latest
```

To access the PostgreSQL container, you can run:

```bash
docker exec -it postgres-db psql -U postgres -d streetservice
```

Alternatively, you can use Docker Compose to spin up the database with:

```bash
docker-compose up -d
```

### Step 2: Run Migrations

Once the database is set up, you'll need to apply the migrations. You can run them directly from your local machine.

To create the initial migration:

```bash
dotnet ef migrations add InitialCreate --project D:\Job\StreetService\StreetService.Infrastructure\StreetService.Infrastructure.csproj --startup-project D:\Job\StreetService\StreetService.API\StreetService.API.csproj
```

To create the migration for adding a point to the street geometry:

```bash
dotnet ef migrations add AddPointToStreetGeometry --project D:\Job\StreetService\StreetService.Infrastructure\StreetService.Infrastructure.csproj --startup-project D:\Job\StreetService\StreetService.API\StreetService.API.csproj
```

Then, update the database:

```bash
dotnet ef database update --project D:\Job\StreetService\StreetService.Infrastructure --startup-project D:\Job\StreetService\StreetService.API
```

### Step 3: Running the Project

To run the project, navigate to the **StreetService** directory and run the following command:

```bash
dotnet run --project StreetService.API
```

### Step 4: Install Required Packages

If you haven't installed the necessary packages, you can do so by running:

```bash
dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson --version 8.0.0
```

### Step 5: Testing the API

You can test the API using Swagger. To add a street, use the following JSON for a POST request: ( i have added a Collection file called PostManExample import it in PostMan)

```json
{
  "id": 1,
  "name": "Example Street",
  "geometry": {
    "type": "LineString",
    "coordinates": [
      [10.0, 20.0],
      [15.0, 25.0],
      [20.0, 30.0]
    ]
  }
}
```

After adding a street, you can test the geometry update endpoint.

### Step 6: Running the Full Project with Docker

To build and run the entire project using Docker, run the following command to build the image:

```bash
docker build -t streetservice-api .
```

Then, use the following command to start the containers using Docker Compose:

```bash
docker-compose up -d
```

To remove the Docker containers, images, and volumes, use the following command:

```bash
docker-compose down --volumes
```

To rebuild and start the containers again:

```bash
docker-compose up --build -d
```

To access the running container:

```bash
docker exec -it streetservice-streetservice-api-1 bash
```

### Step 7: Running Migrations in Docker

If the migrations don't work automatically, change the connection string in the **appsettings.json** file to use **localhost** or your desired host:

```json
"DefaultConnection": "Host=localhost;Port=5432;Database=streetservice;Username=postgres;Password=password"
```

Then, run the following command inside the container to apply the migrations:

```bash
dotnet ef database update --project D:\Job\StreetService\StreetService.Infrastructure --startup-project D:\Job\StreetService\StreetService.API
```

## Features

- **Create and delete streets**: Streets can be created with a name, geometry (using GeoJSON format), and capacity. 
- **Add geometry points**: An endpoint allows you to add a single point to the street geometry, either at the beginning or end of the street, depending on the logic.
- **Database with PostGIS**: The application uses PostGIS for spatial data storage and manipulation.
- **Feature flag for database vs backend logic**: A hidden feature flag allows you to toggle between performing the geometry update at the database level or within the backend application.
  
## Docker Support

### Docker Compose

The **docker-compose.yml** file is provided to easily spin up the entire application stack, including PostgreSQL with PostGIS, the API, and the backend service. Use the following command to start the containers:

```bash
docker-compose up -d
```

### Dockerfile

A **Dockerfile** is provided to build and run the application in a containerized environment. It sets up the .NET API and prepares the application for deployment.

### Kubernetes Support

Currently, Kubernetes support has not been implemented, but Docker Compose can be used for local testing and deployment.

## Conclusion

This repository contains a simple RESTful API built using .NET for managing street data and handling geographic data with PostGIS. The project is containerized using Docker, with PostgreSQL as the database backend. Docker Compose is used to simplify local development and testing, and the project includes support for applying EF Core migrations.
