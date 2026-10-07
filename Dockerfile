# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar proyecto y restaurar paquetes (aprovechamiento de caché)
COPY ["tutorias.csproj", "./"]
RUN dotnet restore "tutorias.csproj"

# Copiar el resto del código fuente y compilar
COPY . .
RUN dotnet publish "tutorias.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Dependencias para generación de PDF y fuentes en Linux
RUN apt-get update && apt-get install -y --no-install-recommends \
    fontconfig \
    libfontconfig1 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "tutorias.dll"]