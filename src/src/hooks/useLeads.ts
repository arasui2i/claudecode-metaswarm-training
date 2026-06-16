import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  getLeads,
  createLead,
  updateLead,
  deleteLead,
  type LeadsQuery,
  type CreateLeadRequest,
  type UpdateLeadRequest,
} from '../api/leads';

const LEADS_KEY = 'leads';

export const useLeads = (query: LeadsQuery) =>
  useQuery({
    queryKey: [LEADS_KEY, query],
    queryFn: () => getLeads(query),
  });

export const useCreateLead = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (body: CreateLeadRequest) => createLead(body),
    onSuccess: () => qc.invalidateQueries({ queryKey: [LEADS_KEY] }),
  });
};

export const useUpdateLead = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (body: UpdateLeadRequest) => updateLead(body),
    onSuccess: () => qc.invalidateQueries({ queryKey: [LEADS_KEY] }),
  });
};

export const useDeleteLead = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => deleteLead(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: [LEADS_KEY] }),
  });
};
