import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import * as api from '../api/activities';

const KEY = 'activities';

export const useActivities = (query: api.ActivitiesQuery) =>
  useQuery({ queryKey: [KEY, query], queryFn: () => api.getActivities(query) });

export const useActivity = (id: string) =>
  useQuery({ queryKey: [KEY, id], queryFn: () => api.getActivity(id), enabled: !!id });

export const useCreateActivity = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: api.createActivity,
    onSuccess: () => qc.invalidateQueries({ queryKey: [KEY] }),
  });
};

export const useUpdateActivity = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: api.updateActivity,
    onSuccess: () => qc.invalidateQueries({ queryKey: [KEY] }),
  });
};

export const useDeleteActivity = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => api.deleteActivity(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: [KEY] }),
  });
};
