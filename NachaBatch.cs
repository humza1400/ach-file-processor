using ach_prototype.Helpers;
using ach_prototype.Records;
using System.Text;

namespace ach_prototype
{
    /*
    * Helper class for a batch - includes header, entries (and addenda), and control.
    * This makes working with batches easier and separates batch concerns from file concerns.
    */
    public class NachaBatch
    {
        public BatchHeaderRecord BatchHeader { get; set; }

        public List<IEntryDetailRecord> Entries { get; set; } = new List<IEntryDetailRecord>();

        public BatchControlRecord BatchControl { get; private set; }

        // Calculated fields for FileControlRecord aggregation
        public int EntryAndAddendaCount { get; private set; } = 0;
        public long EntryHash { get; private set; } = 0;
        public decimal TotalDebitDollarAmount { get; private set; } = 0;
        public decimal TotalCreditDollarAmount { get; private set; } = 0;

        public NachaBatch(BatchHeaderRecord batchHeader)
        {
            BatchHeader = batchHeader;
        }

        public string Generate()
        {
            var sb = new StringBuilder();

            sb.AppendLine(BatchHeader.Generate());

            // Reset batch totals
            EntryAndAddendaCount = 0;
            EntryHash = 0;
            TotalDebitDollarAmount = 0;
            TotalCreditDollarAmount = 0;

            foreach (var entry in Entries)
            {
                if (!NachaHelper.IsValidTransactionCode(entry.TransactionCode))
                    throw new ArgumentException($"Invalid Transaction Code: {entry.TransactionCode}");

                sb.AppendLine(entry.Generate());

                EntryAndAddendaCount++; // 1 for entry detail record

                // Sum up routing number (first 8 digits of Receiving DFI Identification)
                EntryHash += long.Parse(entry.ReceivingDFIIdentification);

                // Accumulate dollar amounts based on transaction code
                if (NachaHelper.IsDebit(entry.TransactionCode))
                    TotalDebitDollarAmount += entry.GetAmount();
                else if (NachaHelper.IsCredit(entry.TransactionCode))
                    TotalCreditDollarAmount += entry.GetAmount();
                else throw new Exception("Invalid transaction code: " + entry.TransactionCode);



                // Handle AddendaRecord if present
                if (entry.AddendaRecord != null)
                {
                    sb.AppendLine(entry.AddendaRecord.Generate());
                    EntryAndAddendaCount++; // count addenda record too
                }
            }

            // Create Batch Control Record
            BatchControl = new BatchControlRecord(
                serviceClassCode: BatchHeader.ServiceClassCode,
                entryAndAddendaCount: EntryAndAddendaCount,
                entryHash: EntryHash,
                totalDebitDollarAmount: TotalDebitDollarAmount,
                totalCreditDollarAmount: TotalCreditDollarAmount,
                companyIdentification: BatchHeader.CompanyIdentification,
                originatingDFIIdentification: BatchHeader.OriginatingDFIIdentification,
                batchNumber: BatchHeader.BatchNumber
            );

            sb.AppendLine(BatchControl.Generate());

            return sb.ToString();
        }
    }

}
