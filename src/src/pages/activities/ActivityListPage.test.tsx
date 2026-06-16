import { render, screen, fireEvent, waitFor, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import ActivityListPage from './ActivityListPage';
import * as api from '../../api/activities';

vi.mock('../../api/activities');

const makeClient = () => new QueryClient({ defaultOptions: { queries: { retry: false } } });

const pagedResult: api.PagedResult<api.ActivitySummary> = {
  items: [
    {
      id: 'a-1',
      title: 'Follow-up call',
      activityType: 'Call',
      status: 'NotStarted',
      priority: 'High',
      dueDate: '2026-06-20T00:00:00Z',
      relatedEntityType: null,
      relatedEntityId: null,
      createdAt: '2026-06-01T00:00:00Z',
    },
    {
      id: 'a-2',
      title: 'Send proposal email',
      activityType: 'Email',
      status: 'InProgress',
      priority: 'Medium',
      dueDate: null,
      relatedEntityType: 'Customer',
      relatedEntityId: 'c-1',
      createdAt: '2026-06-02T00:00:00Z',
    },
  ],
  total: 2,
  page: 1,
  pageSize: 10,
};

describe('ActivityListPage', () => {
  beforeEach(() => {
    vi.mocked(api.getActivities).mockResolvedValue(pagedResult);
    vi.mocked(api.createActivity).mockResolvedValue({ id: 'a-3' });
    vi.mocked(api.updateActivity).mockResolvedValue(undefined);
    vi.mocked(api.deleteActivity).mockResolvedValue(undefined);
  });

  const renderPage = () =>
    render(
      <QueryClientProvider client={makeClient()}>
        <ActivityListPage />
      </QueryClientProvider>
    );

  it('renders the page heading', async () => {
    renderPage();
    expect(await screen.findByText(/activities/i)).toBeInTheDocument();
  });

  it('displays activity list from API', async () => {
    renderPage();
    expect(await screen.findByText('Follow-up call')).toBeInTheDocument();
    expect(await screen.findByText('Send proposal email')).toBeInTheDocument();
  });

  it('shows activity type and status chips', async () => {
    renderPage();
    expect(await screen.findByText('Call')).toBeInTheDocument();
    expect(await screen.findByText('NotStarted')).toBeInTheDocument();
  });

  it('opens create dialog on Add button click', async () => {
    renderPage();
    await screen.findByText('Follow-up call');
    fireEvent.click(screen.getByRole('button', { name: /add activity/i }));
    expect(await screen.findByRole('dialog')).toBeInTheDocument();
  });

  it('shows Save and Cancel buttons in create dialog', async () => {
    renderPage();
    await screen.findByText('Follow-up call');
    fireEvent.click(screen.getByRole('button', { name: /add activity/i }));
    const dialog = await screen.findByRole('dialog');
    expect(within(dialog).getByRole('button', { name: /save/i })).toBeInTheDocument();
    expect(within(dialog).getByRole('button', { name: /cancel/i })).toBeInTheDocument();
  });

  it('opens edit dialog when Edit clicked', async () => {
    renderPage();
    await screen.findByText('Follow-up call');
    fireEvent.click(screen.getAllByRole('button', { name: /edit/i })[0]);
    const dialog = await screen.findByRole('dialog');
    expect(dialog).toBeInTheDocument();
    expect(within(dialog).getByDisplayValue('Follow-up call')).toBeInTheDocument();
  });

  it('calls deleteActivity after confirmation', async () => {
    renderPage();
    await screen.findByText('Follow-up call');
    fireEvent.click(screen.getAllByRole('button', { name: /delete/i })[0]);
    fireEvent.click(await screen.findByRole('button', { name: /confirm/i }));
    await waitFor(() => expect(api.deleteActivity).toHaveBeenCalledWith('a-1'));
  });

  it('filters activities via search input', async () => {
    renderPage();
    await screen.findByText('Follow-up call');
    const search = screen.getByPlaceholderText(/search/i);
    fireEvent.change(search, { target: { value: 'call' } });
    await waitFor(() => expect(api.getActivities).toHaveBeenCalledWith(
      expect.objectContaining({ search: 'call' })
    ));
  });
});
