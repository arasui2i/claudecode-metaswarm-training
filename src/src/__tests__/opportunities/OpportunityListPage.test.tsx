import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import OpportunityListPage from '../../pages/Opportunities/OpportunityListPage';
import * as api from '../../api/opportunities';

vi.mock('../../api/opportunities');

const pagedResult = {
  items: [
    { id: 'o-1', name: 'Big Deal',   stage: 'Proposal',  amount: 50000, probability: 60, closeDate: '2026-03-01', accountId: null, createdAt: '2026-01-01' },
    { id: 'o-2', name: 'Small Deal', stage: 'ClosedWon', amount: 5000,  probability: 100, closeDate: '2026-01-15', accountId: null, createdAt: '2026-01-02' },
  ],
  total: 2, page: 1, pageSize: 10,
};

function Wrapper({ children }: { children: React.ReactNode }) {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return <QueryClientProvider client={qc}>{children}</QueryClientProvider>;
}

describe('OpportunityListPage', () => {
  beforeEach(() => { vi.clearAllMocks(); vi.mocked(api.getOpportunities).mockResolvedValue(pagedResult); });

  it('renders page heading', () => {
    render(<OpportunityListPage />, { wrapper: Wrapper });
    expect(screen.getByText('Opportunities')).toBeInTheDocument();
  });

  it('shows opportunity rows after load', async () => {
    render(<OpportunityListPage />, { wrapper: Wrapper });
    await waitFor(() => expect(screen.getByText('Big Deal')).toBeInTheDocument());
    expect(screen.getByText('Small Deal')).toBeInTheDocument();
  });

  it('renders Add Opportunity button', () => {
    render(<OpportunityListPage />, { wrapper: Wrapper });
    expect(screen.getByRole('button', { name: /add opportunity/i })).toBeInTheDocument();
  });

  it('opens create dialog on Add Opportunity click', async () => {
    render(<OpportunityListPage />, { wrapper: Wrapper });
    fireEvent.click(screen.getByRole('button', { name: /add opportunity/i }));
    await waitFor(() => expect(screen.getByRole('dialog')).toBeInTheDocument());
  });

  it('calls deleteOpportunity after confirm', async () => {
    vi.mocked(api.deleteOpportunity).mockResolvedValue(undefined);
    render(<OpportunityListPage />, { wrapper: Wrapper });
    await waitFor(() => expect(screen.getByText('Big Deal')).toBeInTheDocument());
    fireEvent.click(screen.getAllByRole('button', { name: /delete/i })[0]);
    fireEvent.click(await screen.findByRole('button', { name: /confirm/i }));
    await waitFor(() => expect(api.deleteOpportunity).toHaveBeenCalledWith('o-1'));
  });
});
