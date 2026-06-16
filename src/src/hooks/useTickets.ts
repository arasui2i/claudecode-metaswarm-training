import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import * as api from '../api/tickets';

const KEY = 'tickets';

export const useTickets = (query: api.TicketsQuery) =>
  useQuery({ queryKey: [KEY, query], queryFn: () => api.getTickets(query) });

export const useTicket = (id: string) =>
  useQuery({ queryKey: [KEY, id], queryFn: () => api.getTicket(id), enabled: !!id });

export const useCreateTicket = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: api.createTicket,
    onSuccess: () => qc.invalidateQueries({ queryKey: [KEY] }),
  });
};

export const useUpdateTicket = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: api.updateTicket,
    onSuccess: () => qc.invalidateQueries({ queryKey: [KEY] }),
  });
};

export const useDeleteTicket = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => api.deleteTicket(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: [KEY] }),
  });
};
