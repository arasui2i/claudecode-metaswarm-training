import axios from './axiosInstance';

export type ActivityType = 'Task' | 'Call' | 'Email' | 'Meeting' | 'Note';
export type ActivityStatus = 'NotStarted' | 'InProgress' | 'Completed' | 'Cancelled';
export type Priority = 'Low' | 'Medium' | 'High';

export interface ActivitySummary {
  id: string;
  title: string;
  activityType: ActivityType;
  status: ActivityStatus;
  priority: Priority;
  dueDate: string | null;
  relatedEntityType: string | null;
  relatedEntityId: string | null;
  createdAt: string;
}

export interface ActivityDetail extends ActivitySummary {
  description: string;
  completedAt: string | null;
  updatedAt: string;
}

export interface PagedResult<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}

export interface ActivitiesQuery {
  search?: string;
  page?: number;
  pageSize?: number;
}

export interface CreateActivityRequest {
  title: string;
  description?: string;
  activityType?: ActivityType;
  status?: ActivityStatus;
  priority?: Priority;
  dueDate?: string | null;
  relatedEntityType?: string | null;
  relatedEntityId?: string | null;
}

export interface UpdateActivityRequest extends CreateActivityRequest {
  id: string;
}

export const getActivities = (query: ActivitiesQuery = {}): Promise<PagedResult<ActivitySummary>> =>
  axios.get('/api/activities', { params: query }).then(r => r.data);

export const getActivity = (id: string): Promise<ActivityDetail> =>
  axios.get(`/api/activities/${id}`).then(r => r.data);

export const createActivity = (data: CreateActivityRequest): Promise<{ id: string }> =>
  axios.post('/api/activities', data).then(r => r.data);

export const updateActivity = (data: UpdateActivityRequest): Promise<void> =>
  axios.put(`/api/activities/${data.id}`, data).then(r => r.data);

export const deleteActivity = (id: string): Promise<void> =>
  axios.delete(`/api/activities/${id}`).then(r => r.data);
