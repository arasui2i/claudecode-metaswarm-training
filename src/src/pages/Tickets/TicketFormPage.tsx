import { useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import {
  Box, Button, CircularProgress, MenuItem,
  Paper, TextField, Typography,
} from '@mui/material';
import { useForm, Controller } from 'react-hook-form';
import { useQuery } from '@tanstack/react-query';
import { useCreateTicket, useUpdateTicket, useTicket } from '../../hooks/useTickets';
import { getAccounts } from '../../api/accounts';
import { getContacts } from '../../api/contacts';
import type { CreateTicketRequest, TicketPriority, TicketStatus } from '../../api/tickets';

const PRIORITIES: TicketPriority[] = ['Low', 'Medium', 'High', 'Critical'];
const STATUSES: TicketStatus[] = ['Open', 'InProgress', 'Resolved', 'Closed', 'Pending'];

export default function TicketFormPage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const isEdit = !!id;

  const { data: ticket, isLoading: ticketLoading } = useTicket(id ?? '');
  const { data: accountsData } = useQuery({
    queryKey: ['accounts', { page: 1, pageSize: 200 }],
    queryFn: () => getAccounts({ page: 1, pageSize: 200 }),
  });
  const { data: contactsData } = useQuery({
    queryKey: ['contacts', { page: 1, pageSize: 200 }],
    queryFn: () => getContacts({ page: 1, pageSize: 200 }),
  });

  const createMutation = useCreateTicket();
  const updateMutation = useUpdateTicket();

  const { handleSubmit, control, reset, formState: { errors } } = useForm<CreateTicketRequest>({
    defaultValues: { subject: '', priority: 'Medium', status: 'Open', accountId: null, contactId: null },
  });

  useEffect(() => {
    if (ticket) {
      reset({
        subject: ticket.subject,
        priority: ticket.priority,
        status: ticket.status,
        accountId: ticket.accountId,
        contactId: ticket.contactId,
      });
    }
  }, [ticket, reset]);

  const onSubmit = (data: CreateTicketRequest) => {
    if (isEdit && id) {
      updateMutation.mutate({ ...data, id }, { onSuccess: () => navigate('/tickets') });
    } else {
      createMutation.mutate(data, { onSuccess: () => navigate('/tickets') });
    }
  };

  if (isEdit && ticketLoading) return <CircularProgress />;

  return (
    <Box p={3} maxWidth={600}>
      <Typography variant="h5" mb={3}>{isEdit ? 'Edit Ticket' : 'New Ticket'}</Typography>

      {isEdit && ticket && (
        <TextField
          label="Ticket Number"
          value={ticket.ticketNumber}
          slotProps={{ input: { readOnly: true } }}
          fullWidth
          sx={{ mb: 2 }}
        />
      )}

      <Paper sx={{ p: 3 }}>
        <Box
          component="form"
          id="ticket-form"
          onSubmit={handleSubmit(onSubmit)}
          sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}
        >
          <Controller
            name="subject"
            control={control}
            rules={{ required: 'Subject is required' }}
            render={({ field }) => (
              <TextField
                {...field}
                label="Subject"
                error={!!errors.subject}
                helperText={errors.subject?.message}
                fullWidth
              />
            )}
          />

          <Controller
            name="priority"
            control={control}
            rules={{ required: 'Priority is required' }}
            render={({ field }) => (
              <TextField select label="Priority" fullWidth {...field} error={!!errors.priority} helperText={errors.priority?.message}>
                {PRIORITIES.map(p => <MenuItem key={p} value={p}>{p}</MenuItem>)}
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
            name="accountId"
            control={control}
            render={({ field }) => (
              <TextField select label="Account" fullWidth {...field} value={field.value ?? ''}>
                <MenuItem value="">— None —</MenuItem>
                {accountsData?.items.map(a => (
                  <MenuItem key={a.id} value={a.id}>{a.name}</MenuItem>
                ))}
              </TextField>
            )}
          />

          <Controller
            name="contactId"
            control={control}
            render={({ field }) => (
              <TextField select label="Contact" fullWidth {...field} value={field.value ?? ''}>
                <MenuItem value="">— None —</MenuItem>
                {contactsData?.items.map(c => (
                  <MenuItem key={c.id} value={c.id}>{c.firstName} {c.lastName}</MenuItem>
                ))}
              </TextField>
            )}
          />
        </Box>
      </Paper>

      <Box mt={2} display="flex" gap={1}>
        <Button variant="outlined" onClick={() => navigate('/tickets')}>Cancel</Button>
        <Button type="submit" form="ticket-form" variant="contained">
          {isEdit ? 'Update Ticket' : 'Create Ticket'}
        </Button>
      </Box>
    </Box>
  );
}
