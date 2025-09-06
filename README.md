Small ASP.NET Core Web API for **User Management** with in‑memory persistence, clean layering, and unit tests.

## Tech & Structure

- .NET 8 (works with .NET 6+)
- Layers: **Controllers → Services → Repository (In-Memory)** + **Utils** (Password hashing)
- Patterns: Repository + Service and CQRS
- Validation: Data Annotations
- Swagger: enabled at `/swagger`
- Tests: xUnit (+ FluentAssertions)

````

## Run

```bash
# From the repo root
dotnet restore
dotnet build

# Run the API
dotnet run --project src/UserApi

# Swagger UI
# http://localhost:5197/swagger   (HTTP)
# https://localhost:7094/swagger (HTTPS)
````

## Endpoints

1. `GET /users?search=&page=&pageSize=` – list with search (name/email contains) + pagination.
2. `POST /users` – create (hashes password; rejects duplicate email → **409 Conflict**).
3. `PUT /users/{id}` – update name/title/email (maintains unique email).
4. `DELETE /users/{id}` – soft delete (email becomes reusable).
5. `POST /auth/sign-in` – email + password → returns a **fake JWT-shaped token** (no full JWT stack).

### Curl Examples

```bash
# Create
curl -s -X POST https://localhost:7094/users   -H "Content-Type: application/json"   -d '{"name":"Alice","title":"Engineer","email":"alice@test.com","password":"Passw0rd!"}'

# Duplicate email (should be 409)
curl -i -X POST https://localhost:7094/users   -H "Content-Type: application/json"   -d '{"name":"Alice 2","title":"Engineer","email":"alice@test.com","password":"Passw0rd!"}'

# List (page 1, pageSize 5)
curl -s "https://localhost:7094/users?page=1&pageSize=5"

# Update
curl -s -X PUT https://localhost:7094/users/{id}   -H "Content-Type: application/json"   -d '{"name":"Alice Updated","title":"Sr Engineer","email":"alice.updated@test.com"}'

# Delete
curl -i -X DELETE https://localhost:7094/users/{id}

# Sign-in
curl -s -X POST https://localhost:7094/auth/sign-in   -H "Content-Type: application/json"   -d '{"email":"alice.updated@test.com","password":"Passw0rd!"}'
```

## Tests

```bash
dotnet test
```

Includes at least 3 meaningful tests:

- Unique email rule (duplicate create throws / maps to 409)
- Password verification (hash + verify)
- Paging logic

## Design Choices & Trade-offs

- **In-memory repo** via `ConcurrentDictionary` with secondary email index for fast lookups + uniqueness.
- **Soft delete**: `IsDeleted` & `DeletedAt`. On delete, email is removed from the uniqueness index so it can be reused.
- **Password hashing**: BCrypt.Net.BCryp library
- **Fake token**: returns a JWT-like string without adding auth middleware complexity (keeps the scope small).
- **Validation**: Data Annotations for simplicity. Could swap to FluentValidation in a larger take-home.

## What I'd Improve With More Time

- Add FluentValidation + problem details conventions.
- Proper JWT issuance + authorization policies.
- More tests (controller tests, edge cases, search normalization).
- E2E tests + test fixtures for shared repo instance.
- Proper logging, metrics, and structured error responses.
- Persistence layer variants (EF Core, Dapper) behind the same repository interface.
