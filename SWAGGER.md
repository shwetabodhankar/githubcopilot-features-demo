# Swagger / OpenAPI Prompt Playbook

How to use Copilot to take an API from *"Swagger renders something"* to *"Swagger is good enough to hand to a client team."*

> Pair this with the **last 5 min** of Segment 5 or as a bonus segment. Swashbuckle is already wired up in `Program.cs`; Swagger UI is at <http://localhost:5080/swagger>.

---

## What "good Swagger" looks like

A reviewer should be able to answer these from the Swagger page alone:

1. What does each endpoint **do** (summary + remarks)?
2. What's the **request shape**, with field meanings and examples?
3. What **status codes** can come back, and what does each body look like?
4. What **auth** is required?
5. What are the **error contracts** (ProblemDetails / RFC 7807)?

We'll prompt Copilot toward each one.

---

## 1. Turn on XML doc generation

Prompt (panel):

```
@workspace #file:backend/ShopApi.csproj #file:backend/Program.cs

I want Swashbuckle to surface XML doc comments in Swagger UI.
Step 1: show me the csproj changes (GenerateDocumentationFile, NoWarn 1591).
Step 2: show me the Program.cs change to call c.IncludeXmlComments(...) inside AddSwaggerGen.
Use Path.Combine with AppContext.BaseDirectory and the assembly name — don't hardcode.
Output as a unified diff.
```

Accept. Rebuild. Notice Swagger UI is still bare because no `///` comments exist yet — that's the next step.

---

## 2. Generate XML doc comments for controllers

Open `OrdersController.cs`. Select the class. `Ctrl+I`:

```
/doc

Add XML doc comments to every public action in this controller.
Each action must have:
- <summary> one sentence, imperative ("Returns…", "Creates…")
- <param> for every route/query/body parameter
- <returns> describing the success body
- <response code="200"> etc. for every status code the action can produce

Match the existing code style. Don't change behavior.
```

Do the same for `ProductsController`. Open `/swagger` — descriptions now show.

---

## 3. Declare response types (the part everyone forgets)

Same controller, select an action. `Ctrl+I`:

```
Add [ProducesResponseType] attributes covering every realistic outcome of this action.
Include the response DTO type for success codes and ProblemDetails for 4xx/5xx.
Use the typed overload: [ProducesResponseType(typeof(...), StatusCodes.Status200OK)].
Also add [Produces("application/json")] on the controller if not already present.
```

Now the Swagger "Responses" table is actually useful.

---

## 4. Add request/response examples

Swashbuckle examples need either `Swashbuckle.AspNetCore.Filters` or example providers. Prompt:

```
@workspace
Add Swashbuckle examples for OrdersController.Create.
Plan first:
- which NuGet package to add (smallest footprint),
- where to register it,
- where the example provider class goes (suggest /backend/Swagger/Examples/).
Wait for my approval before writing code.
```

> **Teaching point:** *"plan first"* prompts catch wrong-package suggestions before they hit your csproj.

After approval:

```
Implement the plan. Create OrderCreateRequestExample with two realistic line items.
Don't add other examples yet — I want to see one full vertical slice first.
```

---

## 5. Standardize error responses with ProblemDetails

```
@workspace #file:backend/Program.cs #file:backend/Controllers/OrdersController.cs

I want every error response to be RFC 7807 ProblemDetails.
1. Enable AddProblemDetails() and wire UseExceptionHandler with a ProblemDetails writer.
2. Map common exceptions (ArgumentOutOfRangeException -> 400, KeyNotFoundException -> 404,
   InvalidOperationException for stock issues -> 409) to ProblemDetails with a stable `type` URI.
3. Update controllers to throw rather than return manually-shaped errors.
4. Update [ProducesResponseType] entries to reference ProblemDetails.

Show me the plan + file list before code.
```

---

## 6. Tag, group, and order endpoints

```
@workspace
In AddSwaggerGen, configure:
- tag controllers by a custom display name (Orders -> "Orders & Checkout")
- sort actions by HTTP verb then path
- group versions if any exist (v1 only today, but make it future-proof)

Show me only the lambda body inside AddSwaggerGen.
```

---

## 7. Generate a strongly-typed TypeScript client for the React app

Two options — let Copilot suggest one based on context, then implement:

```
@workspace #file:frontend/package.json #file:frontend/src/api.js

I want to replace the hand-rolled fetch calls in api.js with a generated TypeScript client
built from the .NET API's OpenAPI document.

Compare two approaches in 5 bullets each:
A) NSwag MSBuild (generates at build time, no extra dev tool)
B) openapi-typescript-codegen (npm script, runs against /swagger/v1/swagger.json)

Then recommend ONE based on this repo's constraints (no TS today, Vite, minimal deps).
Do NOT write code yet.
```

Pick one with the room. Then:

```
Implement option B. Add an npm script `gen:api` that runs against http://localhost:5080/swagger/v1/swagger.json
and writes to frontend/src/generated/. Update App.jsx to use the generated client.
Keep api.js as a thin re-export so the rest of the app doesn't change.
```

---

## 8. Generate Postman / Bruno collections from Swagger

```
@workspace
Generate a Bruno collection (folder of .bru files) for every endpoint in OrdersController and ProductsController.
Use the request shapes from the [ProducesResponseType] DTOs.
Put the collection under /tools/bruno-collection/.
Include a happy-path example body for POST /api/orders.
```

---

## 9. Spec-driven development — design before code

Flip the workflow. Hand Copilot an OpenAPI snippet, get the controller:

```
@workspace
Here is the OpenAPI fragment for a new endpoint:

POST /api/orders/{id}/cancel
  - 204 No Content on success
  - 404 if order not found
  - 409 if order already shipped

Generate:
- The controller action with [ProducesResponseType] + XML doc
- The service method signature (don't implement yet)
- The xUnit test scaffolding for all three response codes
- The Bruno request file

Follow the patterns already used in OrdersController.
```

---

## 10. Audit prompt — "is my Swagger any good?"

Run this at the end as a self-check:

```
@workspace
Act as an API design reviewer. Open the generated Swagger at /swagger/v1/swagger.json
(infer from the controllers and attributes — don't actually fetch it).

Score each controller on:
- Endpoint summary clarity (0-3)
- Response code completeness (0-3)
- Error contract consistency (0-3)
- Example quality (0-3)
- Naming consistency (0-3)

For each score below 3, give ONE concrete fix.
Output a markdown table, sorted by total score ascending. No prose.
```

---

## Quick checklist (put this on the slide)

- [ ] `GenerateDocumentationFile` on; XML included in Swashbuckle
- [ ] `///` comments on every public action
- [ ] `[ProducesResponseType]` for every realistic status code
- [ ] At least one request example per POST/PUT
- [ ] ProblemDetails for all 4xx/5xx
- [ ] Endpoints tagged & sorted in UI
- [ ] Spec is the source of truth for the client (generated, not hand-rolled)
