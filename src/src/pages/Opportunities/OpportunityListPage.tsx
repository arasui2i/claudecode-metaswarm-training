import { useState } from 'react';
import {
  Box, Button, Chip, CircularProgress, Dialog, DialogActions, DialogContent, DialogTitle,
  IconButton, InputAdornment, Paper, Table, TableBody, TableCell, TableContainer,
  TableHead, TablePagination, TableRow, TextField, Tooltip, Typography,
} from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import SearchIcon from '@mui/icons-material/Search';
import AddIcon from '@mui/icons-material/Add';
import { useOpportunities, useCreateOpportunity, useUpdateOpportunity, useDeleteOpportunity } from '../../hooks/useOpportunities';
import OpportunityFormDialog from './OpportunityFormDialog';
import type { CreateOpportunityRequest, OpportunitySummary } from '../../api/opportunities';

const stageColor: Record<string, 'default' | 'primary' | 'success' | 'error' | 'warning' | 'info'> = {
  Prospecting: 'default', Qualification: 'info', Proposal: 'primary',
  Negotiation: 'warning', ClosedWon: 'success', ClosedLost: 'error',
};

export default function OpportunityListPage() {
  const [search, setSearch] = useState('');
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [formOpen, setFormOpen] = useState(false);
  const [editTarget, setEditTarget] = useState<OpportunitySummary | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<OpportunitySummary | null>(null);

  const { data, isLoading } = useOpportunities({ search, page: page + 1, pageSize });
  const createOpportunity = useCreateOpportunity();
  const updateOpportunity = useUpdateOpportunity();
  const deleteOpportunity = useDeleteOpportunity();

  const handleCreate = (body: CreateOpportunityRequest) =>
    createOpportunity.mutate(body, { onSuccess: () => setFormOpen(false) });

  const handleUpdate = (body: CreateOpportunityRequest) => {
    if (!editTarget) return;
    updateOpportunity.mutate({ id: editTarget.id, ...body }, { onSuccess: () => setEditTarget(null) });
  };

  const handleDelete = () => {
    if (!deleteTarget) return;
    deleteOpportunity.mutate(deleteTarget.id, { onSuccess: () => setDeleteTarget(null) });
  };

  return (
    <Box sx={{ p: 3 }}>
      <Box sx={{ display: 'flex', alignItems: 'center', mb: 2, gap: 2 }}>
        <Typography variant="h4" sx={{ flexGrow: 1 }}>Opportunities</Typography>
        <TextField size="small" placeholder="Search…" value={search}
          onChange={e => { setSearch(e.target.value); setPage(0); }}
          InputProps={{ startAdornment: <InputAdornment position="start"><SearchIcon /></InputAdornment> }} />
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => setFormOpen(true)}>
          Add Opportunity
        </Button>
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Name</TableCell>
              <TableCell>Stage</TableCell>
              <TableCell>Amount</TableCell>
              <TableCell>Probability</TableCell>
              <TableCell>Close Date</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {isLoading ? (
              <TableRow><TableCell colSpan={6} align="center"><CircularProgress /></TableCell></TableRow>
            ) : data?.items.map(o => (
              <TableRow key={o.id}>
                <TableCell>{o.name}</TableCell>
                <TableCell>
                  <Chip label={o.stage} color={stageColor[o.stage] ?? 'default'} size="small" />
                </TableCell>
                <TableCell>${o.amount.toLocaleString()}</TableCell>
                <TableCell>{o.probability}%</TableCell>
                <TableCell>{o.closeDate ? new Date(o.closeDate).toLocaleDateString() : '—'}</TableCell>
                <TableCell align="right">
                  <Tooltip title="Edit">
                    <IconButton size="small" onClick={() => setEditTarget(o)} aria-label="edit"><EditIcon fontSize="small" /></IconButton>
                  </Tooltip>
                  <Tooltip title="Delete">
                    <IconButton size="small" onClick={() => setDeleteTarget(o)} aria-label="delete"><DeleteIcon fontSize="small" /></IconButton>
                  </Tooltip>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      <TablePagination component="div" count={data?.total ?? 0} page={page} rowsPerPage={pageSize}
        onPageChange={(_, p) => setPage(p)}
        onRowsPerPageChange={e => { setPageSize(+e.target.value); setPage(0); }}
        rowsPerPageOptions={[10, 25, 50]} />

      <OpportunityFormDialog open={formOpen} onClose={() => setFormOpen(false)} onSubmit={handleCreate} title="Add Opportunity" />
      <OpportunityFormDialog open={!!editTarget} onClose={() => setEditTarget(null)} onSubmit={handleUpdate}
        defaultValues={editTarget ?? undefined} title="Edit Opportunity" />

      <Dialog open={!!deleteTarget} onClose={() => setDeleteTarget(null)}>
        <DialogTitle>Delete Opportunity</DialogTitle>
        <DialogContent>
          Are you sure you want to delete <strong>{deleteTarget?.name}</strong>?
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteTarget(null)}>Cancel</Button>
          <Button color="error" variant="contained" onClick={handleDelete} aria-label="confirm">Confirm</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
