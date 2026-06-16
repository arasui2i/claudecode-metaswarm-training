import { renderHook, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { createElement } from 'react';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import { useCustomers, useCreateCustomer, useDeleteCustomer } from '../../hooks/useCustomers';
import * as customersApi from '../../api/customers';

vi.mock('../../api/customers');

function wrapper({ children }: { children: React.ReactNode }) {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return createElement(QueryClientProvider, { client: qc }, children);
}

describe('useCustomers', () => {
  beforeEach(() => vi.clearAllMocks());

  it('returns paged customer data', async () => {
    vi.mocked(customersApi.getCustomers).mockResolvedValue({
      items: [{ id: '1', firstName: 'Alice', lastName: 'Smith', email: 'a@b.com', company: '', status: 'Active', jobTitle: '', createdAt: '' }],
      total: 1,
      page: 1,
      pageSize: 10,
    });

    const { result } = renderHook(() => useCustomers({}), { wrapper });
    await waitFor(() => expect(result.current.isSuccess).toBe(true));

    expect(result.current.data?.total).toBe(1);
    expect(result.current.data?.items[0].firstName).toBe('Alice');
  });
});

describe('useCreateCustomer', () => {
  it('calls createCustomer API', async () => {
    vi.mocked(customersApi.createCustomer).mockResolvedValue('new-id');

    const { result } = renderHook(() => useCreateCustomer(), { wrapper });
    result.current.mutate({ firstName: 'Bob', lastName: 'Jones', email: 'b@c.com' } as never);

    await waitFor(() => expect(result.current.isSuccess).toBe(true));
    expect(customersApi.createCustomer).toHaveBeenCalledOnce();
  });
});

describe('useDeleteCustomer', () => {
  it('calls deleteCustomer API', async () => {
    vi.mocked(customersApi.deleteCustomer).mockResolvedValue(undefined);

    const { result } = renderHook(() => useDeleteCustomer(), { wrapper });
    result.current.mutate('some-id');

    await waitFor(() => expect(result.current.isSuccess).toBe(true));
    expect(customersApi.deleteCustomer).toHaveBeenCalledWith('some-id');
  });
});
