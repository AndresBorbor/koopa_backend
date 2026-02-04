FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dev
WORKDIR /app

RUN apt-get update && apt-get install -y libxml2

# Copiamos el csproj y se restauran dependencias
COPY *.csproj ./
RUN dotnet restore

# Instalar dotnet-ef
RUN dotnet tool install --global dotnet-ef --version 8.*
ENV PATH="$PATH:/root/.dotnet/tools"

# Copiamos el resto del código
COPY . .

# En modo desarrollo (dotnet watch), el driver se copia dentro de bin/Debug/net8.0/clidriver/lib.
ENV LD_LIBRARY_PATH="/app/bin/Debug/net8.0/clidriver/lib:$LD_LIBRARY_PATH"

EXPOSE 8080

CMD ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:8080"]