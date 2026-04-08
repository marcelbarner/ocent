import type { OwnedEntity, FieldProvenanceEntry } from '../shared/models';

export type AccountType = 'Bank' | 'P2P' | 'Cash' | 'Virtual' | 'Depot' | 'PensionInsurance';
export type AccountStatus = 'Active' | 'Closed' | 'Archived';
export type TransactionStatus = 'Raw' | 'Categorized' | 'Reconciled' | 'Disputed' | 'Archived';
export type RecurringFrequency = 'Daily' | 'Weekly' | 'Biweekly' | 'Monthly' | 'Quarterly' | 'Semiannual' | 'Annual' | 'Irregular';

export interface FinanceAccount extends OwnedEntity {
  accountType: AccountType;
  name: string;
  currency: string;
  currentBalance: number;
  balanceAsOf: string;
  institutionName: string | null;
  iban: string | null;
  accountNumber: string | null;
  notes: string | null;
  status: AccountStatus;
  sourceId: string | null;
  metadata: Record<string, unknown> | null;
}

export interface Transaction extends OwnedEntity {
  accountId: string;
  bookingDate: string;
  valueDate: string | null;
  amount: number;
  currency: string;
  description?: string;
  externalId?: string;
  counterpartName: string | null;
  counterpartIban: string | null;
  categoryId: string | null;
  isSplit: boolean;
  recurringPatternId: string | null;
  status: TransactionStatus;
  sourceId: string | null;
  notes: string | null;
}

export interface TransactionSplit {
  id: string;
  householdId: string;
  transactionId: string;
  amount: number;
  currency: string;
  categoryId: string | null;
  description: string | null;
  notes: string | null;
  fieldProvenance: Record<string, FieldProvenanceEntry> | null;
}

export interface TransactionCategory {
  id: string;
  householdId: string;
  parentId: string | null;
  name: string;
  isIncome: boolean;
  isTaxRelevant: boolean;
  colorHex: string | null;
  createdAt: string;
}

export interface RecurringPattern {
  id: string;
  householdId: string;
  createdByUserId: string;
  financeAccountId: string;
  name: string;
  expectedAmount: number | null;
  isActive: boolean;
  currency: string;
  frequency: RecurringFrequency;
  dayOfMonth: number | null;
  categoryId: string | null;
  linkedContractVersionId: string | null;
  activeFrom: string;
  activeTo: string | null;
}
