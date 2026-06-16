import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { MemoryRouter } from 'react-router-dom';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import TicketListPage from './TicketListPage';
import * as api from '../../api/tickets';

vi.mock('../../api/tickets');

const makeClient = () => new QueryClient({ defaultOptions: { queries: { retry: false } } });

const pagedResult: api.PagedResult<api.TicketSummary> = {
  items: [
    { id: 't-1', ticketNumber: 'TKT-00001', subject: 'Server down',  priority: 'High',   status: 'Open' },
    { id: 't-2', ticketNumber: 'TKT-00002', subject: 'Login issue',  priority: 'Medium', status: 'InProgress' },
  ],
  total: 2, page: 1, pageSize: 10,
};

describe('TicketListPage', () => {
  beforeEach(() => {
    vi.mocked(api.getTickets).mockResolvedValue(pagedResult);
    vi.mocked(api.deleteTicket).mockResolvedValue(undefined);
  });

  const renderPage = () =>
    render(
      <QueryClientProvider client={makeClient()}>
        <MemoryRouter>
          <TicketListPage />
        </MemoryRouter>
      </QueryClientProvider>
    );

  it('renders the page heading', async () => {
    renderPage();
    expect(await screen.findByText(/tickets/i)).toBeInTheDocument();
  });

  it('displays ticket list with ticket numbers', async () => {
    renderPage();
    expect(await screen.findByText('TKT-00001')).toBeInTheDocument();
    expect(await screen.findByText('TKT-00002')).toBeInTheDocument();
  });

  it('shows subject column values', async () => {
    renderPage();
    expect(await screen.findByText('Server down')).toBeInTheDocument();
    expect(await screen.findByText('Login issue')).toBeInTheDocument();
  });

  it('shows priority and status chips', async () => {
    renderPage();
    expect(await screen.findByText('High')).toBeInTheDocument();
    expect(await screen.findByText('Open')).toBeInTheDocument();
  });

  it('calls deleteTicket after confirm', async () => {
    renderPage();
    await screen.findByText('TKT-00001');
    fireEvent.click(screen.getAllByRole('button', { name: /delete/i })[0]);
    fireEvent.click(await screen.findByRole('button', { name: /confirm/i }));
    await waitFor(() => expect(api.deleteTicket).toHaveBeenCalledWith('t-1'));
  });

  it('filters tickets via search input', async () => {
    renderPage();
    await screen.findByText('TKT-00001');
    fireEvent.change(screen.getByPlaceholderText(/search/i), { target: { value: 'server' } });
    await waitFor(() => expect(api.getTickets).toHaveBeenCalledWith(
      expect.objectContaining({ search: 'server' })
    ));
  });
});
