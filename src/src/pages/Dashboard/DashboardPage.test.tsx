import { render, screen, fireEvent } from '@testing-library/react';
import { vi, describe, it, expect, beforeEach } from 'vitest';
import DashboardPage from './DashboardPage';
import * as useDashboard from '../../hooks/useDashboard';

vi.mock('../../hooks/useDashboard');
vi.mock('recharts', () => ({
  PieChart: ({ children }: { children: React.ReactNode }) => <div data-testid="pie-chart">{children}</div>,
  Pie: () => <div />,
  Cell: () => <div />,
  Tooltip: () => <div />,
  Legend: () => <div />,
}));

const mockSummary = {
  currentMonthLeads: 12,
  convertedCustomersThisMonth: 4,
  ticketsByStatus: { new: 3, inProgress: 2, pending: 1, resolved: 5, closed: 8 },
};

describe('DashboardPage', () => {
  const mockRefetch = vi.fn();

  beforeEach(() => {
    vi.mocked(useDashboard.useDashboardSummary).mockReturnValue({
      data: mockSummary,
      isLoading: false,
      refetch: mockRefetch,
    } as never);
  });

  it('renders Dashboard heading', () => {
    render(<DashboardPage />);
    expect(screen.getByText(/dashboard/i)).toBeInTheDocument();
  });

  it('displays lead count KPI', () => {
    render(<DashboardPage />);
    expect(screen.getByText('12')).toBeInTheDocument();
    expect(screen.getByText(/leads this month/i)).toBeInTheDocument();
  });

  it('displays converted customers KPI', () => {
    render(<DashboardPage />);
    expect(screen.getByText('4')).toBeInTheDocument();
    expect(screen.getByText(/customers converted/i)).toBeInTheDocument();
  });

  it('shows Skeleton while loading', () => {
    vi.mocked(useDashboard.useDashboardSummary).mockReturnValue({
      data: undefined,
      isLoading: true,
      refetch: mockRefetch,
    } as never);

    render(<DashboardPage />);
    // Skeleton elements render as span elements
    const skeletons = document.querySelectorAll('.MuiSkeleton-root');
    expect(skeletons.length).toBeGreaterThan(0);
  });

  it('calls refetch when Refresh button clicked', () => {
    render(<DashboardPage />);
    fireEvent.click(screen.getByRole('button', { name: /refresh/i }));
    expect(mockRefetch).toHaveBeenCalledOnce();
  });

  it('renders ticket status chart section', () => {
    render(<DashboardPage />);
    expect(screen.getByText(/ticket status summary/i)).toBeInTheDocument();
    expect(screen.getByTestId('pie-chart')).toBeInTheDocument();
  });
});
