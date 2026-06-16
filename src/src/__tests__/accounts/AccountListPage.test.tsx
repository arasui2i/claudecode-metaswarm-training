import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import AccountListPage from '../../pages/Accounts/AccountListPage';
import * as api from '../../api/accounts';

vi.mock('../../api/accounts');

const pagedResult = {
  items: [
    { id: 'a-1', name: 'Acme Corp', accountType: 'Customer', industry: 'Tech', website: 'acme.com', phoneNumber: '123', employeeCount: 200, createdAt: '2026-01-01' },
    { id: 'a-2', name: 'Beta Inc',  accountType: 'Prospect', industry: 'Finance', website: '', phoneNumber: '', employeeCount: 50, createdAt: '2026-01-02' },
  ],
  total: 2, page: 1, pageSize: 10,
};

function Wrapper({ children }: { children: React.ReactNode }) {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return <QueryClientProvider client={qc}>{children}</QueryClientProvider>;
}

describe('AccountListPage', () => {
  beforeEach(() => { vi.clearAllMocks(); vi.mocked(api.getAccounts).mockResolvedValue(pagedResult); });

  it('renders page heading', () => {
    render(<AccountListPage />, { wrapper: Wrapper });
    expect(screen.getByText('Accounts')).toBeInTheDocument();
  });

  it('shows account rows after load', async () => {
    render(<AccountListPage />, { wrapper: Wrapper });
    await waitFor(() => expect(screen.getByText('Acme Corp')).toBeInTheDocument());
    expect(screen.getByText('Beta Inc')).toBeInTheDocument();
  });

  it('renders Add Account button', () => {
    render(<AccountListPage />, { wrapper: Wrapper });
    expect(screen.getByRole('button', { name: /add account/i })).toBeInTheDocument();
  });

  it('opens create dialog on Add Account click', async () => {
    render(<AccountListPage />, { wrapper: Wrapper });
    fireEvent.click(screen.getByRole('button', { name: /add account/i }));
    await waitFor(() => expect(screen.getByRole('dialog')).toBeInTheDocument());
  });

  it('calls deleteAccount after confirm', async () => {
    vi.mocked(api.deleteAccount).mockResolvedValue(undefined);
    render(<AccountListPage />, { wrapper: Wrapper });
    await waitFor(() => expect(screen.getByText('Acme Corp')).toBeInTheDocument());
    fireEvent.click(screen.getAllByRole('button', { name: /delete/i })[0]);
    fireEvent.click(await screen.findByRole('button', { name: /confirm/i }));
    await waitFor(() => expect(api.deleteAccount).toHaveBeenCalledWith('a-1'));
  });
});
