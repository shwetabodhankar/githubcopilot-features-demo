# GitHub Copilot — 45-Minute Hands-On Workshop

> Audience: developers new to GitHub Copilot
> Stack used in demos: **.NET 8 Web API + React (Vite) + SQLite (local DB)**
> Goal: by the end, attendees know *what Copilot can do*, *what it can't*, and *how to prompt it well* across the SDLC.

---

## Pre-flight (do this BEFORE the session)

- [ ] VS Code + GitHub Copilot + GitHub Copilot Chat extensions installed and signed in.
- [ ] .NET 8 SDK + Node 18+ installed.
- [ ] Run `dotnet restore` in `backend/`, `npm install` in `frontend/`.
- [ ] Open this repo at the workspace root so Copilot indexes everything.
- [ ] Have two windows ready: VS Code on left, browser (running the app) on right.
- [ ] Disable any "auto-accept suggestions" muscle memory — we want to *show* the suggestions.

---

## Agenda at a glance

| Time | Segment | What attendees see |
|------|---------|--------------------|
| 0:00 – 0:03 | Intro & ground rules | The mental model |
| 0:03 – 0:10 | **Basics — inline completions** | Ghost text, multi-line, Tab/Esc/Alt+] |
| 0:10 – 0:18 | **Copilot Chat fundamentals** | Inline chat, side panel, /commands, #variables, @participants |
| 0:18 – 0:27 | **Real SDLC demo #1 — fix a bug & explain code** | Backend null-ref bug, /explain, /fix |
| 0:27 – 0:35 | **Real SDLC demo #2 — improve test coverage** | /tests on `OrderService`, edge cases |
| 0:35 – 0:40 | **Architecture & docs — Mermaid round-trip** | Generate diagram → change code → regenerate; change diagram → ask Copilot to refactor |
| 0:40 – 0:43 | **Good vs Bad prompting** | Side-by-side examples (see `PROMPTS.md`) |
| 0:43 – 0:45 | **What Copilot can't do (yet) + Q&A** | Honest limits |

---

## 0:00 — Intro (3 min)

**Say:**
> "Copilot is a *pair programmer*, not an *autopilot*. It's brilliant at the next 10 lines, decent at the next 100, and dangerous past that if you don't review. Today we'll learn the four surfaces — inline, inline chat, panel chat, and agent — and when to use each."

**Show this slide / whiteboard:**

```
       ┌─────────────────────────────────────────┐
       │  WHAT  →  HOW  →  CONSTRAINTS  →  CHECK │
       └─────────────────────────────────────────┘
            The 4 parts of every good prompt
```

Mention the **trust ladder**: completion → chat suggestion → edits across files → agent mode. Higher up = more power, more review needed.

---

## 0:03 — Segment 1: Inline Completions (7 min)

### Demo 1a — Ghost text from a comment
Open `backend/Services/PricingService.cs`. The class is empty except for a comment:

```csharp
// Calculate the final price for an order line:
// - apply percentage discount (0-100)
// - apply flat tax rate (0-1)
// - never return negative; throw ArgumentOutOfRangeException on bad input
public decimal CalculateLinePrice(decimal unitPrice, int quantity, decimal discountPercent, decimal taxRate)
```

> Pause on the signature. Press **Enter**. Show ghost text.
> Press **Tab** to accept. Then **Alt+]** / **Alt+[** to cycle alternates.

**Teaching point:** Copilot reads the comment + signature + neighboring files. The richer the context, the better the suggestion. *"Garbage in, garbage out — but also, no context in, generic out."*

### Demo 1b — Completion from tests
Open `backend.Tests/PricingServiceTests.cs`. Type:

```csharp
[Fact]
public void CalculateLinePrice_NegativeQuantity_Throws()
```

Watch Copilot complete the body. Point out it inferred the *intent* from the name.

**Key shortcuts to put on screen:**
- `Tab` — accept
- `Esc` — dismiss
- `Alt+]` / `Alt+[` — next / previous suggestion
- `Ctrl+→` — accept one word at a time
- `Ctrl+Enter` — open the **completions panel** (10 alternatives side-by-side)

### Limit to call out
Type a function that needs the *exact* business rule from a Confluence page. It'll invent something plausible-but-wrong. **"If the rule lives outside the repo, you must paste it in."**

---

## 0:10 — Segment 2: Copilot Chat Fundamentals (8 min)

### The four surfaces

| Surface | How to open | Best for |
|---------|-------------|----------|
| Inline ghost text | just type | next few lines |
| **Inline Chat** | `Ctrl+I` | "change this selection" |
| **Chat Panel** | `Ctrl+Alt+I` | Q&A, multi-file, planning |
| **Agent mode** | Panel → mode dropdown → *Agent* | "do the whole task, edit files, run terminal" |

### Slash commands (show in panel)
- `/explain` — what does this do?
- `/fix` — fix the problem (often paired with a selection or error)
- `/tests` — generate tests for selection
- `/doc` — add docstring / XML doc
- `/new` — scaffold a new project or file
- `/clear` — wipe chat memory (use this when context goes stale!)

### Chat variables (the `#` ones)
- `#file:<name>` — attach a specific file
- `#selection` — current editor selection
- `#editor` — the whole active file
- `#codebase` — let Copilot search the workspace (slower, more powerful)
- `#terminalLastCommand` — attach what just ran
- `#problems` — current diagnostics

### Chat participants (the `@` ones)
- `@workspace` — answers grounded in your repo (use this 80% of the time)
- `@vscode` — "how do I do X in VS Code?"
- `@terminal` — shell commands

> **Demo:** in the panel, type
> `@workspace what does the OrdersController do and which services does it call?`
> Show how it cites files. Click the citations.

---

## 0:18 — Segment 3: SDLC Demo #1 — Diagnose & Fix (9 min)

Open the running React app. Click an order — it 500s. Open the terminal: `NullReferenceException` in `OrderService.GetOrderTotal`.

### Step 1 — Understand before changing
1. Open `OrderService.cs`, select `GetOrderTotal`.
2. `Ctrl+I` → `/explain`
3. Read the explanation aloud. Note what it gets right and where it hand-waves.

### Step 2 — Use #problems to ground the fix
In the chat panel:
```
/fix #problems #file:OrderService.cs
The endpoint throws NullReferenceException when an order has no line items.
The fix must:
- treat a null or empty Lines collection as total = 0
- not change the public method signature
- include a unit test in OrderServiceTests.cs
```

> **Teaching point — the WHAT / HOW / CONSTRAINTS / CHECK structure.**
> Bad prompt: *"fix the bug"*.
> Good prompt: the one above — it tells Copilot the *symptom*, the *invariants*, and the *deliverable*.

### Step 3 — Review the diff like a PR
- Don't blind-accept. Use the diff view.
- Run the test. Click the order again in the browser.

### Bonus — `/explain` for onboarding
Right-click a folder → "Copilot: Explain this folder" (or in chat: `@workspace explain the structure of /backend`). Great for new hires.

---

## 0:27 — Segment 4: SDLC Demo #2 — Improve Test Coverage (8 min)

> **Full script with copy-paste prompts and coverage commands:** [COVERAGE.md](COVERAGE.md).

Open `PricingService.cs`. Currently only the happy path is tested.

### Step 1 — Ask for a coverage gap analysis
In the panel:
```
@workspace #file:PricingService.cs #file:PricingServiceTests.cs
List the branches and edge cases in PricingService that are NOT covered by the existing tests. Output as a markdown table: Scenario | Inputs | Expected | Currently tested (Y/N).
```

### Step 2 — Generate the missing tests
Select the un-tested branch. `Ctrl+I` → `/tests`.

Then refine:
```
Also add a theory with 5 boundary cases for discountPercent: -1, 0, 50, 100, 101.
Use FluentAssertions style. Keep test names in the Method_Scenario_Expected pattern.
```

### Step 3 — Run coverage
```
@terminal run dotnet test with coverage and show me the lines still uncovered in PricingService
```

**Teaching point:** Copilot is *great* at writing tests but *bad* at deciding what's worth testing. You bring the judgment; it brings the typing speed.

---

## 0:35 — Segment 5: Architecture & Mermaid Round-Trip (5 min)

### Direction 1 — Code → Diagram
In chat:
```
@workspace Generate a Mermaid C4-style container diagram for this solution.
Include: React SPA, .NET API, SQLite file, and the main controllers/services.
Use flowchart LR syntax. Render in a fenced ```mermaid block.
```

Paste output into `diagrams/architecture.md`. Open the Markdown preview — it renders.

### Direction 2 — Diagram → Code
Edit the diagram: add a new node `Redis[(Redis cache)]` between API and DB. Then:
```
@workspace #file:diagrams/architecture.md
The diagram now shows a Redis cache between the API and SQLite.
Propose the minimal code changes to introduce IOrderCache, a Redis-backed implementation,
and DI registration. Don't implement Redis client yet — stub it. Show me a plan first, then the diff.
```

> **Teaching point:** *"Plan first, then diff"* — always ask for the plan when changes span multiple files. You'll catch wrong assumptions before they become wrong code.

### Other diagram types to mention
- `sequenceDiagram` — for a single API call's flow
- `erDiagram` — generate from your EF Core models or SQL schema
- `stateDiagram-v2` — for order state machines

### Bonus (if time) — Swagger / OpenAPI
See [SWAGGER.md](SWAGGER.md) for a 10-prompt playbook that takes Swagger from "bare" to "production-ready" (XML docs → `[ProducesResponseType]` → ProblemDetails → generated TS client). Pick **prompt #2 and #3** for a 90-second taste.

---

## 0:40 — Segment 6: Good vs Bad Prompting (3 min)

Open `PROMPTS.md` and walk through 3 paired examples live. Key rules:

1. **Give it a role + a goal.** *"You are reviewing a PR. Find security issues in this diff."*
2. **Constrain the output.** *"Return only the changed methods, no prose."*
3. **Anchor to files.** Use `#file:` instead of pasting.
4. **One task per turn.** Bundle = bad diff.
5. **Iterate, don't restart.** Refine the last answer; don't re-prompt from scratch.

---

## 0:43 — Segment 7: What Copilot CAN and CAN'T Do (2 min)

### Strong at
- Boilerplate, scaffolding, glue code
- Translating between languages / frameworks
- Writing tests for existing code
- Explaining unfamiliar code & errors
- Regex, SQL, shell one-liners
- Refactoring within a file
- Generating diagrams & docs from code

### Weak at (verify everything!)
- **Anything requiring external knowledge it can't see** (your auth provider's exact config, your company's style guide unless you attach it).
- **Numerical correctness** — financial math, dates/timezones, units.
- **Security-sensitive code** — it will happily write SQL injection-friendly code if you don't constrain it.
- **License-aware coding** — it doesn't track licenses of suggestions.
- **Large refactors across many files** — agent mode helps, but plan + review in chunks.
- **Knowing it's wrong** — it rarely says "I don't know." Treat confidence as a *style*, not a *signal*.

### The one rule
> **You are responsible for every line you accept.** Copilot speeds up typing; it doesn't transfer accountability.

---

## Appendix — Files in this workshop kit
COVERAGE.md` — full test-coverage exercise (gap analysis → /tests → coverage report)
- `SWAGGER.md` — Swagger/OpenAPI prompt playbook
- `
- `WORKSHOP.md` — this script
- `PROMPTS.md` — good vs bad prompts, cheat sheet
- `backend/` — .NET 8 Web API with seeded bugs & gaps
- `frontend/` — React (Vite) app
- `database/schema.sql` — SQLite schema + seed
- `diagrams/architecture.md` — starter Mermaid diagrams
- `BUGS.md` — the planted bugs (instructor reference — don't show attendees)
