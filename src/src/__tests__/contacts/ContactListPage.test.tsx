import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import ContactListPage from '../../pages/Contacts/ContactListPage';
import * as contactsApi from '../../api/contacts';

vi.mock('../../api/contacts');

const pagedResult = {
  items: [
    { id: 'c-1', firstName: 'Alice', lastName: 'Smith', email: 'alice@crm.com', phoneNumber: '123', jobTitle: 'CEO', contactType: 'Primary', customerId: null, createdAt: '2026-01-01' },
    { id: 'c-2', firstName: 'Bob', lastName: 'Jones', email: 'bob@crm.com', phoneNumber: '456', jobTitle: 'CTO', contactType: 'Secondary', customerId: null, createdAt: '2026-01-02' },
  ],
  total: 2, page: 1, pageSize: 10,
};

function Wrapper({ children }: { children: React.ReactNode }) {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return <QueryClientProvider client={qc}>{children}</QueryClientProvider>;
}

describe('ContactListPage', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(contactsApi.getContacts).mockResolvedValue(pagedResult);
  });

  it('renders page heading', () => {
    render(<ContactListPage />, { wrapper: Wrapper });
    expect(screen.getByText('Contacts')).toBeInTheDocument();
  });

  it('shows contact rows after load', async () => {
    render(<ContactListPage />, { wrapper: Wrapper });
    await waitFor(() => expect(screen.getByText('Alice Smith')).toBeInTheDocument());
    expect(screen.getByText('bob@crm.com')).toBeInTheDocument();
  });

  it('renders Add Contact button', () => {
    render(<ContactListPage />, { wrapper: Wrapper });
    expect(screen.getByRole('button', { name: /add contact/i })).toBeInTheDocument();
  });

  it('opens create dialog on Add Contact click', async () => {
    render(<ContactListPage />, { wrapper: Wrapper });
    fireEvent.click(screen.getByRole('button', { name: /add contact/i }));
    await waitFor(() => expect(screen.getByRole('dialog')).toBeInTheDocument());
  });

  it('calls deleteContact after confirm', async () => {
    vi.mocked(contactsApi.deleteContact).mockResolvedValue(undefined);

    render(<ContactListPage />, { wrapper: Wrapper });
    await waitFor(() => expect(screen.getByText('Alice Smith')).toBeInTheDocument());

    fireEvent.click(screen.getAllByRole('button', { name: /delete/i })[0]);
    const confirmBtn = await screen.findByRole('button', { name: /confirm/i });
    fireEvent.click(confirmBtn);

    await waitFor(() => expect(contactsApi.deleteContact).toHaveBeenCalledWith('c-1'));
  });
});
