import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  getContacts,
  createContact,
  updateContact,
  deleteContact,
  type ContactsQuery,
  type CreateContactRequest,
  type UpdateContactRequest,
} from '../api/contacts';

const CONTACTS_KEY = 'contacts';

export const useContacts = (query: ContactsQuery) =>
  useQuery({
    queryKey: [CONTACTS_KEY, query],
    queryFn: () => getContacts(query),
  });

export const useCreateContact = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (body: CreateContactRequest) => createContact(body),
    onSuccess: () => qc.invalidateQueries({ queryKey: [CONTACTS_KEY] }),
  });
};

export const useUpdateContact = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (body: UpdateContactRequest) => updateContact(body),
    onSuccess: () => qc.invalidateQueries({ queryKey: [CONTACTS_KEY] }),
  });
};

export const useDeleteContact = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => deleteContact(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: [CONTACTS_KEY] }),
  });
};
