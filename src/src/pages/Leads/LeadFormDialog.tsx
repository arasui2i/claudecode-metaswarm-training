import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Grid,
  TextField,
} from '@mui/material';
import { useForm } from 'react-hook-form';
import type { CreateLeadRequest } from '../../api/leads';

interface Props {
  open: boolean;
  onClose: () => void;
  onSubmit: (data: CreateLeadRequest) => void;
  defaultValues?: Partial<CreateLeadRequest>;
  title?: string;
}

export default function LeadFormDialog({ open, onClose, onSubmit, defaultValues, title = 'Add Lead' }: Props) {
  const { register, handleSubmit, formState: { errors }, reset } = useForm<CreateLeadRequest>({
    defaultValues,
  });

  const handleClose = () => {
    reset();
    onClose();
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>{title}</DialogTitle>
      <form onSubmit={handleSubmit(onSubmit)}>
        <DialogContent>
          <Grid container spacing={2} sx={{ mt: 0.5 }}>
            <Grid item xs={6}>
              <TextField
                label="First Name"
                fullWidth
                {...register('firstName', { required: 'First name is required' })}
                error={!!errors.firstName}
                helperText={errors.firstName?.message}
              />
            </Grid>
            <Grid item xs={6}>
              <TextField
                label="Last Name"
                fullWidth
                {...register('lastName', { required: 'Last name is required' })}
                error={!!errors.lastName}
                helperText={errors.lastName?.message}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                label="Email"
                type="email"
                fullWidth
                {...register('email', {
                  required: 'Email is required',
                  pattern: { value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/, message: 'Invalid email' },
                })}
                error={!!errors.email}
                helperText={errors.email?.message}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField label="Company" fullWidth {...register('company')} />
            </Grid>
            <Grid item xs={6}>
              <TextField label="Job Title" fullWidth {...register('jobTitle')} />
            </Grid>
            <Grid item xs={6}>
              <TextField label="Phone" fullWidth {...register('phoneNumber')} />
            </Grid>
            <Grid item xs={12}>
              <TextField label="Notes" fullWidth multiline rows={3} {...register('notes')} />
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>Cancel</Button>
          <Button type="submit" variant="contained">Save</Button>
        </DialogActions>
      </form>
    </Dialog>
  );
}
