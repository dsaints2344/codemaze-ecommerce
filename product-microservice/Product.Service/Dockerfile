# Build from the repository root so the shared local NuGet feed is in the build context:
#   docker build -f product-microservice/Product.Service/Dockerfile -t product-service .
FROM mcr.microsoft.com/dotnet/aspnet:10.0 as base
WORKDIR /app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["shared-libs/local-nuget-packages/", "shared-libs/local-nuget-packages/"]
COPY ["product-microservice/nuget.config", "product-microservice/"]
COPY ["product-microservice/Product.Service/Product.Service.csproj", "product-microservice/Product.Service/"]
RUN dotnet restore "product-microservice/Product.Service/Product.Service.csproj"
COPY ["product-microservice/Product.Service/", "product-microservice/Product.Service/"]
WORKDIR "/src/product-microservice/Product.Service"
RUN dotnet build "Product.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build
FROM build as publish
RUN dotnet publish "Product.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish
FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Product.Service.dll"]
