namespace ocent.Backend.WebApi.Domain.Shared;

public enum SourceKind
{
  ManualEntry,
  CsvImport,
  BulkUpload,
  OCR,
  AIAgent,
}
