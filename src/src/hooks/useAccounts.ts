import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  getAccounts, createAccount, updateAccount, deleteAccount,
  type AccountsQuery, type CreateAccountRequest, type UpdateAccountRequest,
} from '../api/accounts';

const KEY = 'accounts';

export const useAccounts = (query: AccountsQuery) =>
  useQuery({ queryKey: [KEY, query], queryFn: () => getAccounts(query) });

export const useCreateAccount = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (body: CreateAccountRequest) => createAccount(body),
    onSuccess: () => qc.invalidateQueries({ queryKey: [KEY] }),
  });
};

export const useUpdateAccount = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (body: UpdateAccountRequest) => updateAccount(body),
    onSuccess: () => qc.invalidateQueries({ queryKey: [KEY] }),
  });
};

export const useDeleteAccount = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => deleteAccount(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: [KEY] }),
  });
};
