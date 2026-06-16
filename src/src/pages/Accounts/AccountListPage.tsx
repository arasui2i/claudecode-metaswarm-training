import { useState } from 'react';
import {
  Box, Button, CircularProgress, Dialog, DialogActions, DialogContent, DialogTitle,
  IconButton, InputAdornment, Paper, Table, TableBody, TableCell, TableContainer,
  TableHead, TablePagination, TableRow, TextField, Tooltip, Typography,
} from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import SearchIcon from '@mui/icons-material/Search';
import AddIcon from '@mui/icons-material/Add';
import { useAccounts, useCreateAccount, useUpdateAccount, useDeleteAccount } from '../../hooks/useAccounts';
import AccountFormDialog from './AccountFormDialog';
import type { CreateAccountRequest, AccountSummary } from '../../api/accounts';

export default function AccountListPage() {
  const [search, setSearch] = useState('');
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [formOpen, setFormOpen] = useState(false);
  const [editTarget, setEditTarget] = useState<AccountSummary | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<AccountSummary | null>(null);

  const { data, isLoading } = useAccounts({ search, page: page + 1, pageSize });
  const createAccount = useCreateAccount();
  const updateAccount = useUpdateAccount();
  const deleteAccount = useDeleteAccount();

  const handleCreate = (body: CreateAccountRequest) =>
    createAccount.mutate(body, { onSuccess: () => setFormOpen(false) });

  const handleUpdate = (body: CreateAccountRequest) => {
    if (!editTarget) return;
    updateAccount.mutate({ id: editTarget.id, ...body }, { onSuccess: () => setEditTarget(null) });
  };

  const handleDelete = () => {
    if (!deleteTarget) return;
    deleteAccount.mutate(deleteTarget.id, { onSuccess: () => setDeleteTarget(null) });
  };

  return (
    <Box sx={{ p: 3 }}>
      <Box sx={{ display: 'flex', alignItems: 'center', mb: 2, gap: 2 }}>
        <Typography variant="h4" sx={{ flexGrow: 1 }}>Accounts</Typography>
        <TextField size="small" placeholder="Search…" value={search}
          onChange={e => { setSearch(e.target.value); setPage(0); }}
          InputProps={{ startAdornment: <InputAdornment position="start"><SearchIcon /></InputAdornment> }} />
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => setFormOpen(true)}>
          Add Account
        </Button>
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Name</TableCell>
              <TableCell>Type</TableCell>
              <TableCell>Industry</TableCell>
              <TableCell>Website</TableCell>
              <TableCell>Employees</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {isLoading ? (
              <TableRow><TableCell colSpan={6} align="center"><CircularProgress /></TableCell></TableRow>
            ) : data?.items.map(a => (
              <TableRow key={a.id}>
                <TableCell>{a.name}</TableCell>
                <TableCell>{a.accountType}</TableCell>
                <TableCell>{a.industry}</TableCell>
                <TableCell>{a.website}</TableCell>
                <TableCell>{a.employeeCount}</TableCell>
                <TableCell align="right">
                  <Tooltip title="Edit">
                    <IconButton size="small" onClick={() => setEditTarget(a)} aria-label="edit"><EditIcon fontSize="small" /></IconButton>
                  </Tooltip>
                  <Tooltip title="Delete">
                    <IconButton size="small" onClick={() => setDeleteTarget(a)} aria-label="delete"><DeleteIcon fontSize="small" /></IconButton>
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

      <AccountFormDialog open={formOpen} onClose={() => setFormOpen(false)} onSubmit={handleCreate} title="Add Account" />
      <AccountFormDialog open={!!editTarget} onClose={() => setEditTarget(null)} onSubmit={handleUpdate}
        defaultValues={editTarget ?? undefined} title="Edit Account" />

      <Dialog open={!!deleteTarget} onClose={() => setDeleteTarget(null)}>
        <DialogTitle>Delete Account</DialogTitle>
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
