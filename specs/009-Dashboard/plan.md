# Dashboard — Technical Implementation Plan

## Overview

Single GET endpoint returns all KPI data in one call. The frontend renders two stat cards and a doughnut chart. No real-time updates; data loads on mount with a manual refresh option.

---

## Schema Gaps to Resolve Before Implementation

### Gap 1 — Lead: no ConvertedAt timestamp

The spec requires "Converted Customers This Month". The `Lead` entity only has `CreatedAt` and `UpdatedAt` (from `BaseEntity`). Using `UpdatedAt` as a proxy is unreliable — any edit to a converted lead after month-end would shift the count.

**Resolution:** Add `ConvertedAt` (`DateTime?`, nullable) to `Lead`. Set it in `UpdateLeadHandler` when `Status` transitions to `Converted` for the first time (i.e. `ConvertedAt` is still null).

### Gap 2 — TicketStatus: missing Pending value

The spec lists five statuses: **New, In Progress, Pending, Resolved, Closed**.  
Our `TicketStatus` enum has four: `Open, InProgress, Resolved, Closed`.

Two sub-gaps:
| Spec status | Code status | Action |
|-------------|-------------|--------|
| New | Open | Map `Open → "new"` in the DTO (rename only, no schema change) |
| Pending | *(missing)* | Add `Pending = 4` to enum; migration has no data impact (integer enum, nullable column) |

---

## Architecture Decisions

| Decision | Choice | Reason |
|----------|--------|--------|
| Data access in handler | `IDashboardRepository` (new) | Consistent with repository pattern used across all modules; cross-entity queries stay in Infrastructure |
| Chart library | **Recharts** (`npm install recharts`) | Lightweight, React-native, composable; no additional CSS setup; integrates cleanly with MUI layouts |
| Query filters | Rely on existing `HasQueryFilter(!IsDeleted)` | Soft-deleted leads and tickets are automatically excluded from all queries |
| Month boundary | `DateTime.UtcNow` in repository | Simple, consistent with other UTC timestamps in `BaseEntity` |

---

## Backend Tasks

### T-01 — Add `ConvertedAt` to `Lead` entity

**File:** `src/CRM.Domain/Entities/Lead.cs`

Add:
```csharp
public DateTime? ConvertedAt { get; set; }
```

**File:** `src/CRM.Application/Features/Leads/UpdateLead/UpdateLeadHandler.cs`

When updating, if `request.Status == LeadStatus.Converted && lead.ConvertedAt == null`, set `lead.ConvertedAt = DateTime.UtcNow`.

**Migration:** `AddLeadConvertedAt` — adds a nullable `datetime2` column, zero downtime.

---

### T-02 — Add `Pending` to `TicketStatus` enum

**File:** `src/CRM.Domain/Enums/TicketStatus.cs`

```csharp
public enum TicketStatus
{
    Open,
    InProgress,
    Resolved,
    Closed,
    Pending,   // value = 4, appended to avoid shifting existing values
}
```

Update frontend `TicketStatus` enum and `STATUS_LABELS` maps in `TicketListPage` and `TicketFormPage`.

**Migration:** `AddTicketStatusPending` — no schema change (int column, existing rows unaffected).

---

### T-03 — DashboardSummaryDto

**File:** `src/CRM.Application/Features/Dashboard/DashboardSummaryDto.cs`

```csharp
public record TicketsByStatusDto(
    int New,
    int InProgress,
    int Pending,
    int Resolved,
    int Closed);

public record DashboardSummaryDto(
    int CurrentMonthLeads,
    int ConvertedCustomersThisMonth,
    TicketsByStatusDto TicketsByStatus);
```

`New` maps from `TicketStatus.Open` in the repository query.

---

### T-04 — IDashboardRepository

**File:** `src/CRM.Application/Common/Interfaces/IDashboardRepository.cs`

```csharp
public interface IDashboardRepository
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken ct = default);
}
```

---

### T-05 — GetDashboardSummaryQuery + Handler

**Folder:** `src/CRM.Application/Features/Dashboard/GetDashboardSummary/`

- `GetDashboardSummaryQuery` — parameterless `IRequest<DashboardSummaryDto>`
- `GetDashboardSummaryHandler` — injects `IDashboardRepository`, delegates entirely to `GetSummaryAsync`

No validator needed (no user input).

---

### T-06 — DashboardRepository

**File:** `src/CRM.Infrastructure/Repositories/DashboardRepository.cs`

Three queries executed inside `GetSummaryAsync`:

```
monthStart  = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc)
monthEnd    = monthStart.AddMonths(1)

currentMonthLeads
  = Leads.Count(l => l.CreatedAt >= monthStart && l.CreatedAt < monthEnd)
  // soft-delete filter applied automatically via HasQueryFilter

convertedCustomersThisMonth
  = Leads.Count(l => l.Status == Converted
                  && l.ConvertedAt >= monthStart
                  && l.ConvertedAt < monthEnd)

ticketsByStatus
  = Tickets.GroupBy(t => t.Status)
           .Select(g => new { Status = g.Key, Count = g.Count() })
           .ToDictionary(...)
  // missing statuses default to 0
```

---

### T-07 — DashboardController + Program.cs + DI

**File:** `src/CRM.API/Controllers/DashboardController.cs`

- `GET /api/dashboard/summary` — `[Authorize(Policy = "dashboard.view")]`

**File:** `src/CRM.API/Program.cs`

Add policy:
```csharp
options.AddPolicy("dashboard.view", p => p.AddRequirements(new PermissionRequirement("dashboard.view")));
```

**File:** `src/CRM.Infrastructure/DependencyInjection.cs`

```csharp
services.AddScoped<IDashboardRepository, DashboardRepository>();
```

---

## Frontend Tasks

### T-08 — Install Recharts

```
npm install recharts
```

No additional configuration needed.

---

### T-09 — Dashboard API client

**File:** `web/src/api/dashboard.ts`

Types:
```typescript
interface TicketsByStatus { new: number; inProgress: number; pending: number; resolved: number; closed: number; }
interface DashboardSummary { currentMonthLeads: number; convertedCustomersThisMonth: number; ticketsByStatus: TicketsByStatus; }
```

Function: `getDashboardSummary(): Promise<DashboardSummary>` — `GET /api/dashboard/summary`

---

### T-10 — useDashboard hook

**File:** `web/src/hooks/useDashboard.ts`

```typescript
export function useDashboardSummary() {
  return useQuery({
    queryKey: ['dashboard', 'summary'],
    queryFn: getDashboardSummary,
    staleTime: 5 * 60 * 1000,   // 5 minutes
  });
}
```

Expose `refetch` from the hook for the manual refresh button.

---

### T-11 — DashboardPage

**File:** `web/src/pages/Dashboard/DashboardPage.tsx`

Layout (MUI Grid):

```
┌─────────────────────────────────────────────────────┐
│  Dashboard                         [↻ Refresh]      │
├────────────────────────┬────────────────────────────┤
│  Leads This Month      │  Customers Converted        │
│  ┌──────────────────┐  │  ┌──────────────────────┐  │
│  │       125        │  │  │         18           │  │
│  └──────────────────┘  │  └──────────────────────┘  │
├────────────────────────┴────────────────────────────┤
│  Ticket Status Summary                               │
│  ┌──────────────────────────────────────────────┐   │
│  │         [Recharts PieChart — doughnut]        │   │
│  │   ● New  ● In Progress  ● Pending             │   │
│  │   ● Resolved  ● Closed                       │   │
│  └──────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────┘
```

- KPI cards: MUI `Card` + `Typography`
- Chart: Recharts `PieChart` with `innerRadius` (doughnut), `Tooltip`, `Legend`
- Skeleton: MUI `Skeleton` shown for all sections while `isLoading`
- Refresh: `Button` calls `refetch()` from `useDashboardSummary`

---

### T-12 — Wire up routing + sidebar

**File:** `web/src/App.tsx`
- Add route: `<Route path="/dashboard" element={<DashboardPage />} />`
- Change default redirect: `/` → `/dashboard`

**File:** `web/src/components/Layout/Sidebar.tsx`
- Dashboard nav item already exists with path `/`; update path to `/dashboard`

---

## Tests

### T-13 — GetDashboardSummaryHandlerTests

**File:** `tests/CRM.Tests/Features/Dashboard/GetDashboardSummaryHandlerTests.cs`

- Mock `IDashboardRepository.GetSummaryAsync` returning a known `DashboardSummaryDto`
- Assert handler returns the same DTO (pure delegation test)

### T-14 — DashboardPage.test.tsx

**File:** `web/src/pages/Dashboard/DashboardPage.test.tsx`

- KPI card shows correct lead count
- KPI card shows correct converted count
- Skeleton rendered while loading
- Refresh button calls `refetch`

---

## Task Summary

| Phase | Task | Description |
|-------|------|-------------|
| 1 — Schema fixes | T-01 | Add `ConvertedAt` to `Lead` + migration |
| 1 — Schema fixes | T-02 | Add `Pending` to `TicketStatus` + migration |
| 2 — Application | T-03 | `DashboardSummaryDto` |
| 2 — Application | T-04 | `IDashboardRepository` |
| 2 — Application | T-05 | `GetDashboardSummaryQuery` + Handler |
| 3 — Infrastructure | T-06 | `DashboardRepository` (3 aggregate queries) |
| 3 — Infrastructure | T-07 | Controller + policy + DI registration |
| 4 — Frontend | T-08 | `npm install recharts` |
| 4 — Frontend | T-09 | `dashboard.ts` API client |
| 4 — Frontend | T-10 | `useDashboard.ts` hook |
| 4 — Frontend | T-11 | `DashboardPage.tsx` |
| 4 — Frontend | T-12 | Routing + sidebar update |
| 5 — Tests | T-13 | `GetDashboardSummaryHandlerTests` |
| 5 — Tests | T-14 | `DashboardPage.test.tsx` |
| **Total** | | **14 tasks** |
