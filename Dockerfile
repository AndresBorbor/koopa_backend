# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el csproj y restaurar dependencias
COPY ./KoopaBackend.csproj .
RUN dotnet restore KoopaBackend.csproj

# Copiar todo y compilar
COPY . .
RUN dotnet publish KoopaBackend.csproj -c Release -o /app/publish

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5027
ENTRYPOINT ["dotnet", "KoopaBackend.dll"]
