import axiosInstance from './axiosInstance';

export interface CustomerSummary {
  id: string;
  firstName: string;
  lastName: string;
  company: string;
  email: string;
  status: string;
  jobTitle: string;
  createdAt: string;
}

export interface CustomerDetail extends CustomerSummary {
  phoneNumber: string;
  industry: string;
  annualIncome: number;
  employeeCount: number;
  headquartersAddress: string;
  gender: string;
  age: number;
  updatedAt: string;
}

export interface PagedResult<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}

export interface CustomersQuery {
  search?: string;
  page?: number;
  pageSize?: number;
}

export interface CreateCustomerRequest {
  firstName: string;
  lastName: string;
  email: string;
  company?: string;
  phoneNumber?: string;
  status?: string;
  jobTitle?: string;
  gender?: string;
  age?: number;
  industry?: string;
  annualIncome?: number;
  employeeCount?: number;
  headquartersAddress?: string;
}

export type UpdateCustomerRequest = CreateCustomerRequest & { id: string };

export const getCustomers = async (query: CustomersQuery): Promise<PagedResult<CustomerSummary>> => {
  const { data } = await axiosInstance.get('/customers', { params: query });
  return data;
};

export const getCustomerById = async (id: string): Promise<CustomerDetail> => {
  const { data } = await axiosInstance.get(`/customers/${id}`);
  return data;
};

export const createCustomer = async (body: CreateCustomerRequest): Promise<string> => {
  const { data } = await axiosInstance.post('/customers', body);
  return data.id;
};

export const updateCustomer = async ({ id, ...body }: UpdateCustomerRequest): Promise<void> => {
  await axiosInstance.put(`/customers/${id}`, { id, ...body });
};

export const deleteCustomer = async (id: string): Promise<void> => {
  await axiosInstance.delete(`/customers/${id}`);
};
