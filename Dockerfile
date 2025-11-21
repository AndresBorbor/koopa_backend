FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dev
WORKDIR /app

# Copiamos el csproj y restauramos dependencias
COPY *.csproj ./
RUN dotnet restore

# Instalar dotnet-ef para poder usar migraciones dentro del contenedor
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Copiamos el resto del código
COPY . .

EXPOSE 8080

# Comando por defecto: dotnet watch
CMD ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:8080"]