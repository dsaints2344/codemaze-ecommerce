# codemaze-ecommerce

My working repository for the [Code Maze — Microservices in .NET](https://courses.code-maze.com/courses/microservices-in-dotnet) course. I update it in place as I complete each chapter.

**Current state:** Chapter 3 — Microservice Communication.

## Services

| Service | Project | URL (Development) | Purpose |
| --- | --- | --- | --- |
| Basket | `basket-microservice/Basket.Service` | https://localhost:8000/swagger | Manages customer baskets; subscribes to `OrderCreated` events over RabbitMQ. |
| Order | `order-microservice/Order.Service` | https://localhost:8001/swagger | Manages orders; publishes `OrderCreated` events over RabbitMQ. |

Each service is its own solution (`Basket.Service.sln`, `Order.Service.sln`), uses .NET 9, minimal APIs, and an in-memory data store. Inter-service communication goes through a RabbitMQ event bus.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
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

Each service also has a `Dockerfile` if you prefer to run it in a container.
