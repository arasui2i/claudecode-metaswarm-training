import axiosInstance from './axiosInstance';

export interface ContactSummary {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  jobTitle: string;
  contactType: string;
  customerId: string | null;
  createdAt: string;
}

export interface ContactDetail extends ContactSummary {
  department: string;
  notes: string;
  updatedAt: string;
}

export interface PagedResult<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}

export interface ContactsQuery {
  search?: string;
  page?: number;
  pageSize?: number;
}

export interface CreateContactRequest {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  jobTitle?: string;
  department?: string;
  contactType?: string;
  customerId?: string;
  notes?: string;
}

export type UpdateContactRequest = CreateContactRequest & { id: string };

export const getContacts = async (query: ContactsQuery): Promise<PagedResult<ContactSummary>> => {
  const { data } = await axiosInstance.get('/contacts', { params: query });
  return data;
};

export const getContactById = async (id: string): Promise<ContactDetail> => {
  const { data } = await axiosInstance.get(`/contacts/${id}`);
  return data;
};

export const createContact = async (body: CreateContactRequest): Promise<string> => {
  const { data } = await axiosInstance.post('/contacts', body);
  return data.id;
};

export const updateContact = async ({ id, ...body }: UpdateContactRequest): Promise<void> => {
  await axiosInstance.put(`/contacts/${id}`, { id, ...body });
};

export const deleteContact = async (id: string): Promise<void> => {
  await axiosInstance.delete(`/contacts/${id}`);
};
