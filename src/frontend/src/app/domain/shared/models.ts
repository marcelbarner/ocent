export type LifecycleStage = 'Raw' | 'Enriched' | 'Verified';
export type Visibility = 'Shared' | 'Private';
export type ProvenanceSource = 'Manual' | 'Imported' | 'OCR' | 'AI_Proposed' | 'Derived';
export type UserRole = 'Owner' | 'Member';
export type LinkDirection = 'Directed' | 'Bidirectional';
export type SourceKind = 'ManualEntry' | 'CsvImport' | 'BulkUpload' | 'OCR' | 'AIAgent';
export type ContainerKind = 'General' | 'TaxCase' | 'Event' | 'Case';
export type ContainerStatus = 'Active' | 'Closed' | 'Archived';

export interface FieldProvenanceEntry {
  source: ProvenanceSource;
  confidence: number | null;
  reviewed: boolean;
  reviewedByUserId: string | null;
  reviewedAt: string | null;
}

export interface OwnedEntity {
  id: string;
  householdId: string;
  createdByUserId: string;
  visibility: Visibility;
  writeRestricted: boolean;
  createdAt: string;
  updatedAt: string;
  lifecycleStage: LifecycleStage;
  fieldProvenance: Record<string, FieldProvenanceEntry> | null;
}

export interface Household {
  id: string;
  name: string;
  createdAt: string;
  updatedAt: string;
}

export interface User {
  id: string;
  householdId: string;
  email: string;
  displayName: string;
  passwordHash: string;
  totpSecret: string | null;
  role: UserRole;
  visibilityDefault: Visibility;
  createdAt: string;
  updatedAt: string;
  lastLoginAt: string | null;
  archivedAt: string | null;
}

export interface Container extends OwnedEntity {
  name: string;
  description: string | null;
  kind: ContainerKind;
  taxYear: number | null;
  status: ContainerStatus;
  metadata: Record<string, unknown> | null;
}

export interface ContainerMembership {
  id: string;
  containerId: string;
  entityType: string;
  entityId: string;
  assignedByUserId: string;
  assignedAt: string;
  note: string | null;
}

export interface Tag {
  id: string;
  householdId: string;
  name: string;
  colorHex: string | null;
  createdAt: string;
}

export interface EntityTag {
  id: string;
  tagId: string;
  entityType: string;
  entityId: string;
  taggedByUserId: string;
  taggedAt: string;
}

export interface Link {
  id: string;
  householdId: string;
  sourceEntityType: string;
  sourceEntityId: string;
  targetEntityType: string;
  targetEntityId: string;
  linkType: string;
  direction: LinkDirection;
  createdByUserId: string;
  createdAt: string;
  note: string | null;
  provenanceSource: ProvenanceSource;
}

export interface Source {
  id: string;
  householdId: string;
  kind: SourceKind;
  label: string;
  importedAt: string;
  importedByUserId: string;
  fileReference: string | null;
  metadata: Record<string, unknown> | null;
}
