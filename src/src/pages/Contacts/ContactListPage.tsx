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
import { useContacts, useCreateContact, useUpdateContact, useDeleteContact } from '../../hooks/useContacts';
import ContactFormDialog from './ContactFormDialog';
import type { CreateContactRequest, ContactSummary } from '../../api/contacts';

export default function ContactListPage() {
  const [search, setSearch] = useState('');
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [formOpen, setFormOpen] = useState(false);
  const [editTarget, setEditTarget] = useState<ContactSummary | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<ContactSummary | null>(null);

  const { data, isLoading } = useContacts({ search, page: page + 1, pageSize });
  const createContact = useCreateContact();
  const updateContact = useUpdateContact();
  const deleteContact = useDeleteContact();

  const handleCreate = (body: CreateContactRequest) =>
    createContact.mutate(body, { onSuccess: () => setFormOpen(false) });

  const handleUpdate = (body: CreateContactRequest) => {
    if (!editTarget) return;
    updateContact.mutate({ id: editTarget.id, ...body }, { onSuccess: () => setEditTarget(null) });
  };

  const handleDelete = () => {
    if (!deleteTarget) return;
    deleteContact.mutate(deleteTarget.id, { onSuccess: () => setDeleteTarget(null) });
  };

  return (
    <Box sx={{ p: 3 }}>
      <Box sx={{ display: 'flex', alignItems: 'center', mb: 2, gap: 2 }}>
        <Typography variant="h4" sx={{ flexGrow: 1 }}>Contacts</Typography>
        <TextField size="small" placeholder="Search…" value={search}
          onChange={e => { setSearch(e.target.value); setPage(0); }}
          InputProps={{ startAdornment: <InputAdornment position="start"><SearchIcon /></InputAdornment> }} />
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => setFormOpen(true)}>
          Add Contact
        </Button>
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Name</TableCell>
              <TableCell>Email</TableCell>
              <TableCell>Phone</TableCell>
              <TableCell>Job Title</TableCell>
              <TableCell>Type</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {isLoading ? (
              <TableRow><TableCell colSpan={6} align="center"><CircularProgress /></TableCell></TableRow>
            ) : data?.items.map(c => (
              <TableRow key={c.id}>
                <TableCell>{c.firstName} {c.lastName}</TableCell>
                <TableCell>{c.email}</TableCell>
                <TableCell>{c.phoneNumber}</TableCell>
                <TableCell>{c.jobTitle}</TableCell>
                <TableCell>{c.contactType}</TableCell>
                <TableCell align="right">
                  <Tooltip title="Edit">
                    <IconButton size="small" onClick={() => setEditTarget(c)} aria-label="edit"><EditIcon fontSize="small" /></IconButton>
                  </Tooltip>
                  <Tooltip title="Delete">
                    <IconButton size="small" onClick={() => setDeleteTarget(c)} aria-label="delete"><DeleteIcon fontSize="small" /></IconButton>
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

      <ContactFormDialog open={formOpen} onClose={() => setFormOpen(false)} onSubmit={handleCreate} title="Add Contact" />
      <ContactFormDialog open={!!editTarget} onClose={() => setEditTarget(null)} onSubmit={handleUpdate}
        defaultValues={editTarget ?? undefined} title="Edit Contact" />

      <Dialog open={!!deleteTarget} onClose={() => setDeleteTarget(null)}>
        <DialogTitle>Delete Contact</DialogTitle>
        <DialogContent>
          Are you sure you want to delete <strong>{deleteTarget?.firstName} {deleteTarget?.lastName}</strong>?
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteTarget(null)}>Cancel</Button>
          <Button color="error" variant="contained" onClick={handleDelete} aria-label="confirm">Confirm</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
