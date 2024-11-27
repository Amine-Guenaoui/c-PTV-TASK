#!/bin/bash

# Run migrations before starting the application
dotnet ef database update --project StreetService.API/StreetService.API.csproj --startup-project StreetService.API/StreetService.API.csproj

# Start the application
dotnet StreetService.API.dll
