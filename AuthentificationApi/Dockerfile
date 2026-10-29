# =========================
# Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY AuthentificationApi/AuthentificationApi.csproj ./AuthentificationApi/
COPY GestionOrdinateurs.Shared/GestionOrdinateurs.Shared.csproj ./GestionOrdinateurs.Shared/
RUN dotnet restore ./AuthentificationApi/AuthentificationApi.csproj

# Copy the remaining source code
COPY AuthentificationApi/ ./AuthentificationApi/
COPY GestionOrdinateurs.Shared/ ./GestionOrdinateurs.Shared/
RUN dotnet publish ./AuthentificationApi/AuthentificationApi.csproj -c Release -o out

# =========================
# Runtime (Port 5157)
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:5157

EXPOSE 5157

ENTRYPOINT ["dotnet", "AuthentificationApi.dll"]