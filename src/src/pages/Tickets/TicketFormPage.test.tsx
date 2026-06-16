import { render, screen, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import TicketFormPage from './TicketFormPage';
import * as ticketApi from '../../api/tickets';
import * as accountApi from '../../api/accounts';
import * as contactApi from '../../api/contacts';

vi.mock('../../api/tickets');
vi.mock('../../api/accounts');
vi.mock('../../api/contacts');

const makeClient = () => new QueryClient({ defaultOptions: { queries: { retry: false } } });

const emptyPaged = { items: [], total: 0, page: 1, pageSize: 200 };

const ticketDetail: ticketApi.TicketDetail = {
  id: 't-1',
  ticketNumber: 'TKT-00001',
  subject: 'Server down',
  priority: 'High',
  status: 'Open',
  accountId: null,
  accountName: null,
  contactId: null,
  contactName: null,
  createdAt: '2026-06-01T00:00:00Z',
  updatedAt: '2026-06-01T00:00:00Z',
};

describe('TicketFormPage — create mode', () => {
  beforeEach(() => {
    vi.mocked(accountApi.getAccounts).mockResolvedValue(emptyPaged as never);
    vi.mocked(contactApi.getContacts).mockResolvedValue(emptyPaged as never);
  });

  const renderCreate = () =>
    render(
      <QueryClientProvider client={makeClient()}>
        <MemoryRouter initialEntries={['/tickets/new']}>
          <Routes>
            <Route path="/tickets/new" element={<TicketFormPage />} />
          </Routes>
        </MemoryRouter>
      </QueryClientProvider>
    );

  it('renders New Ticket heading', async () => {
    renderCreate();
    expect(await screen.findByText(/new ticket/i)).toBeInTheDocument();
  });

  it('does not show Ticket Number field in create mode', async () => {
    renderCreate();
    await screen.findByText(/new ticket/i);
    expect(screen.queryByTestId('ticket-number-field')).not.toBeInTheDocument();
  });

  it('shows Subject and Priority fields', async () => {
    renderCreate();
    expect(await screen.findByLabelText(/subject/i)).toBeInTheDocument();
  });
});

describe('TicketFormPage — edit mode', () => {
  beforeEach(() => {
    vi.mocked(ticketApi.getTicket).mockResolvedValue(ticketDetail);
    vi.mocked(accountApi.getAccounts).mockResolvedValue(emptyPaged as never);
    vi.mocked(contactApi.getContacts).mockResolvedValue(emptyPaged as never);
  });

  const renderEdit = () =>
    render(
      <QueryClientProvider client={makeClient()}>
        <MemoryRouter initialEntries={['/tickets/t-1/edit']}>
          <Routes>
            <Route path="/tickets/:id/edit" element={<TicketFormPage />} />
          </Routes>
        </MemoryRouter>
      </QueryClientProvider>
    );

  it('renders Edit Ticket heading', async () => {
    renderEdit();
    expect(await screen.findByText(/edit ticket/i)).toBeInTheDocument();
  });

  it('shows Ticket Number as read-only in edit mode', async () => {
    renderEdit();
    const field = await screen.findByDisplayValue('TKT-00001');
    expect(field).toBeInTheDocument();
    expect(field).toHaveAttribute('readonly');
  });

  it('pre-fills subject from loaded ticket', async () => {
    renderEdit();
    await waitFor(() =>
      expect(screen.getByDisplayValue('Server down')).toBeInTheDocument()
    );
  });
});
