import {
  Dialog, DialogTitle, DialogContent, DialogActions,
  Button, TextField, MenuItem, Box,
} from '@mui/material';
import { useForm, Controller } from 'react-hook-form';
import type { ActivitySummary, CreateActivityRequest, ActivityType, ActivityStatus, Priority } from '../../api/activities';

interface Props {
  open: boolean;
  onClose: () => void;
  onSave: (data: CreateActivityRequest) => void;
  initial?: ActivitySummary | null;
}

const ACTIVITY_TYPES: ActivityType[] = ['Task', 'Call', 'Email', 'Meeting', 'Note'];
const STATUSES: ActivityStatus[] = ['NotStarted', 'InProgress', 'Completed', 'Cancelled'];
const PRIORITIES: Priority[] = ['Low', 'Medium', 'High'];

export default function ActivityFormDialog({ open, onClose, onSave, initial }: Props) {
  const { handleSubmit, control, formState: { errors }, reset } = useForm<CreateActivityRequest>({
    defaultValues: {
      title: initial?.title ?? '',
      activityType: initial?.activityType ?? 'Task',
      status: initial?.status ?? 'NotStarted',
      priority: initial?.priority ?? 'Medium',
      dueDate: initial?.dueDate ?? null,
    },
  });

  const onSubmit = (data: CreateActivityRequest) => {
    onSave(data);
    reset();
    onClose();
  };

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>{initial ? 'Edit Activity' : 'Add Activity'}</DialogTitle>
      <DialogContent>
        <Box component="form" id="activity-form" onSubmit={handleSubmit(onSubmit)} sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 1 }}>
          <Controller
            name="title"
            control={control}
            rules={{ required: 'Title is required' }}
            render={({ field }) => (
              <TextField
                {...field}
                label="Title"
                error={!!errors.title}
                helperText={errors.title?.message}
                fullWidth
              />
            )}
          />
          <Controller
            name="activityType"
            control={control}
            render={({ field }) => (
              <TextField select label="Type" fullWidth {...field}>
                {ACTIVITY_TYPES.map(t => <MenuItem key={t} value={t}>{t}</MenuItem>)}
              </TextField>
            )}
          />
          <Controller
            name="status"
            control={control}
            render={({ field }) => (
              <TextField select label="Status" fullWidth {...field}>
                {STATUSES.map(s => <MenuItem key={s} value={s}>{s}</MenuItem>)}
              </TextField>
            )}
          />
          <Controller
            name="priority"
            control={control}
            render={({ field }) => (
              <TextField select label="Priority" fullWidth {...field}>
                {PRIORITIES.map(p => <MenuItem key={p} value={p}>{p}</MenuItem>)}
              </TextField>
            )}
          />
          <Controller
            name="dueDate"
            control={control}
            render={({ field }) => (
              <TextField
                {...field}
                value={field.value ?? ''}
                label="Due Date"
                type="date"
                InputLabelProps={{ shrink: true }}
                fullWidth
              />
            )}
          />
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button type="submit" form="activity-form" variant="contained">Save</Button>
      </DialogActions>
    </Dialog>
  );
}
