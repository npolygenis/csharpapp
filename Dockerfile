#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/CSharpApp.Api/CSharpApp.Api.csproj", "src/CSharpApp.Api/"]
COPY ["src/CSharpApp.Application/CSharpApp.Application.csproj", "src/CSharpApp.Application/"]
COPY ["src/CSharpApp.Core/CSharpApp.Core.csproj", "src/CSharpApp.Core/"]
COPY ["src/CSharpApp.Infrastructure/CSharpApp.Infrastructure.csproj", "src/CSharpApp.Infrastructure/"]
RUN dotnet restore "./src/CSharpApp.Api/CSharpApp.Api.csproj"
COPY . .
WORKDIR "/src/src/CSharpApp.Api"
RUN dotnet build "./CSharpApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CSharpApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CSharpApp.Api.dll"]
