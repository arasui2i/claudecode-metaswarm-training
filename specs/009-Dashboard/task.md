# Dashboard — Task List

## Phase 1: Schema Fixes

### T-01: Add ConvertedAt to Lead entity
- [ ] Add `public DateTime? ConvertedAt { get; set; }` to `src/CRM.Domain/Entities/Lead.cs`
- [ ] Update `src/CRM.Application/Features/Leads/UpdateLead/UpdateLeadHandler.cs`: when `request.Status == LeadStatus.Converted && lead.ConvertedAt == null`, set `lead.ConvertedAt = DateTime.UtcNow`
- [ ] Add EF Core migration: `dotnet ef migrations add AddLeadConvertedAt`
- [ ] Apply migration: `dotnet ef database update`

### T-02: Add Pending to TicketStatus enum
- [ ] Add `Pending = 4` to `src/CRM.Domain/Enums/TicketStatus.cs` (appended after Closed to avoid shifting existing values)
- [ ] Add EF Core migration: `dotnet ef migrations add AddTicketStatusPending` (no schema change, just documents intent)
- [ ] Update `web/src/api/tickets.ts`: add `Pending = 4` to `TicketStatus` enum
- [ ] Update `web/src/pages/Tickets/TicketListPage.tsx`: add `Pending` entry to `STATUS_LABELS`
- [ ] Update `web/src/pages/Tickets/TicketFormPage.tsx`: add `Pending` option to Status select

---

## Phase 2: Application Layer

### T-03: DashboardSummaryDto
- [ ] Create file `src/CRM.Application/Features/Dashboard/DashboardSummaryDto.cs`
- [ ] Define `record TicketsByStatusDto(int New, int InProgress, int Pending, int Resolved, int Closed)`
- [ ] Define `record DashboardSummaryDto(int CurrentMonthLeads, int ConvertedCustomersThisMonth, TicketsByStatusDto TicketsByStatus)`

### T-04: IDashboardRepository
- [ ] Create file `src/CRM.Application/Common/Interfaces/IDashboardRepository.cs`
- [ ] Define interface with single method: `Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken ct = default)`

### T-05: GetDashboardSummaryQuery and Handler
- [ ] Create folder `src/CRM.Application/Features/Dashboard/GetDashboardSummary/`
- [ ] Create `GetDashboardSummaryQuery.cs`: parameterless record implementing `IRequest<DashboardSummaryDto>`
- [ ] Create `GetDashboardSummaryHandler.cs`: inject `IDashboardRepository`, return `await _repo.GetSummaryAsync(ct)`
- [ ] No validator needed (no user input)

---

## Phase 3: Infrastructure Layer

### T-06: DashboardRepository
- [ ] Create `src/CRM.Infrastructure/Repositories/DashboardRepository.cs`
- [ ] Inject `AppDbContext` via constructor
- [ ] Implement `GetSummaryAsync`:
  - Compute `monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc)` and `monthEnd = monthStart.AddMonths(1)`
  - Query `currentMonthLeads`: count Leads where `CreatedAt >= monthStart && CreatedAt < monthEnd` (soft-delete filter auto-applied)
  - Query `convertedCustomersThisMonth`: count Leads where `Status == Converted && ConvertedAt >= monthStart && ConvertedAt < monthEnd`
  - Query `ticketsByStatus`: group Tickets by Status, count each group; default missing statuses to 0; map `Open → New` in the DTO
  - Return `DashboardSummaryDto`

### T-07: DashboardController, Policy, and DI Registration
- [ ] Create `src/CRM.API/Controllers/DashboardController.cs`
  - Single endpoint: `GET /api/dashboard/summary`
  - Decorate with `[Authorize(Policy = "dashboard.view")]`
  - Send `GetDashboardSummaryQuery` via MediatR, return `Ok(result)`
- [ ] Add policy in `src/CRM.API/Program.cs`:
  ```csharp
  options.AddPolicy("dashboard.view", p => p.AddRequirements(new PermissionRequirement("dashboard.view")));
  ```
- [ ] Register in `src/CRM.Infrastructure/DependencyInjection.cs`:
  ```csharp
  services.AddScoped<IDashboardRepository, DashboardRepository>();
  ```

---

## Phase 4: Frontend

### T-08: Install Recharts
- [ ] Run `npm install recharts` inside the `web/` directory
- [ ] Verify `recharts` appears in `web/package.json` dependencies

### T-09: Dashboard API client
- [ ] Create `web/src/api/dashboard.ts`
- [ ] Define types: `TicketsByStatus` and `DashboardSummary`
- [ ] Export function `getDashboardSummary(): Promise<DashboardSummary>` calling `GET /api/dashboard/summary` via axios

### T-10: useDashboard hook
- [ ] Create `web/src/hooks/useDashboard.ts`
- [ ] Export `useDashboardSummary()` using React Query `useQuery`
  - `queryKey: ['dashboard', 'summary']`
  - `queryFn: getDashboardSummary`
  - `staleTime: 5 * 60 * 1000`

### T-11: DashboardPage component
- [ ] Create `web/src/pages/Dashboard/DashboardPage.tsx`
- [ ] Call `useDashboardSummary()`, destructure `{ data, isLoading, refetch }`
- [ ] Page header: title "Dashboard" + Refresh `Button` that calls `refetch()`
- [ ] KPI cards (MUI `Card` + `Typography`):
  - "Leads This Month" showing `data.currentMonthLeads`
  - "Customers Converted This Month" showing `data.convertedCustomersThisMonth`
- [ ] Ticket Status Summary section:
  - Recharts `PieChart` with `innerRadius` (doughnut style)
  - Five `Cell` entries: New, In Progress, Pending, Resolved, Closed
  - Include `Tooltip` and `Legend`
- [ ] Show MUI `Skeleton` for all sections while `isLoading`

### T-12: Wire up routing and sidebar
- [ ] In `web/src/App.tsx`:
  - Import `DashboardPage`
  - Add route: `<Route path="/dashboard" element={<DashboardPage />} />`
  - Update default redirect from `/` to `/dashboard`
- [ ] In `web/src/components/Layout/Sidebar.tsx`:
  - Update Dashboard nav item path from `/` to `/dashboard`

---

## Phase 5: Tests

### T-13: GetDashboardSummaryHandlerTests (C#)
- [ ] Create `tests/CRM.Tests/Features/Dashboard/GetDashboardSummaryHandlerTests.cs`
- [ ] Test: handler returns DTO from repository when query is sent
- [ ] Test: handler calls `GetSummaryAsync` exactly once
- [ ] Mock `IDashboardRepository` with NSubstitute

### T-14: DashboardPage.test.tsx (React)
- [ ] Create `web/src/pages/Dashboard/DashboardPage.test.tsx`
- [ ] Mock `useDashboardSummary` hook
- [ ] Test: KPI card displays correct lead count
- [ ] Test: KPI card displays correct converted count
- [ ] Test: Skeleton rendered while `isLoading` is true
- [ ] Test: Refresh button triggers `refetch`
