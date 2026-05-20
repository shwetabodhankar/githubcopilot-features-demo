# Test Coverage Demo

A guided exercise: use Copilot to go from **one happy-path test** to **branch-complete coverage** on `PricingService` and `OrderService`, then prove it with a coverage report.

> Pair this with **Segment 4** of [WORKSHOP.md](WORKSHOP.md).

---

## 0. Baseline — what you have today

```powershell
cd backend.Tests
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

Output drops a `coverage.cobertura.xml` under `TestResults\<guid>\`. Install a report viewer once:

```powershell
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
start coveragereport\index.html
```

You'll see ~30–40% line coverage on `PricingService` and `OrderService`. **That's the demo's starting point.**

---

## 1. Ask Copilot for a gap analysis (don't ask for tests yet)

In the chat panel:

```
@workspace #file:backend/Services/PricingService.cs #file:backend.Tests/PricingServiceTests.cs

Analyze branch and edge-case coverage for PricingService.
Output a markdown table with columns:
| Scenario | Inputs | Expected | Currently tested? |

Then list, separately, any *behavior* that the comment on the interface promises
but the implementation does NOT enforce. Be specific about line numbers.
```

> **Teaching point:** asking for a **table first** keeps Copilot from leaping into code based on the wrong assumptions. You get to correct the plan cheaply.

You'll see it flag the missing guard clauses (negative qty, discount > 100, etc.).

---

## 2. Have Copilot generate the missing tests

Select the existing `PricingServiceTests` class. `Ctrl+I`:

```
/tests

Add tests to cover the gaps you identified.
Constraints:
- xUnit + FluentAssertions, no Moq
- naming: Method_Scenario_ExpectedResult
- use [Theory] + [InlineData] where it reduces duplication
- one assert per test where reasonable
- group guard-clause tests under a #region "Guard clauses"
- do NOT modify the existing happy-path test
```

Review the diff. **Reject anything that tests Copilot's hallucinated rules** (e.g. "throws on `quantity == 0`") if those rules aren't in the interface contract. Either:
- accept and update the interface contract, or
- reply: *"Drop the `quantity == 0` cases — the contract doesn't require that."*

---

## 3. Update `PricingService` so the new tests pass

The guard-clause tests will fail because `PricingService` doesn't validate inputs. Select the method and:

```
/fix
The new tests in #file:backend.Tests/PricingServiceTests.cs expect ArgumentOutOfRangeException
for: negative unitPrice, negative quantity, discountPercent outside [0,100], taxRate outside [0,1].
Add the guard clauses at the top of CalculateLinePrice. Throw with a paramName and a meaningful message.
Do not change the calculation logic.
```

Run the tests. Re-run coverage. Numbers should jump.

---

## 4. Same flow for `OrderService` — but with the DB

The `OrderService` tests need an in-memory `AppDbContext`. The test project already references `Microsoft.EntityFrameworkCore.InMemory`. Prompt:

```
@workspace #file:backend/Services/OrderService.cs #file:backend.Tests/OrderServiceTests.cs

Generate tests that cover:
1. GetOrderTotalAsync returns 0 when the order has null or empty Lines (this is currently a bug — write the test that PROVES the bug, mark it [Fact(Skip="bug #1")] until /fix lands)
2. GetOrderTotalAsync returns null/throws KeyNotFound when the order id doesn't exist (decide which and be consistent)
3. CreateAsync rejects an unknown productId
4. CreateAsync rejects a quantity that exceeds StockOnHand

Use a fresh InMemory DbContext per test (factory helper already exists: NewDb()).
Use the real PricingService — these are integration-ish unit tests.
```

This intentionally surfaces **bugs #1, #3, #4** from [BUGS.md](BUGS.md). Use `/fix` segment-by-segment to repair the service, removing the `Skip=` as each one goes green.

---

## 5. Add a controller integration test (one is enough for the demo)

The test project already references `Microsoft.AspNetCore.Mvc.Testing`. Prompt:

```
@workspace #file:backend/Controllers/OrdersController.cs

Generate one integration test using WebApplicationFactory<Program>:
- POST /api/orders with two valid line items
- assert: 201 Created, response body has Id > 0, Total > 0
- use a fresh SQLite InMemory or :memory: connection so it doesn't touch shop.db

Show me the WebApplicationFactory subclass and the test class in two separate code blocks.
```

> **Teaching point:** Copilot is excellent at boilerplate like `WebApplicationFactory` overrides — exactly the code humans get wrong because they only write it once a year.

---

## 6. Prove it — re-run coverage and diff

```powershell
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

Show the before/after coverage % to the room. This is the **payoff moment** of the segment.

---

## Reference — what "good" looks like

Here's roughly the shape Copilot should produce for `PricingService`. Use it as your mental yardstick when reviewing its output (don't paste this into the test file — let Copilot generate it live).

```csharp
public class PricingServiceTests
{
    private readonly PricingService _sut = new();

    [Fact]
    public void CalculateLinePrice_HappyPath_ReturnsDiscountedThenTaxed()
    {
        _sut.CalculateLinePrice(10m, 2, 10m, 0.08m).Should().Be(19.44m);
    }

    [Theory]
    [InlineData(0,   1, 0,   0,    0)]      // free product
    [InlineData(100, 0, 0,   0,    0)]      // zero qty -> 0
    [InlineData(10,  3, 100, 0,    0)]      // 100% discount -> 0
    [InlineData(10,  1, 0,   1,    20)]     // 100% tax doubles price
    public void CalculateLinePrice_Boundaries(decimal price, int qty, decimal disc, decimal tax, decimal expected)
        => _sut.CalculateLinePrice(price, qty, disc, tax).Should().Be(expected);

    #region Guard clauses
    [Theory]
    [InlineData(-1,  1, 0,   0)]
    [InlineData(10, -1, 0,   0)]
    [InlineData(10,  1, -1,  0)]
    [InlineData(10,  1, 101, 0)]
    [InlineData(10,  1, 0,  -0.01)]
    [InlineData(10,  1, 0,   1.01)]
    public void CalculateLinePrice_InvalidInput_Throws(decimal price, int qty, decimal disc, decimal tax)
    {
        Action act = () => _sut.CalculateLinePrice(price, qty, disc, tax);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
    #endregion
}
```

---

## Talking points to drop in live

- *"Copilot writes the tests; **you** decide what 'tested' means."*
- *"A coverage % without a contract is theater. Pin down the contract first."*
- *"`[Theory]` + `[InlineData]` is where Copilot saves the most typing — embrace it."*
- *"When a generated test fails, **read the test before you change the code.** Half the time the test is wrong."*
