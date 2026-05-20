# Planted Bugs & Demo Gaps — INSTRUCTOR ONLY

> Do **not** share with attendees. This is the answer key so you can drive the live demos confidently.

## Backend bugs

| # | File | Symptom | Demo segment | Prompt to use |
|---|------|---------|--------------|---------------|
| 1 | `Services/OrderService.cs` `GetOrderTotalAsync` | `NullReferenceException` when `order.Lines` is null (Bob's seeded order, id=2) | Segment 3 — /fix | See `PROMPTS.md` §1 |
| 2 | Same | No null-check on `order` either — returns NRE instead of 404 | Optional follow-up | "also handle the case where the order does not exist; return null" |
| 3 | `Services/OrderService.cs` `CreateAsync` | No stock validation — can order Gizmo (stock=0) | Stretch demo | "add stock validation; throw InvalidOperationException with message including productId and requested qty" |
| 4 | Same | No null-check on `product` | Pairs with #3 | combine in same prompt |
| 5 | `Controllers/ProductsController.cs` `Search` | **SQL injection** via interpolated `FromSqlRaw` | Security prompt demo | See `PROMPTS.md` §8 |

## Test coverage gaps (PricingService)

Only the happy path is tested. Missing:
- negative `unitPrice`
- negative `quantity` (should throw)
- `discountPercent` < 0 or > 100 (should throw)
- `taxRate` < 0 or > 1 (should throw)
- `discountPercent == 100` (result should be 0 + tax of 0 = 0)
- very large values (overflow consideration)
- zero quantity (decide: throw or return 0?)

## Test coverage gaps (OrderService)

- order not found → currently NRE, should be `null` or 404
- order with null/empty Lines → bug #1
- create with zero items → currently builds empty order, should probably reject
- create with unknown productId → bug #4

## Frontend gaps

- `App.jsx` doesn't show a friendly error for the 500 (it does catch, but the UX is minimal).
- No loading state.
- No way to create an order — good "/new feature" demo: ask Copilot to add a "New order" form.

## Suggested live-demo order

1. Run app. Click Bob's order. Show the 500. → Segment 3.
2. /explain `GetOrderTotalAsync`, then /fix with the structured prompt.
3. Re-run, verify Bob now returns total=0.
4. Move to `PricingService` for /tests. Ask for the gap table first, then generate the tests.
5. Open `architecture.md`, do the Mermaid round-trip.
6. (If time) Show `/Search` SQL-injection prompt as the security cherry-on-top.
