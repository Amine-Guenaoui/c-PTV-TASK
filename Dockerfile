# Use the official .NET SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
RUN apt-get update
RUN apt-get install -y curl
# Set the working directory inside the container
WORKDIR /app

# Copy the project files and restore dependencies
COPY . ./
RUN dotnet restore "StreetService.API/StreetService.API.csproj"

# Install EF tools globally (to run migrations)
RUN dotnet tool install --global dotnet-ef

# Publish the app to the /out directory
RUN dotnet publish "StreetService.API/StreetService.API.csproj" -c Release -o /app/out

# Use the official .NET SDK image again for migrations
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS migration-env

# Set the working directory
WORKDIR /app

# Copy the project files and dependencies from the previous stage
COPY --from=build-env /app /app

# Run the migration command (only on the SDK image)
RUN dotnet ef database update --connection "Host=streetservice-postgres-1;Port=5432;Database=streetservice;Username=postgres;Password=password"

# Use the official .NET runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime-env

# Set the working directory inside the container
WORKDIR /app

# Copy the published app from the build environment
COPY --from=build-env /app/out ./

# Expose port 5000 for the app
EXPOSE 5000

# Set the environment variable to make the app listen on port 5000
ENV ASPNETCORE_URLS=http://+:5000

# Run the app
ENTRYPOINT ["dotnet", "StreetService.API.dll"]
