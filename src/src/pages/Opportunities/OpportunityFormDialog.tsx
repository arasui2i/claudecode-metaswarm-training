import { Button, Dialog, DialogActions, DialogContent, DialogTitle, Grid, TextField } from '@mui/material';
import { useForm } from 'react-hook-form';
import type { CreateOpportunityRequest } from '../../api/opportunities';

interface Props {
  open: boolean;
  onClose: () => void;
  onSubmit: (data: CreateOpportunityRequest) => void;
  defaultValues?: Partial<CreateOpportunityRequest>;
  title?: string;
}

export default function OpportunityFormDialog({ open, onClose, onSubmit, defaultValues, title = 'Add Opportunity' }: Props) {
  const { register, handleSubmit, formState: { errors }, reset } = useForm<CreateOpportunityRequest>({ defaultValues });
  const handleClose = () => { reset(); onClose(); };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
      <DialogTitle>{title}</DialogTitle>
      <form onSubmit={handleSubmit(onSubmit)}>
        <DialogContent>
          <Grid container spacing={2} sx={{ mt: 0.5 }}>
            <Grid item xs={12}>
              <TextField label="Opportunity Name" fullWidth
                {...register('name', { required: 'Name is required' })}
                error={!!errors.name} helperText={errors.name?.message} />
            </Grid>
            <Grid item xs={6}>
              <TextField label="Amount ($)" type="number" fullWidth
                {...register('amount', { valueAsNumber: true })} />
            </Grid>
            <Grid item xs={6}>
              <TextField label="Probability (%)" type="number" fullWidth
                {...register('probability', {
                  valueAsNumber: true,
                  min: { value: 0, message: 'Min 0' },
                  max: { value: 100, message: 'Max 100' },
                })}
                error={!!errors.probability} helperText={errors.probability?.message} />
            </Grid>
            <Grid item xs={6}>
              <TextField label="Close Date" type="date" fullWidth InputLabelProps={{ shrink: true }}
                {...register('closeDate')} />
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
