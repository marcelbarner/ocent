import type { OwnedEntity } from '../shared/models';

export type DocumentStatus = 'Uploaded' | 'Processing' | 'Enriched' | 'Archived';

export interface Document extends OwnedEntity {
  name: string;
  documentType: string;
  issueDate: string | null;
  issuer: string | null;
  status: DocumentStatus;
  sourceId: string | null;
}

export interface DocumentVersion {
  id: string;
  documentId: string;
  versionNumber: number;
  changeDescription: string | null;
  createdByUserId: string;
  createdAt: string;
}

export interface DocumentFile {
  id: string;
  documentVersionId: string;
  storagePath: string;
  mimeType: string;
  fileSizeBytes: number;
  checksumSha256: string;
  createdAt: string;
}
