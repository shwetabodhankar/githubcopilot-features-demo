# Prompting Cheat Sheet — Good vs Bad

> Rule of thumb: a great prompt has **WHAT** (the task), **HOW** (the approach / style), **CONSTRAINTS** (don't change X, must use Y), and **CHECK** (how you'll know it worked).

---

## 1. Fixing a bug

### Bad
```
fix the bug in OrderService
```
Why bad: no symptom, no file, no acceptance criteria. Copilot will guess.

### Good
```
/fix #file:OrderService.cs #problems
Symptom: GET /api/orders/{id} throws NullReferenceException when the order has no line items.
Fix:
- treat null or empty Lines as total = 0
- do NOT change the public signature of GetOrderTotal
- add a unit test in OrderServiceTests.cs named GetOrderTotal_NoLines_ReturnsZero
Output: a unified diff, no prose.
```

---

## 2. Generating tests

### Bad
```
write tests for this
```

### Good
```
/tests
Generate xUnit tests for the selected method using FluentAssertions.
Naming pattern: Method_Scenario_ExpectedResult.
Cover: happy path, null inputs, negative quantity, discount > 100, tax rate of 0 and 1.
Use [Theory] + [InlineData] where it reduces duplication.
Do not mock — this is a pure function.
```

---

## 3. Explaining unfamiliar code

### Bad
```
what does this do
```

### Good
```
/explain
Explain this method for a developer new to the codebase.
Cover: purpose, inputs, outputs, side effects, and any non-obvious assumptions.
Call out anything that looks like a bug or smell. Keep it under 150 words.
```

---

## 4. Refactoring

### Bad
```
make it cleaner
```

### Good
```
Refactor the selection to:
- extract the discount logic into a private method ApplyDiscount(decimal, decimal)
- replace the if/else chain with a switch expression
- keep behavior identical (the existing tests must still pass)
Show me the diff only.
```

---

## 5. Generating a new feature (multi-file)

### Bad
```
add a cart feature
```

### Good
```
@workspace
Goal: add a Cart feature to the API.
Scope:
- new entity Cart { Id, UserId, Items: List<CartItem> } in /backend/Models
- ICartRepository + EF Core implementation
- CartController with GET /api/cart/{userId}, POST /api/cart/{userId}/items, DELETE /api/cart/{userId}/items/{itemId}
- DI registration in Program.cs
- xUnit tests for the controller using WebApplicationFactory
Constraints:
- match the patterns used in OrdersController and OrderService
- use the existing AppDbContext, add a DbSet<Cart>
- do NOT add new NuGet packages
Step 1: show me the PLAN as a checklist. I will approve before you write code.
```

> Always ask for **a plan first** when the change spans >2 files.

---

## 6. SQL / data

### Bad
```
query for top customers
```

### Good
```
Write a SQLite query against this schema (#file:database/schema.sql).
Return the top 10 customers by total order value in the last 90 days.
Columns: customer_id, customer_name, order_count, total_spent.
Sort by total_spent desc.
Use parameterized date filters (:cutoff_date) — no string concatenation.
```

---

## 7. Mermaid diagrams

### Bad
```
draw a diagram
```

### Good — code → diagram
```
@workspace Generate a Mermaid sequenceDiagram for the flow:
React calls POST /api/orders -> OrdersController -> OrderService.CreateOrder
-> PricingService.CalculateLinePrice -> AppDbContext.SaveChanges -> response.
Include error path when stock is insufficient.
Output only the fenced ```mermaid block.
```

### Good — diagram → code
```
@workspace #file:diagrams/architecture.md
I added a Redis cache node between API and DB.
Propose the minimal set of code changes (file list + 1-line rationale each).
Do not write code yet — wait for my approval.
```

---

## 8. Security review

### Bad
```
is this secure
```

### Good
```
Act as a security reviewer.
Scan the selection for: SQL injection, XSS, missing authZ checks,
unvalidated input, secrets in code, insecure deserialization.
For each finding: severity (Low/Med/High), exact line, fix suggestion.
Ignore style issues.
```

---

## 9. Commit messages & PR descriptions

### Bad
```
write a commit message
```

### Good
```
@workspace #changes
Write a Conventional Commits message and a PR description.
PR description sections: Summary, Motivation, Changes (bulleted), Testing, Risk.
Keep summary under 72 chars.
```

(`#changes` includes the current git diff.)

---

## 10. When the answer is wrong — recover, don't restart

### Bad
Start a new chat with a slightly different wording. (You lose all context.)

### Good
Reply in the same thread:
```
That's close, but:
- the tax should be applied AFTER the discount, not before
- keep the method static
Regenerate only the CalculateLinePrice method.
```

---

## Quick reference

| Symbol | What it does | Example |
|--------|--------------|---------|
| `/explain` | Explain selection | `/explain` |
| `/fix` | Fix problem | `/fix #problems` |
| `/tests` | Generate tests | `/tests` |
| `/doc` | Add docs | `/doc` |
| `/new` | Scaffold project | `/new react+vite app` |
| `/clear` | Reset chat memory | `/clear` |
| `#file:foo.cs` | Attach a file | `#file:OrderService.cs` |
| `#selection` | Current selection | implicit in inline chat |
| `#editor` | Whole active file | |
| `#codebase` | Search the repo | slower but powerful |
| `#problems` | Current diagnostics | great with `/fix` |
| `#terminalLastCommand` | Last terminal output | pair with `@terminal` |
| `#changes` | Current git diff | great for commit msgs |
| `@workspace` | Repo-grounded answers | default for code Qs |
| `@vscode` | VS Code how-to | `@vscode how do I split editor` |
| `@terminal` | Shell help | `@terminal kill port 5000` |
