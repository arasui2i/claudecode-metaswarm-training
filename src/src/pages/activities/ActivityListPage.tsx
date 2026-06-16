import { useState } from 'react';
import {
  Box, Button, Chip, CircularProgress, IconButton,
  Paper, Table, TableBody, TableCell, TableContainer,
  TableHead, TableRow, TextField, Typography, Dialog,
  DialogTitle, DialogContent, DialogActions,
} from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import { useActivities, useCreateActivity, useUpdateActivity, useDeleteActivity } from '../../hooks/useActivities';
import ActivityFormDialog from './ActivityFormDialog';
import type { ActivitySummary, CreateActivityRequest, Priority, ActivityStatus } from '../../api/activities';

const STATUS_COLOR: Record<ActivityStatus, 'default' | 'info' | 'success' | 'error'> = {
  NotStarted: 'default',
  InProgress: 'info',
  Completed: 'success',
  Cancelled: 'error',
};

const PRIORITY_COLOR: Record<Priority, 'default' | 'warning' | 'error'> = {
  Low: 'default',
  Medium: 'warning',
  High: 'error',
};

export default function ActivityListPage() {
  const [search, setSearch] = useState('');
  const [dialogOpen, setDialogOpen] = useState(false);
  const [dialogKey, setDialogKey] = useState(0);
  const [editing, setEditing] = useState<ActivitySummary | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<string | null>(null);

  const { data, isLoading } = useActivities({ search });
  const createMutation = useCreateActivity();
  const updateMutation = useUpdateActivity();
  const deleteMutation = useDeleteActivity();

  const handleSave = (formData: CreateActivityRequest) => {
    if (editing) {
      updateMutation.mutate({ ...formData, id: editing.id });
    } else {
      createMutation.mutate(formData);
    }
  };

  const handleEdit = (activity: ActivitySummary) => {
    setEditing(activity);
    setDialogOpen(true);
    setDialogKey(k => k + 1);
  };

  const handleAdd = () => {
    setEditing(null);
    setDialogOpen(true);
    setDialogKey(k => k + 1);
  };

  return (
    <Box p={3}>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
        <Typography variant="h5">Activities</Typography>
        <Button variant="contained" onClick={handleAdd}>Add Activity</Button>
      </Box>

      <TextField
        placeholder="Search activities..."
        value={search}
        onChange={e => setSearch(e.target.value)}
        size="small"
        sx={{ mb: 2, width: 300 }}
      />

      {isLoading ? (
        <CircularProgress />
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Title</TableCell>
                <TableCell>Type</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Priority</TableCell>
                <TableCell>Due Date</TableCell>
                <TableCell>Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {data?.items.map(activity => (
                <TableRow key={activity.id}>
                  <TableCell>{activity.title}</TableCell>
                  <TableCell>{activity.activityType}</TableCell>
                  <TableCell>
                    <Chip
                      label={activity.status}
                      color={STATUS_COLOR[activity.status]}
                      size="small"
                    />
                  </TableCell>
                  <TableCell>
                    <Chip
                      label={activity.priority}
                      color={PRIORITY_COLOR[activity.priority]}
                      size="small"
                    />
                  </TableCell>
                  <TableCell>
                    {activity.dueDate ? new Date(activity.dueDate).toLocaleDateString() : '—'}
                  </TableCell>
                  <TableCell>
                    <IconButton aria-label="edit" size="small" onClick={() => handleEdit(activity)}>
                      <EditIcon fontSize="small" />
                    </IconButton>
                    <IconButton aria-label="delete" size="small" onClick={() => setDeleteTarget(activity.id)}>
                      <DeleteIcon fontSize="small" />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      <ActivityFormDialog
        key={dialogKey}
        open={dialogOpen}
        onClose={() => setDialogOpen(false)}
        onSave={handleSave}
        initial={editing}
      />

      <Dialog open={!!deleteTarget} onClose={() => setDeleteTarget(null)}>
        <DialogTitle>Delete Activity</DialogTitle>
        <DialogContent>Are you sure you want to delete this activity?</DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteTarget(null)}>Cancel</Button>
          <Button
            variant="contained"
            color="error"
            onClick={() => {
              if (deleteTarget) deleteMutation.mutate(deleteTarget);
              setDeleteTarget(null);
            }}
          >
            Confirm
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
