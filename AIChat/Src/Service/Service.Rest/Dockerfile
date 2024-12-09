# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Src/Service/Service.Rest/Service.Rest.csproj", "Src/Service/Service.Rest/"]
COPY ["Src/Application/Application.Command/Application.Command.csproj", "Src/Application/Application.Command/"]
COPY ["Src/Infrastructure/Infrastructure.Data.Repository.EfCore/Infrastructure.Data.Repository.EfCore.csproj", "Src/Infrastructure/Infrastructure.Data.Repository.EfCore/"]
COPY ["Src/Domain/Domain.Core/Domain.Core.csproj", "Src/Domain/Domain.Core/"]
COPY ["Src/Shared/Shared.Exception.Abstraction/Shared.Exception.Abstraction.csproj", "Src/Shared/Shared.Exception.Abstraction/"]
COPY ["Src/Domain/Domain.Base/Domain.Base.csproj", "Src/Domain/Domain.Base/"]
COPY ["Src/Application/Application.Service/Application.Service.csproj", "Src/Application/Application.Service/"]
COPY ["Src/Application/Application.Query/Application.Query.csproj", "Src/Application/Application.Query/"]
COPY ["Src/Infrastructure/Infrastructure.Services/Infrastructure.Services.csproj", "Src/Infrastructure/Infrastructure.Services/"]
COPY ["Src/Shared/Shared.MediatR/Shared.MediatR.csproj", "Src/Shared/Shared.MediatR/"]
COPY ["Src/Shared/Shared.Logging.Abstraction/Shared.Logging.Abstraction.csproj", "Src/Shared/Shared.Logging.Abstraction/"]
COPY ["Src/Shared/Shared.Middlewares/Shared.Middlewares.csproj", "Src/Shared/Shared.Middlewares/"]
RUN dotnet restore "./Src/Service/Service.Rest/Service.Rest.csproj"
COPY . .
WORKDIR "/src/Src/Service/Service.Rest"
RUN dotnet build "./Service.Rest.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Service.Rest.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Service.Rest.dll"]