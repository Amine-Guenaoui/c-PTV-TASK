# Use the official .NET SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

# Set the working directory inside the container
WORKDIR /app

# Copy the project file and restore dependencies
COPY . ./
RUN dotnet restore "StreetService.API/StreetService.API.csproj"

# Publish the app to the /out directory
RUN dotnet publish "StreetService.API/StreetService.API.csproj" -c Release -o /app/out

# Use the official .NET runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set the working directory inside the container
WORKDIR /app

# Copy the published app from the build environment
COPY --from=build-env /app/out ./

# Expose port 5000
EXPOSE 5000

# Set the environment variable to make the app listen on port 5000
ENV ASPNETCORE_URLS=http://+:5000

# Run the app
ENTRYPOINT ["dotnet", "StreetService.API.dll"]
