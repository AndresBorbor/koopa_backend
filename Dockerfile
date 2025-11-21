# Imagen SDK para desarrollo (.NET 8)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dev

# Carpeta de trabajo dentro del contenedor
WORKDIR /app/koopa_backend

# Copiamos todo el proyecto (el contexto del build es ./koopa_backend)
COPY . .

# Restauramos paquetes NuGet
# 👇 No indicamos nombre de .csproj para que funcione sin importar el casing
RUN dotnet restore

# Instalamos una versión ESTABLE de dotnet-ef como herramienta global
RUN dotnet tool install --global dotnet-ef --version 8.0.0

# Agregamos las tools globales al PATH
ENV PATH="$PATH:/root/.dotnet/tools"

# Opcional: compilar para verificar
# RUN dotnet build -c Debug --no-restore

EXPOSE 8080

# Comando por defecto (docker-compose lo puede sobreescribir con dotnet watch)
ENTRYPOINT ["dotnet", "run", "--urls", "http://+:8080"]
