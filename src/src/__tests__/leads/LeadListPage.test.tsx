import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import LeadListPage from '../../pages/Leads/LeadListPage';
import * as leadsApi from '../../api/leads';

vi.mock('../../api/leads');

const pagedResult = {
  items: [
    { id: 'lead-1', firstName: 'Alice', lastName: 'Smith', email: 'alice@crm.com', company: 'Acme', status: 'New', source: 'Website', createdAt: '2026-01-01' },
    { id: 'lead-2', firstName: 'Bob', lastName: 'Jones', email: 'bob@crm.com', company: 'Corp', status: 'Contacted', source: 'Referral', createdAt: '2026-01-02' },
  ],
  total: 2,
  page: 1,
  pageSize: 10,
};

function Wrapper({ children }: { children: React.ReactNode }) {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return <QueryClientProvider client={qc}>{children}</QueryClientProvider>;
}

describe('LeadListPage', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(leadsApi.getLeads).mockResolvedValue(pagedResult);
  });

  it('renders page heading', () => {
    render(<LeadListPage />, { wrapper: Wrapper });
    expect(screen.getByText('Leads')).toBeInTheDocument();
  });

  it('shows lead rows after load', async () => {
    render(<LeadListPage />, { wrapper: Wrapper });
    await waitFor(() => expect(screen.getByText('Alice Smith')).toBeInTheDocument());
    expect(screen.getByText('bob@crm.com')).toBeInTheDocument();
  });

  it('renders Add Lead button', () => {
    render(<LeadListPage />, { wrapper: Wrapper });
    expect(screen.getByRole('button', { name: /add lead/i })).toBeInTheDocument();
  });

  it('opens create dialog on Add Lead click', async () => {
    render(<LeadListPage />, { wrapper: Wrapper });
    fireEvent.click(screen.getByRole('button', { name: /add lead/i }));
    await waitFor(() => expect(screen.getByRole('dialog')).toBeInTheDocument());
  });

  it('calls deleteLead after confirm', async () => {
    vi.mocked(leadsApi.deleteLead).mockResolvedValue(undefined);

    render(<LeadListPage />, { wrapper: Wrapper });
    await waitFor(() => expect(screen.getByText('Alice Smith')).toBeInTheDocument());

    fireEvent.click(screen.getAllByRole('button', { name: /delete/i })[0]);
    const confirmBtn = await screen.findByRole('button', { name: /confirm/i });
    fireEvent.click(confirmBtn);

    await waitFor(() => expect(leadsApi.deleteLead).toHaveBeenCalledWith('lead-1'));
  });
});
