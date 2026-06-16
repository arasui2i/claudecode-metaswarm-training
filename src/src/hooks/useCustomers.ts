import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  getCustomers,
  getCustomerById,
  createCustomer,
  updateCustomer,
  deleteCustomer,
  type CustomersQuery,
  type CreateCustomerRequest,
  type UpdateCustomerRequest,
} from '../api/customers';

const CUSTOMERS_KEY = 'customers';

export const useCustomers = (query: CustomersQuery) =>
  useQuery({
    queryKey: [CUSTOMERS_KEY, query],
    queryFn: () => getCustomers(query),
  });

export const useCustomerById = (id: string) =>
  useQuery({
    queryKey: [CUSTOMERS_KEY, id],
    queryFn: () => getCustomerById(id),
    enabled: !!id,
  });

export const useCreateCustomer = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (body: CreateCustomerRequest) => createCustomer(body),
    onSuccess: () => qc.invalidateQueries({ queryKey: [CUSTOMERS_KEY] }),
  });
};

export const useUpdateCustomer = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (body: UpdateCustomerRequest) => updateCustomer(body),
    onSuccess: () => qc.invalidateQueries({ queryKey: [CUSTOMERS_KEY] }),
  });
};

export const useDeleteCustomer = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => deleteCustomer(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: [CUSTOMERS_KEY] }),
  });
};
