# Architecture Diagrams

Use these as the **starting point** for Segment 5 (Mermaid round-trip).
Open the Markdown preview in VS Code to render.

---

## 1. Container view (current state)

```mermaid
flowchart LR
    User((User))
    SPA[React SPA<br/>Vite, :5173]
    API[.NET 8 Web API<br/>ShopApi, :5080]
    DB[(SQLite<br/>shop.db)]

    User -->|HTTPS| SPA
    SPA -->|REST /api| API
    API -->|EF Core| DB
```

---

## 2. Sequence — GET /api/orders/{id}

```mermaid
sequenceDiagram
    participant U as User
    participant R as React App
    participant C as OrdersController
    participant S as OrderService
    participant P as PricingService
    participant D as AppDbContext

    U->>R: click order #1
    R->>C: GET /api/orders/1
    C->>S: GetByIdAsync(1)
    S->>D: Orders.Include(Lines).First...
    D-->>S: Order
    C->>S: GetOrderTotalAsync(1)
    loop for each line
        S->>P: CalculateLinePrice(...)
        P-->>S: line total
    end
    S-->>C: total
    C-->>R: 200 { order, total }
    R-->>U: render
```

---

## 3. ER diagram

```mermaid
erDiagram
    PRODUCTS ||--o{ ORDER_LINES : "sold as"
    ORDERS ||--o{ ORDER_LINES : "contains"

    PRODUCTS {
        int    Id PK
        string Name
        decimal UnitPrice
        int    StockOnHand
    }
    ORDERS {
        int     Id PK
        string  CustomerName
        date    CreatedAt
        decimal DiscountPercent
        decimal TaxRate
    }
    ORDER_LINES {
        int     Id PK
        int     OrderId FK
        int     ProductId FK
        int     Quantity
        decimal UnitPriceSnapshot
    }
```

---

## Exercise — round-trip

**Step A — Code → Diagram.**
Ask Copilot to regenerate diagram #2 after you add a `StockService.Reserve` call inside `OrderService.CreateAsync`.

**Step B — Diagram → Code.**
Edit diagram #1 to add a `Redis[(Redis cache)]` node between `API` and `DB`.
Then ask Copilot for the minimal code-change plan (see prompt #7 in [PROMPTS.md](../PROMPTS.md)).
