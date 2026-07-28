# E-Commerce API

A production-style **ASP.NET Core Web API** for an e-commerce platform, built with **Clean Architecture** principles. It includes product catalog browsing, baskets, JWT authentication, order processing, and Stripe payment integration with signed webhooks.

---

## ✨ Features

- **Product Catalog** — browse products with filtering (by brand/type), sorting (name/price asc/desc), and server-side pagination.
- **JWT Authentication & Identity** — register/login, role-based access (Admin/SuperAdmin), current-user and address endpoints secured via `[Authorize]`.
- **Shopping Basket** — Redis-backed customer basket (create/update/delete), decoupled from the relational database for speed.
- **Orders** — order creation from a basket, delivery method selection, order history per user, order lookup by id.
- **Payments (Stripe)** — create/update `PaymentIntent`, and a **signature-verified webhook** endpoint that reacts to `payment_intent.succeeded` / `payment_intent.payment_failed` events to update order status.
- **Response Caching** — custom `[RedisCache]` action filter that caches `200 OK` JSON responses in Redis per request path + query string, with configurable TTL.
- **Consistent Error Handling** — a `Result` / `Result<T>` pattern throughout the Application layer, mapped to RFC 7807 `ProblemDetails` responses with correct HTTP status codes (404, 400, 409, 401, 403, 500).
- **Data Seeding** — automatic EF Core migration + JSON-based seeding for brands, types, products, delivery methods, and a default admin user on startup.
- **Specification Pattern** — encapsulated, reusable query logic (filtering, includes, ordering, pagination) kept out of controllers and repositories.

---

## 🏗️ Architecture

The solution follows **Clean Architecture**, with dependencies pointing inward toward `Domain`:

```
E_Commerce.API              → Controllers, middleware, DI composition, Program.cs
E_Commerce.Application      → Services, DTOs, Specifications, Result/Error pattern, contracts (interfaces)
E_Commerce.Infrastructure   → EF Core, Identity, Redis repositories, Stripe gateway, data seeding
E_Commerce.Domain           → Entities, base types, repository/UoW contracts (no external dependencies)
```

**Key patterns in use:**

| Pattern | Where |
|---|---|
| Specification Pattern | `Application/Specifications/*` — encapsulates `Include`, `Where`, `OrderBy`, and pagination per query |
| Generic Repository + Unit of Work | `Infrastructure/Repositories/GenericRepository`, `UnitOfWork` |
| Result Pattern | `Application/Common/Result.cs` — services return `Result` / `Result<T>` instead of throwing for expected failures |
| Action Filter (Caching) | `API/Attributes/RedisCacheAttribute.cs` — short-circuits the pipeline on cache hit |
| Options Pattern | `PaymentGatewaySettings`, `JwtSettings`, `UrlSettings` bound from configuration |
| Dependency Injection | Services registered via `AddInfrastructureServices` / `AddApplicationServices` extension methods |

---

## 🛠️ Tech Stack

- **ASP.NET Core Web API — .NET 8**
- **Entity Framework Core 8** with **SQL Server** — relational data access (products, orders, delivery methods)
- **ASP.NET Core Identity** — user accounts, roles, addresses
- **Redis** (`StackExchange.Redis`) — basket storage + response caching
- **Stripe.net** — payment intents + webhook signature verification
- **AutoMapper** — entity ↔ DTO mapping
- **JWT Bearer Authentication**
- **Swagger / OpenAPI** (Swashbuckle) — interactive API docs in development

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server** (LocalDB, full SQL Server, or a container)
- Redis instance (local via Docker, or a cloud instance)
- A [Stripe](https://dashboard.stripe.com/) account (test mode) for `SecretKey` / `WebhookSecret`

### Clone & Configure

```bash
git clone https://github.com/Zyad-Emad/ECommerce.git
cd ECommerce
```

Add your local secrets . Use `dotnet user-secrets` or a local `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=E_CommerceDb;Trusted_Connection=True;TrustServerCertificate=True",
    "IdentityConnection": "Server=.;Database=E_CommerceIdentityDb;Trusted_Connection=True;TrustServerCertificate=True",
    "Redis": "localhost:6379"
  },
  "JWT": {
    "SecretKey": "REPLACE_WITH_A_LONG_RANDOM_SECRET",
    "Issuer": "ECommerceAPI",
    "Audience": "ECommerceClient",
    "ExpirationInMinutes": 60
  },
  "Stripe": {
    "SecretKey": "sk_test_...",
    "DefaultCurrency": "usd",
    "WebhookSecret": "whsec_..."
  }
}
```

### Run

```bash
dotnet restore
dotnet build
dotnet run --project E_Commerce.API
```

On first run, the API automatically applies pending EF Core migrations and seeds initial catalog data (brands, types, products, delivery methods) and a default admin account.

Swagger UI will be available at `/swagger` in the Development environment.

### Testing Stripe Webhooks Locally

Use the [Stripe CLI](https://stripe.com/docs/stripe-cli) to forward events to your local webhook endpoint:

```bash
stripe listen --forward-to https://localhost:<port>/api/payments/webhook
```

---

## 📁 Project Structure (high level)

```
E_Commerce.Domain/
  Common/            → BaseEntity<TKey>
  Entities/          → Product, Order, Basket, Identity-related entities
  Contracts/         → IGenericRepository, IUnitOfWork, ISpecifications, IDataSeeder

E_Commerce.Application/
  Common/            → Result, Error, PaginatedResult, settings, query params
  Contracts/         → Service interfaces (IProductService, IOrderService, ...)
  DTOs/              → Request/response models
  Services/          → Business logic implementation
  Specifications/    → Query specifications (products, orders, counts, payment intent lookup)

E_Commerce.Infrastructure/
  Data/              → DbContext, EF configurations
  Identity/          → Identity DbContext, ApplicationUser, TokenService
  Repositories/      → GenericRepository, UnitOfWork, BasketRepository, CacheRepository
  Payments/          → StripePaymentGateway
  DataSeeding/       → CatalogDataSeeder, IdentityDataSeeder
  Specifications/    → SpecificationEvaluator (turns a spec into an IQueryable)

E_Commerce.API/
  Controllers/       → AuthenticationController, ProductsController, BasketsController,
                        OrdersController, PaymentsController
  Attributes/        → RedisCacheAttribute
  Extensions/        → WebApplicationExtension (seed & migrate on startup)
  Program.cs
```

---

## 📌 Key Endpoints

| Method | Route | Description |
|---|---|---|
| POST | `/api/authentication/register` | Register a new user |
| POST | `/api/authentication/login` | Log in and receive a JWT |
| GET | `/api/authentication/currentUser` | Get the current authenticated user |
| GET/PUT | `/api/authentication/address` | Get/update the current user's shipping address |
| GET | `/api/products` | List products (filter, search, sort, paginate) — cached |
| GET | `/api/products/{id}` | Get a single product with brand & type |
| GET | `/api/products/types` / `/brands` | Reference data for filters |
| GET/POST/DELETE | `/api/baskets/{id}` | Manage the current basket (Redis-backed) |
| POST | `/api/orders` | Create an order from a basket |
| GET | `/api/orders` / `/api/orders/{id}` | Order history / order detail |
| GET | `/api/orders/deliveryMethods` | Available delivery methods |
| POST | `/api/payments/{basketId}` | Create/update a Stripe PaymentIntent for a basket |
| POST | `/api/payments/webhook` | Stripe webhook (signature-verified) |

*(Full request/response contracts are documented in Swagger once the API is running.)*

---

## 👤 Author

**Zyad Emad**
[GitHub](https://github.com/Zyad-Emad) · [LinkedIn](https://www.linkedin.com/in/zyad-emad-5bb1a1339/) · [Email](zyad.emad19@gmail.com)
