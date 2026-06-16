import { renderHook, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { createElement } from 'react';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import { useLeads, useCreateLead, useDeleteLead } from '../../hooks/useLeads';
import * as leadsApi from '../../api/leads';

vi.mock('../../api/leads');

function wrapper({ children }: { children: React.ReactNode }) {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return createElement(QueryClientProvider, { client: qc }, children);
}

describe('useLeads', () => {
  beforeEach(() => vi.clearAllMocks());

  it('returns paged lead data', async () => {
    vi.mocked(leadsApi.getLeads).mockResolvedValue({
      items: [{ id: '1', firstName: 'Alice', lastName: 'Smith', email: 'a@b.com', company: '', status: 'New', source: 'Website', createdAt: '' }],
      total: 1,
      page: 1,
      pageSize: 10,
    });

    const { result } = renderHook(() => useLeads({}), { wrapper });
    await waitFor(() => expect(result.current.isSuccess).toBe(true));

    expect(result.current.data?.total).toBe(1);
    expect(result.current.data?.items[0].firstName).toBe('Alice');
  });
});

describe('useCreateLead', () => {
  it('calls createLead API', async () => {
    vi.mocked(leadsApi.createLead).mockResolvedValue('new-id');

    const { result } = renderHook(() => useCreateLead(), { wrapper });
    result.current.mutate({ firstName: 'Bob', lastName: 'Jones', email: 'b@c.com' } as never);

    await waitFor(() => expect(result.current.isSuccess).toBe(true));
    expect(leadsApi.createLead).toHaveBeenCalledOnce();
  });
});

describe('useDeleteLead', () => {
  it('calls deleteLead API', async () => {
    vi.mocked(leadsApi.deleteLead).mockResolvedValue(undefined);

    const { result } = renderHook(() => useDeleteLead(), { wrapper });
    result.current.mutate('some-id');

    await waitFor(() => expect(result.current.isSuccess).toBe(true));
    expect(leadsApi.deleteLead).toHaveBeenCalledWith('some-id');
  });
});
