# Build from the repository root so the shared local NuGet feed is in the build context:
#   docker build -f basket-microservice/Basket.Service/Dockerfile -t basket-service .
FROM mcr.microsoft.com/dotnet/aspnet:10.0 as base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["shared-libs/local-nuget-packages/", "shared-libs/local-nuget-packages/"]
COPY ["basket-microservice/nuget.config", "basket-microservice/"]
COPY ["basket-microservice/Basket.Service/Basket.Service.csproj", "basket-microservice/Basket.Service/"]
RUN dotnet restore "basket-microservice/Basket.Service/Basket.Service.csproj"

COPY ["basket-microservice/Basket.Service/", "basket-microservice/Basket.Service/"]
WORKDIR "/src/basket-microservice/Basket.Service"
RUN dotnet build "Basket.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build as publish
RUN dotnet publish "Basket.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Basket.Service.dll"]
