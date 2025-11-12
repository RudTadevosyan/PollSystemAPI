# Poll System Backend – Documentation

A **RESTful WebAPI backend** for managing polls, voting, and result aggregation, built with **.NET 8** and following **Clean Architecture** principles. The system is organized into modular microservices, each responsible for a specific domain, and communicates through **event-driven patterns** using **RabbitMQ** and **MassTransit**.

It combines **caching, relational (PostgreSQL), and NoSQL (MongoDB) data stores** to ensure efficient, reliable, and consistent data handling. With synchronous validation, atomic transactions, and clear separation of concerns, it demonstrates a practical approach to structuring complex business logic, asynchronous workflows, and inter-service communication in a maintainable, scalable way.

---

## Overview

The Poll System Backend consists of independent services that handle different aspects of a polling platform:

- **AuthServiceAPI** – Handles user registration, authentication, JWT issuance, and user email lookups.
- **PollManagementAPI** – Core service for creating polls, managing options and votes, enforcing business rules, and publishing poll events.
- **ResultAggregationAPI** – Computes final poll results, stores them in MongoDB, and caches user emails in Redis for efficient notification delivery.
- **Shared Library** – Contains DTOs, events, and reusable specifications shared across services.

Services communicate asynchronously through **RabbitMQ** and **MassTransit**, while synchronous validation occurs through HTTP calls when necessary.

---

## Features

### Authentication & Authorization
- JWT-based authentication and role/policy support.
- Secure endpoints for creating polls, options, and votes.
- Inter-service JWT validation for internal requests.

### Poll Management
- Full CRUD for polls and options.
- Pagination and filtering for polls.
- Enforces one vote per user per poll.
- Atomic vote processing using EF Core transactions.

### Event-Driven Architecture
- Poll closure detected by Quartz.NET background jobs.
- `PollClosedEvent` triggers result computation.
- `GetVotersRequest` and `GetUserEmailRequest` fetch voter data for notifications.

### Result Aggregation
- Consumes `PollClosedEvent` and computes vote percentages.
- Stores results permanently in MongoDB.
- Caches user emails in Redis to reduce repeated requests to AuthService.

### Caching & Performance
- Redis caches user emails for 24 hours to reduce AuthService load.
- MongoDB serves as the persistent store for final poll results.

### Data Persistence
- PostgreSQL stores relational data (users, polls, votes) using EF Core.
- MongoDB stores computed poll results permanently until explicitly deleted.

### Docker & Development Setup
- Each service is containerized with Docker.
- Supports health checks, persistent volumes, and a shared network.
- Optional: Docker Compose configuration for running all services together.

### API Documentation
- Swagger UI available for each service for interactive testing.

---

## Architecture & Structure

### AuthServiceAPI
- **Domain:** User entity, specifications, exceptions.
- **Application:** Services for authentication, JWT, email lookup.
- **Infrastructure:** EF Core with PostgreSQL, MassTransit consumer for email queries.
- **API:** Controllers for registration, login, and internal email queries.

### PollManagementAPI
- **Domain:** Poll, Option, Vote entities, specifications, custom exceptions.
- **Application:** Services for polls, votes, AutoMapper profiles, event consumers.
- **Infrastructure:** Repositories, VotePollDbContext, Quartz jobs, PollClosedEvent publisher.
- **API:** Controllers with endpoints for polls, options, votes, and testing.

### ResultAggregationAPI
- **Domain:** DTOs, service interfaces for poll results.
- **Application:** PollResultService, PollClosedEventConsumer.
- **Infrastructure:** RedisCacheService for user emails, MongoDB repository for poll results, HttpClientFactory for synchronous validation.
- **API:** Endpoint for fetching final poll results with HTTP validation against PollManagementAPI.

---

## Technologies Used
- **.NET 8, C#**
- **ASP.NET Core WebAPI** (RESTful)
- **MassTransit & RabbitMQ** – asynchronous messaging
- **Quartz.NET** – scheduled poll closure
- **PostgreSQL & EF Core** – relational data persistence
- **MongoDB** – NoSQL storage for computed poll results
- **Redis** – caching of user emails
- **HttpClientFactory** – synchronous validation across services
- **Docker** – containerization


## Urls
- **PollManagmentAPI - http://localhost:44317/swagger
- **AuthServiceAPI - http://localhost:44307/swagger
- **ResultAggregationAPI - http://localhost:44333/swagger
---

## Getting Started

### 1. Prerequisites
- **Docker & Docker Compose**
- **.NET 8 SDK**

### 2. Clone the Repository
```bash
git clone https://github.com/RudTadevosyan/PollSystemAPI.git
```

### 3. Configure appsettings.json

- **AuthServiceAPI** PostgreSQL connection (host:postgres-auth, for Docker), JWT settings, RabbitMQ credentials.
- **PollManagementAPI** PostgreSQL connection (host:postgres-poll, for Docker), same JWT settings, RabbitMQ credentials.
- **ResultAggregationAPI** Redis connection, MongoDB connection and database name, RabbitMQ credentials, PollManagementAPI 's base URL

Docker note: do not use localhost; use the service names defined in Docker Compose.
