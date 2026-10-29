# =========================
# Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copie des fichiers csproj et restauration
COPY GestionOrdinateurs.API/GestionOrdinateurs.API.csproj ./GestionOrdinateurs.API/
COPY GestionOrdinateurs.Shared/GestionOrdinateurs.Shared.csproj ./GestionOrdinateurs.Shared/
RUN dotnet restore ./GestionOrdinateurs.API/GestionOrdinateurs.API.csproj

# Copie du code source et publication
COPY GestionOrdinateurs.API/ ./GestionOrdinateurs.API/
COPY GestionOrdinateurs.Shared/ ./GestionOrdinateurs.Shared/
RUN dotnet publish ./GestionOrdinateurs.API/GestionOrdinateurs.API.csproj -c Release -o out

# =========================
# Runtime (Port 5019)
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:5019

EXPOSE 5019

ENTRYPOINT ["dotnet", "GestionOrdinateurs.API.dll"]