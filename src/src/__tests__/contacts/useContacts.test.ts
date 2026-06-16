import { renderHook, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { createElement } from 'react';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import { useContacts, useCreateContact, useDeleteContact } from '../../hooks/useContacts';
import * as contactsApi from '../../api/contacts';

vi.mock('../../api/contacts');

function wrapper({ children }: { children: React.ReactNode }) {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return createElement(QueryClientProvider, { client: qc }, children);
}

describe('useContacts', () => {
  beforeEach(() => vi.clearAllMocks());

  it('returns paged contact data', async () => {
    vi.mocked(contactsApi.getContacts).mockResolvedValue({
      items: [{ id: '1', firstName: 'Alice', lastName: 'Smith', email: 'a@b.com', phoneNumber: '', jobTitle: '', contactType: 'Primary', customerId: null, createdAt: '' }],
      total: 1, page: 1, pageSize: 10,
    });

    const { result } = renderHook(() => useContacts({}), { wrapper });
    await waitFor(() => expect(result.current.isSuccess).toBe(true));

    expect(result.current.data?.total).toBe(1);
    expect(result.current.data?.items[0].firstName).toBe('Alice');
  });
});

describe('useCreateContact', () => {
  it('calls createContact API', async () => {
    vi.mocked(contactsApi.createContact).mockResolvedValue('new-id');

    const { result } = renderHook(() => useCreateContact(), { wrapper });
    result.current.mutate({ firstName: 'Bob', lastName: 'Jones', email: 'b@c.com' } as never);

    await waitFor(() => expect(result.current.isSuccess).toBe(true));
    expect(contactsApi.createContact).toHaveBeenCalledOnce();
  });
});

describe('useDeleteContact', () => {
  it('calls deleteContact API', async () => {
    vi.mocked(contactsApi.deleteContact).mockResolvedValue(undefined);

    const { result } = renderHook(() => useDeleteContact(), { wrapper });
    result.current.mutate('some-id');

    await waitFor(() => expect(result.current.isSuccess).toBe(true));
    expect(contactsApi.deleteContact).toHaveBeenCalledWith('some-id');
  });
});
