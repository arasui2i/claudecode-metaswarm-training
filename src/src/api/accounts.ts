import axiosInstance from './axiosInstance';

export interface AccountSummary {
  id: string;
  name: string;
  accountType: string;
  industry: string;
  website: string;
  phoneNumber: string;
  employeeCount: number;
  createdAt: string;
}

export interface AccountDetail extends AccountSummary {
  annualRevenue: number;
  billingAddress: string;
  description: string;
  updatedAt: string;
}

export interface PagedResult<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}

export interface AccountsQuery { search?: string; page?: number; pageSize?: number; }

export interface CreateAccountRequest {
  name: string;
  accountType?: string;
  industry?: string;
  website?: string;
  phoneNumber?: string;
  annualRevenue?: number;
  employeeCount?: number;
  billingAddress?: string;
  description?: string;
}

export type UpdateAccountRequest = CreateAccountRequest & { id: string };

export const getAccounts = async (q: AccountsQuery): Promise<PagedResult<AccountSummary>> =>
  (await axiosInstance.get('/accounts', { params: q })).data;

export const getAccountById = async (id: string): Promise<AccountDetail> =>
  (await axiosInstance.get(`/accounts/${id}`)).data;

export const createAccount = async (body: CreateAccountRequest): Promise<string> =>
  (await axiosInstance.post('/accounts', body)).data.id;

export const updateAccount = async ({ id, ...body }: UpdateAccountRequest): Promise<void> =>
  axiosInstance.put(`/accounts/${id}`, { id, ...body });

export const deleteAccount = async (id: string): Promise<void> =>
  axiosInstance.delete(`/accounts/${id}`);
