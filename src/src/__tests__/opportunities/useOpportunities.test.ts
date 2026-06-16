import { renderHook, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { createElement } from 'react';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import { useOpportunities, useCreateOpportunity, useDeleteOpportunity } from '../../hooks/useOpportunities';
import * as api from '../../api/opportunities';

vi.mock('../../api/opportunities');

function wrapper({ children }: { children: React.ReactNode }) {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return createElement(QueryClientProvider, { client: qc }, children);
}

describe('useOpportunities', () => {
  beforeEach(() => vi.clearAllMocks());

  it('returns paged opportunity data', async () => {
    vi.mocked(api.getOpportunities).mockResolvedValue({
      items: [{ id: '1', name: 'Big Deal', stage: 'Proposal', amount: 50000, probability: 60, closeDate: null, accountId: null, createdAt: '' }],
      total: 1, page: 1, pageSize: 10,
    });
    const { result } = renderHook(() => useOpportunities({}), { wrapper });
    await waitFor(() => expect(result.current.isSuccess).toBe(true));
    expect(result.current.data?.items[0].name).toBe('Big Deal');
    expect(result.current.data?.items[0].amount).toBe(50000);
  });
});

describe('useCreateOpportunity', () => {
  it('calls createOpportunity API', async () => {
    vi.mocked(api.createOpportunity).mockResolvedValue('new-id');
    const { result } = renderHook(() => useCreateOpportunity(), { wrapper });
    result.current.mutate({ name: 'New Deal', amount: 10000 });
    await waitFor(() => expect(result.current.isSuccess).toBe(true));
    expect(api.createOpportunity).toHaveBeenCalledOnce();
  });
});

describe('useDeleteOpportunity', () => {
  it('calls deleteOpportunity API', async () => {
    vi.mocked(api.deleteOpportunity).mockResolvedValue(undefined);
    const { result } = renderHook(() => useDeleteOpportunity(), { wrapper });
    result.current.mutate('some-id');
    await waitFor(() => expect(result.current.isSuccess).toBe(true));
    expect(api.deleteOpportunity).toHaveBeenCalledWith('some-id');
  });
});
