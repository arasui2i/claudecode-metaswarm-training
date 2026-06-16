import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  getOpportunities, createOpportunity, updateOpportunity, deleteOpportunity,
  type OpportunitiesQuery, type CreateOpportunityRequest, type UpdateOpportunityRequest,
} from '../api/opportunities';

const KEY = 'opportunities';

export const useOpportunities = (query: OpportunitiesQuery) =>
  useQuery({ queryKey: [KEY, query], queryFn: () => getOpportunities(query) });

export const useCreateOpportunity = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (body: CreateOpportunityRequest) => createOpportunity(body),
    onSuccess: () => qc.invalidateQueries({ queryKey: [KEY] }),
  });
};

export const useUpdateOpportunity = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (body: UpdateOpportunityRequest) => updateOpportunity(body),
    onSuccess: () => qc.invalidateQueries({ queryKey: [KEY] }),
  });
};

export const useDeleteOpportunity = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => deleteOpportunity(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: [KEY] }),
  });
};
