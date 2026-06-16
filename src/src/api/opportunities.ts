import axiosInstance from './axiosInstance';

export interface OpportunitySummary {
  id: string;
  name: string;
  stage: string;
  amount: number;
  probability: number;
  closeDate: string | null;
  accountId: string | null;
  createdAt: string;
}

export interface OpportunityDetail extends OpportunitySummary {
  description: string;
  contactId: string | null;
  updatedAt: string;
}

export interface PagedResult<T> { items: T[]; total: number; page: number; pageSize: number; }
export interface OpportunitiesQuery { search?: string; page?: number; pageSize?: number; }

export interface CreateOpportunityRequest {
  name: string;
  stage?: string;
  amount?: number;
  probability?: number;
  closeDate?: string;
  description?: string;
  accountId?: string;
  contactId?: string;
}

export type UpdateOpportunityRequest = CreateOpportunityRequest & { id: string };

export const getOpportunities = async (q: OpportunitiesQuery): Promise<PagedResult<OpportunitySummary>> =>
  (await axiosInstance.get('/opportunities', { params: q })).data;

export const getOpportunityById = async (id: string): Promise<OpportunityDetail> =>
  (await axiosInstance.get(`/opportunities/${id}`)).data;

export const createOpportunity = async (body: CreateOpportunityRequest): Promise<string> =>
  (await axiosInstance.post('/opportunities', body)).data.id;

export const updateOpportunity = async ({ id, ...body }: UpdateOpportunityRequest): Promise<void> =>
  axiosInstance.put(`/opportunities/${id}`, { id, ...body });

export const deleteOpportunity = async (id: string): Promise<void> =>
  axiosInstance.delete(`/opportunities/${id}`);
