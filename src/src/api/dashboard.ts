import axios from './axiosInstance';

export interface TicketsByStatus {
  new: number;
  inProgress: number;
  pending: number;
  resolved: number;
  closed: number;
}

export interface DashboardSummary {
  currentMonthLeads: number;
  convertedCustomersThisMonth: number;
  ticketsByStatus: TicketsByStatus;
}

export const getDashboardSummary = (): Promise<DashboardSummary> =>
  axios.get('/api/dashboard/summary').then(r => r.data);
