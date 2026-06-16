import { useState } from 'react';
import {
  Box,
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  InputAdornment,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow,
  TextField,
  Tooltip,
  Typography,
} from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import SearchIcon from '@mui/icons-material/Search';
import AddIcon from '@mui/icons-material/Add';
import { useLeads, useCreateLead, useUpdateLead, useDeleteLead } from '../../hooks/useLeads';
import LeadFormDialog from './LeadFormDialog';
import type { CreateLeadRequest, LeadSummary } from '../../api/leads';

export default function LeadListPage() {
  const [search, setSearch] = useState('');
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);

  const [formOpen, setFormOpen] = useState(false);
  const [editTarget, setEditTarget] = useState<LeadSummary | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<LeadSummary | null>(null);

  const { data, isLoading } = useLeads({ search, page: page + 1, pageSize });
  const createLead = useCreateLead();
  const updateLead = useUpdateLead();
  const deleteLead = useDeleteLead();

  const handleCreate = (body: CreateLeadRequest) => {
    createLead.mutate(body, { onSuccess: () => setFormOpen(false) });
  };

  const handleUpdate = (body: CreateLeadRequest) => {
    if (!editTarget) return;
    updateLead.mutate({ id: editTarget.id, ...body }, { onSuccess: () => setEditTarget(null) });
  };

  const handleDelete = () => {
    if (!deleteTarget) return;
    deleteLead.mutate(deleteTarget.id, { onSuccess: () => setDeleteTarget(null) });
  };

  return (
    <Box sx={{ p: 3 }}>
      <Box sx={{ display: 'flex', alignItems: 'center', mb: 2, gap: 2 }}>
        <Typography variant="h4" sx={{ flexGrow: 1 }}>Leads</Typography>
        <TextField
          size="small"
          placeholder="Search…"
          value={search}
          onChange={e => { setSearch(e.target.value); setPage(0); }}
          InputProps={{ startAdornment: <InputAdornment position="start"><SearchIcon /></InputAdornment> }}
        />
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => setFormOpen(true)}>
          Add Lead
        </Button>
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Name</TableCell>
              <TableCell>Email</TableCell>
              <TableCell>Company</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Source</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {isLoading ? (
              <TableRow>
                <TableCell colSpan={6} align="center"><CircularProgress /></TableCell>
              </TableRow>
            ) : data?.items.map(l => (
              <TableRow key={l.id}>
                <TableCell>{l.firstName} {l.lastName}</TableCell>
                <TableCell>{l.email}</TableCell>
                <TableCell>{l.company}</TableCell>
                <TableCell>{l.status}</TableCell>
                <TableCell>{l.source}</TableCell>
                <TableCell align="right">
                  <Tooltip title="Edit">
                    <IconButton size="small" onClick={() => setEditTarget(l)} aria-label="edit">
                      <EditIcon fontSize="small" />
                    </IconButton>
                  </Tooltip>
                  <Tooltip title="Delete">
                    <IconButton size="small" onClick={() => setDeleteTarget(l)} aria-label="delete">
                      <DeleteIcon fontSize="small" />
                    </IconButton>
                  </Tooltip>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      <TablePagination
        component="div"
        count={data?.total ?? 0}
        page={page}
        rowsPerPage={pageSize}
        onPageChange={(_, p) => setPage(p)}
        onRowsPerPageChange={e => { setPageSize(+e.target.value); setPage(0); }}
        rowsPerPageOptions={[10, 25, 50]}
      />

      <LeadFormDialog
        open={formOpen}
        onClose={() => setFormOpen(false)}
        onSubmit={handleCreate}
        title="Add Lead"
      />

      <LeadFormDialog
        open={!!editTarget}
        onClose={() => setEditTarget(null)}
        onSubmit={handleUpdate}
        defaultValues={editTarget ?? undefined}
        title="Edit Lead"
      />

      <Dialog open={!!deleteTarget} onClose={() => setDeleteTarget(null)}>
        <DialogTitle>Delete Lead</DialogTitle>
        <DialogContent>
          Are you sure you want to delete{' '}
          <strong>{deleteTarget?.firstName} {deleteTarget?.lastName}</strong>?
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteTarget(null)}>Cancel</Button>
          <Button color="error" variant="contained" onClick={handleDelete} aria-label="confirm">
            Confirm
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
