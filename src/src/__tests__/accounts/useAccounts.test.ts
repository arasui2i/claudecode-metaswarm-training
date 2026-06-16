import { renderHook, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { createElement } from 'react';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import { useAccounts, useCreateAccount, useDeleteAccount } from '../../hooks/useAccounts';
import * as api from '../../api/accounts';

vi.mock('../../api/accounts');

function wrapper({ children }: { children: React.ReactNode }) {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return createElement(QueryClientProvider, { client: qc }, children);
}

describe('useAccounts', () => {
  beforeEach(() => vi.clearAllMocks());

  it('returns paged account data', async () => {
    vi.mocked(api.getAccounts).mockResolvedValue({
      items: [{ id: '1', name: 'Acme Corp', accountType: 'Customer', industry: 'Tech', website: '', phoneNumber: '', employeeCount: 100, createdAt: '' }],
      total: 1, page: 1, pageSize: 10,
    });
    const { result } = renderHook(() => useAccounts({}), { wrapper });
    await waitFor(() => expect(result.current.isSuccess).toBe(true));
    expect(result.current.data?.items[0].name).toBe('Acme Corp');
  });
});

describe('useCreateAccount', () => {
  it('calls createAccount API', async () => {
    vi.mocked(api.createAccount).mockResolvedValue('new-id');
    const { result } = renderHook(() => useCreateAccount(), { wrapper });
    result.current.mutate({ name: 'New Corp' });
    await waitFor(() => expect(result.current.isSuccess).toBe(true));
    expect(api.createAccount).toHaveBeenCalledOnce();
  });
});

describe('useDeleteAccount', () => {
  it('calls deleteAccount API', async () => {
    vi.mocked(api.deleteAccount).mockResolvedValue(undefined);
    const { result } = renderHook(() => useDeleteAccount(), { wrapper });
    result.current.mutate('some-id');
    await waitFor(() => expect(result.current.isSuccess).toBe(true));
    expect(api.deleteAccount).toHaveBeenCalledWith('some-id');
  });
});
