# Usar la imagen base de .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SupportApi.csproj", "."]
RUN dotnet restore "./SupportApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "SupportApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SupportApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .


# Copiar el appsettings.Development.json actualizado
COPY appsettings.Development.json ./appsettings.Development.json

# Crear el directorio Archivos
RUN mkdir -p /app/Archivos

ENTRYPOINT ["dotnet", "SupportApi.dll"]
