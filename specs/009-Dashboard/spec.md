# Dashboard

## Business Goal

Provide a high-level overview of CRM performance through key metrics and visual summaries to help users quickly understand business activity and customer engagement.

---

## Backend Requirements

### Dashboard Summary API

Create an API to return dashboard KPI metrics.

#### Endpoint

GET /api/dashboard/summary

#### Response

```json
{
  "currentMonthLeads": 125,
  "convertedCustomersThisMonth": 18,
  "ticketsByStatus": {
    "new": 12,
    "inProgress": 8,
    "pending": 5,
    "resolved": 20,
    "closed": 45
  }
}
```

### KPI Calculations

#### Current Month Leads

Count all leads created during the current month.

#### Converted Customers This Month

Count all leads converted to customers during the current month.

#### Tickets by Status

Group tickets by status and return the count for each status.

Supported statuses:

* New
* In Progress
* Pending
* Resolved
* Closed

### Validation

* Dashboard data should return only active records.
* User must be authenticated.

---

## Frontend Requirements

### Dashboard Page

Page Name:

Dashboard

### KPI Cards

Display the following KPI cards:

#### Leads This Month

Display total leads created in the current month.

#### Customers Converted This Month

Display total customers converted during the current month.

### Ticket Status Summary

Display ticket counts grouped by status.

Suggested visualization:

* Doughnut Chart
  or
* Bar Chart

Statuses:

* New
* In Progress
* Pending
* Resolved
* Closed

### Refresh Behavior

* Load dashboard data on page load.
* Allow manual refresh.

---

## Acceptance Criteria

### AC1

When the dashboard page loads, current month lead count is displayed.

### AC2

When the dashboard page loads, converted customer count is displayed.

### AC3

When the dashboard page loads, ticket counts are displayed by status.

### AC4

Dashboard data is retrieved through a single API call.

### AC5

Only authenticated users can access the dashboard.

---

## Out of Scope

* Forecasting
* Revenue Analytics
* Opportunity Pipeline Analytics
* Drill-down Reports
* Export Functionality
* Custom Dashboard Widgets
* Real-Time Updates
