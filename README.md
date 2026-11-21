# codemaze-ecommerce

My working repository for the [Code Maze — Microservices in .NET](https://courses.code-maze.com/courses/microservices-in-dotnet) course. I update it in place as I complete each chapter.

**Current state:** Chapter 3 — Microservice Communication.

## Services

| Service | Project | URL (Development) | Purpose |
| --- | --- | --- | --- |
| Basket | `basket-microservice/Basket.Service` | https://localhost:8000/swagger | Manages customer baskets; subscribes to `OrderCreated` events over RabbitMQ. |
| Order | `order-microservice/Order.Service` | https://localhost:8001/swagger | Manages orders; publishes `OrderCreated` events over RabbitMQ. |

Each service is its own solution (`Basket.Service.sln`, `Order.Service.sln`), uses .NET 10, minimal APIs, and an in-memory data store. Inter-service communication goes through a RabbitMQ event bus.

## Shared library

The RabbitMQ connection, event bus and base `Event` record live in `shared-libs/ECommerce.Shared` and reach the services as the `ECommerce.Shared` NuGet package, not as a project reference. The package is committed in the local feed `shared-libs/local-nuget-packages`, and each service's `nuget.config` points at that folder, so a fresh clone restores without any extra setup.

To publish a change, pack it under a **new** version, push it to the feed, then bump the `ECommerce.Shared` reference in the services that need it. NuGet treats versions as immutable: any machine that already restored a version keeps using its cached copy, even if the file in the feed is replaced.

```bash
cd shared-libs/ECommerce.Shared
dotnet pack -p:Version=1.0.1
dotnet nuget push bin/Release/ECommerce.Shared.1.0.1.nupkg -s ../local-nuget-packages
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Docker (for RabbitMQ)

## Running locally

1. Start RabbitMQ (the services expect it at `host.docker.internal`, configured in each `appsettings.json`):

   ```bash
   docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
   ```

   Management UI: http://localhost:15672 (guest / guest).

2. Run each service in its own terminal:

   ```bash
   dotnet run --project basket-microservice/Basket.Service
   ```

   ```bash
   dotnet run --project order-microservice/Order.Service
   ```

3. Open the Swagger UI for each service at the URLs above. Creating an order in the Order service publishes an `OrderCreated` event that the Basket service consumes.

Each service also has a `Dockerfile` if you prefer to run it in a container. Build the images from the repository root, so the shared local NuGet feed is part of the build context:

```bash
docker build -f basket-microservice/Basket.Service/Dockerfile -t basket-service .
docker build -f order-microservice/Order.Service/Dockerfile -t order-service .
```
