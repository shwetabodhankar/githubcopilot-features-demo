-- SQLite schema for the workshop demo.
-- The .NET API creates this automatically via EnsureCreated(),
-- but the file is here so Copilot can ground SQL/ERD prompts on it.

CREATE TABLE Products (
    Id           INTEGER PRIMARY KEY AUTOINCREMENT,
    Name         TEXT    NOT NULL,
    UnitPrice    NUMERIC NOT NULL,
    StockOnHand  INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE Orders (
    Id              INTEGER PRIMARY KEY AUTOINCREMENT,
    CustomerName    TEXT    NOT NULL,
    CreatedAt       TEXT    NOT NULL,
    DiscountPercent NUMERIC NOT NULL DEFAULT 0,
    TaxRate         NUMERIC NOT NULL DEFAULT 0
);

CREATE TABLE OrderLines (
    Id                 INTEGER PRIMARY KEY AUTOINCREMENT,
    OrderId            INTEGER NOT NULL REFERENCES Orders(Id) ON DELETE CASCADE,
    ProductId          INTEGER NOT NULL REFERENCES Products(Id),
    Quantity           INTEGER NOT NULL,
    UnitPriceSnapshot  NUMERIC NOT NULL
);

CREATE INDEX IX_OrderLines_OrderId   ON OrderLines(OrderId);
CREATE INDEX IX_OrderLines_ProductId ON OrderLines(ProductId);

-- Seed
INSERT INTO Products (Name, UnitPrice, StockOnHand) VALUES
    ('Widget', 9.99, 100),
    ('Gadget', 19.50, 40),
    ('Gizmo',  4.25, 0);   -- out of stock — triggers BUG #3 demo
