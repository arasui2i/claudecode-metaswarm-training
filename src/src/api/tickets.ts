import axios from './axiosInstance';

export type TicketPriority = 'Low' | 'Medium' | 'High' | 'Critical';
export type TicketStatus = 'Open' | 'InProgress' | 'Resolved' | 'Closed' | 'Pending';

export interface TicketSummary {
  id: string;
  ticketNumber: string;
  subject: string;
  priority: TicketPriority;
  status: TicketStatus;
}

export interface TicketDetail extends TicketSummary {
  accountId: string | null;
  accountName: string | null;
  contactId: string | null;
  contactName: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface PagedResult<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}

export interface TicketsQuery {
  search?: string;
  page?: number;
  pageSize?: number;
}

export interface CreateTicketRequest {
  subject: string;
  accountId?: string | null;
  contactId?: string | null;
  priority: TicketPriority;
  status?: TicketStatus;
}

export interface UpdateTicketRequest extends CreateTicketRequest {
  id: string;
}

export const getTickets = (query: TicketsQuery = {}): Promise<PagedResult<TicketSummary>> =>
  axios.get('/api/tickets', { params: query }).then(r => r.data);

export const getTicket = (id: string): Promise<TicketDetail> =>
  axios.get(`/api/tickets/${id}`).then(r => r.data);

export const createTicket = (data: CreateTicketRequest): Promise<{ id: string }> =>
  axios.post('/api/tickets', data).then(r => r.data);

export const updateTicket = (data: UpdateTicketRequest): Promise<void> =>
  axios.put(`/api/tickets/${data.id}`, data).then(r => r.data);

export const deleteTicket = (id: string): Promise<void> =>
  axios.delete(`/api/tickets/${id}`).then(r => r.data);
