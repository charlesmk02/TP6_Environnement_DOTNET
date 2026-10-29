# =========================
# Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY GestionOrdinateurs.Client/GestionOrdinateurs.Client.csproj ./GestionOrdinateurs.Client/
COPY GestionOrdinateurs.Shared/GestionOrdinateurs.Shared.csproj ./GestionOrdinateurs.Shared/
RUN dotnet restore ./GestionOrdinateurs.Client/GestionOrdinateurs.Client.csproj

# Copy the remaining source code
COPY GestionOrdinateurs.Client/ ./GestionOrdinateurs.Client/
COPY GestionOrdinateurs.Shared/ ./GestionOrdinateurs.Shared/
RUN dotnet publish ./GestionOrdinateurs.Client/GestionOrdinateurs.Client.csproj -c Release -o out

# =========================
# Runtime Nginx (Port 5000)
# =========================
FROM nginx:alpine AS runtime
COPY GestionOrdinateurs.Client/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/out/wwwroot /usr/share/nginx/html

EXPOSE 5000

CMD ["nginx", "-g", "daemon off;"]