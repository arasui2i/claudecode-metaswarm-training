import axiosInstance from './axiosInstance';

export interface LeadSummary {
  id: string;
  firstName: string;
  lastName: string;
  company: string;
  email: string;
  status: string;
  source: string;
  createdAt: string;
}

export interface LeadDetail extends LeadSummary {
  phoneNumber: string;
  jobTitle: string;
  notes: string;
  convertedCustomerId: string | null;
  updatedAt: string;
}

export interface PagedResult<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}

export interface LeadsQuery {
  search?: string;
  page?: number;
  pageSize?: number;
}

export interface CreateLeadRequest {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  company?: string;
  jobTitle?: string;
  status?: string;
  source?: string;
  notes?: string;
}

export type UpdateLeadRequest = CreateLeadRequest & { id: string };

export const getLeads = async (query: LeadsQuery): Promise<PagedResult<LeadSummary>> => {
  const { data } = await axiosInstance.get('/leads', { params: query });
  return data;
};

export const getLeadById = async (id: string): Promise<LeadDetail> => {
  const { data } = await axiosInstance.get(`/leads/${id}`);
  return data;
};

export const createLead = async (body: CreateLeadRequest): Promise<string> => {
  const { data } = await axiosInstance.post('/leads', body);
  return data.id;
};

export const updateLead = async ({ id, ...body }: UpdateLeadRequest): Promise<void> => {
  await axiosInstance.put(`/leads/${id}`, { id, ...body });
};

export const deleteLead = async (id: string): Promise<void> => {
  await axiosInstance.delete(`/leads/${id}`);
};
