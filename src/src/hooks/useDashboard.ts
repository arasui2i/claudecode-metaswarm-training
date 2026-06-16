import { useQuery } from '@tanstack/react-query';
import { getDashboardSummary } from '../api/dashboard';

export const useDashboardSummary = () =>
  useQuery({
    queryKey: ['dashboard', 'summary'],
    queryFn: getDashboardSummary,
    staleTime: 5 * 60 * 1000,
  });
