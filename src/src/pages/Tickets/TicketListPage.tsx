import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box, Button, Chip, CircularProgress, IconButton,
  Paper, Table, TableBody, TableCell, TableContainer,
  TableHead, TableRow, TextField, Typography,
  Dialog, DialogTitle, DialogContent, DialogActions,
} from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import { useTickets, useDeleteTicket } from '../../hooks/useTickets';
import type { TicketPriority, TicketStatus } from '../../api/tickets';

const PRIORITY_COLOR: Record<TicketPriority, 'default' | 'info' | 'warning' | 'error'> = {
  Low: 'default',
  Medium: 'info',
  High: 'warning',
  Critical: 'error',
};

const STATUS_COLOR: Record<TicketStatus, 'default' | 'info' | 'success' | 'error' | 'warning'> = {
  Open: 'info',
  InProgress: 'warning',
  Resolved: 'success',
  Closed: 'default',
  Pending: 'error',
};

export default function TicketListPage() {
  const navigate = useNavigate();
  const [search, setSearch] = useState('');
  const [deleteTarget, setDeleteTarget] = useState<string | null>(null);

  const { data, isLoading } = useTickets({ search });
  const deleteMutation = useDeleteTicket();

  return (
    <Box p={3}>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
        <Typography variant="h5">Tickets</Typography>
        <Button variant="contained" onClick={() => navigate('/tickets/new')}>New Ticket</Button>
      </Box>

      <TextField
        placeholder="Search tickets..."
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
                <TableCell>Ticket Number</TableCell>
                <TableCell>Subject</TableCell>
                <TableCell>Priority</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {data?.items.map(ticket => (
                <TableRow key={ticket.id}>
                  <TableCell>{ticket.ticketNumber}</TableCell>
                  <TableCell>{ticket.subject}</TableCell>
                  <TableCell>
                    <Chip label={ticket.priority} color={PRIORITY_COLOR[ticket.priority]} size="small" />
                  </TableCell>
                  <TableCell>
                    <Chip label={ticket.status} color={STATUS_COLOR[ticket.status]} size="small" />
                  </TableCell>
                  <TableCell>
                    <IconButton aria-label="edit" size="small" onClick={() => navigate(`/tickets/${ticket.id}/edit`)}>
                      <EditIcon fontSize="small" />
                    </IconButton>
                    <IconButton aria-label="delete" size="small" onClick={() => setDeleteTarget(ticket.id)}>
                      <DeleteIcon fontSize="small" />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      <Dialog open={!!deleteTarget} onClose={() => setDeleteTarget(null)}>
        <DialogTitle>Delete Ticket</DialogTitle>
        <DialogContent>Are you sure you want to delete this ticket?</DialogContent>
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
