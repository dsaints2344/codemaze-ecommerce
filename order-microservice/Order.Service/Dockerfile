# Build from the repository root so the shared local NuGet feed is in the build context:
#   docker build -f order-microservice/Order.Service/Dockerfile -t order-service .
FROM mcr.microsoft.com/dotnet/aspnet:10.0 as base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["shared-libs/local-nuget-packages/", "shared-libs/local-nuget-packages/"]
COPY ["order-microservice/nuget.config", "order-microservice/"]
COPY ["order-microservice/Order.Service/Order.Service.csproj", "order-microservice/Order.Service/"]
RUN dotnet restore "order-microservice/Order.Service/Order.Service.csproj"

COPY ["order-microservice/Order.Service/", "order-microservice/Order.Service/"]
WORKDIR "/src/order-microservice/Order.Service"
RUN dotnet build "Order.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build as publish
RUN dotnet publish "Order.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Order.Service.dll"]
