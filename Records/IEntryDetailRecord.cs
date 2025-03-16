namespace ach_prototype.Records
{
    /*
     * Interface for EntryDetailRecords so we can support multiple types (PPD, CCD, WEB, TEL)
     */
    public interface IEntryDetailRecord
    {
        string TransactionCode { get; }
        string ReceivingDFIIdentification { get; }
        decimal Amount { get; }

        AddendaRecord AddendaRecord { get; }

        string Generate();
        decimal GetAmount();
    }
}
