import { Button, Dialog, DialogActions, DialogContent, DialogTitle, Grid, TextField } from '@mui/material';
import { useForm } from 'react-hook-form';
import type { CreateAccountRequest } from '../../api/accounts';

interface Props {
  open: boolean;
  onClose: () => void;
  onSubmit: (data: CreateAccountRequest) => void;
  defaultValues?: Partial<CreateAccountRequest>;
  title?: string;
}

export default function AccountFormDialog({ open, onClose, onSubmit, defaultValues, title = 'Add Account' }: Props) {
  const { register, handleSubmit, formState: { errors }, reset } = useForm<CreateAccountRequest>({ defaultValues });
  const handleClose = () => { reset(); onClose(); };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>{title}</DialogTitle>
      <form onSubmit={handleSubmit(onSubmit)}>
        <DialogContent>
          <Grid container spacing={2} sx={{ mt: 0.5 }}>
            <Grid item xs={12}>
              <TextField label="Account Name" fullWidth
                {...register('name', { required: 'Account name is required' })}
                error={!!errors.name} helperText={errors.name?.message} />
            </Grid>
            <Grid item xs={6}>
              <TextField label="Industry" fullWidth {...register('industry')} />
            </Grid>
            <Grid item xs={6}>
              <TextField label="Phone" fullWidth {...register('phoneNumber')} />
            </Grid>
            <Grid item xs={12}>
              <TextField label="Website" fullWidth {...register('website')} />
            </Grid>
            <Grid item xs={6}>
              <TextField label="Employee Count" type="number" fullWidth {...register('employeeCount', { valueAsNumber: true })} />
            </Grid>
            <Grid item xs={6}>
              <TextField label="Annual Revenue" type="number" fullWidth {...register('annualRevenue', { valueAsNumber: true })} />
            </Grid>
            <Grid item xs={12}>
              <TextField label="Billing Address" fullWidth {...register('billingAddress')} />
            </Grid>
            <Grid item xs={12}>
              <TextField label="Description" fullWidth multiline rows={3} {...register('description')} />
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
