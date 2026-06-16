import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import CustomerListPage from '../../pages/Customers/CustomerListPage';
import * as customersApi from '../../api/customers';

vi.mock('../../api/customers');

const pagedResult = {
  items: [
    { id: 'abc-1', firstName: 'Alice', lastName: 'Smith', email: 'alice@crm.com', company: 'Acme', status: 'Active', jobTitle: 'CEO', createdAt: '2026-01-01' },
    { id: 'abc-2', firstName: 'Bob', lastName: 'Jones', email: 'bob@crm.com', company: 'Corp', status: 'Lead', jobTitle: 'Sales', createdAt: '2026-01-02' },
  ],
  total: 2,
  page: 1,
  pageSize: 10,
};

function Wrapper({ children }: { children: React.ReactNode }) {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  return <QueryClientProvider client={qc}>{children}</QueryClientProvider>;
}

describe('CustomerListPage', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(customersApi.getCustomers).mockResolvedValue(pagedResult);
  });

  it('renders page heading', () => {
    render(<CustomerListPage />, { wrapper: Wrapper });
    expect(screen.getByText('Customers')).toBeInTheDocument();
  });

  it('shows customer rows after load', async () => {
    render(<CustomerListPage />, { wrapper: Wrapper });
    await waitFor(() => expect(screen.getByText('Alice Smith')).toBeInTheDocument());
    expect(screen.getByText('bob@crm.com')).toBeInTheDocument();
  });

  it('renders Add Customer button', async () => {
    render(<CustomerListPage />, { wrapper: Wrapper });
    expect(screen.getByRole('button', { name: /add customer/i })).toBeInTheDocument();
  });

  it('opens create dialog on Add Customer click', async () => {
    render(<CustomerListPage />, { wrapper: Wrapper });
    fireEvent.click(screen.getByRole('button', { name: /add customer/i }));
    await waitFor(() => expect(screen.getByRole('dialog')).toBeInTheDocument());
  });

  it('calls deleteCustomer after confirm', async () => {
    vi.mocked(customersApi.deleteCustomer).mockResolvedValue(undefined);

    render(<CustomerListPage />, { wrapper: Wrapper });
    await waitFor(() => expect(screen.getByText('Alice Smith')).toBeInTheDocument());

    fireEvent.click(screen.getAllByRole('button', { name: /delete/i })[0]);
    const confirmBtn = await screen.findByRole('button', { name: /confirm/i });
    fireEvent.click(confirmBtn);

    await waitFor(() => expect(customersApi.deleteCustomer).toHaveBeenCalledWith('abc-1'));
  });
});
