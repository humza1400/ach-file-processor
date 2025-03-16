using ach_prototype.Records;
using System.Text;

namespace ach_prototype
{
    /*
     * An ACH file is a fixed-width, ASCII file, with each line exactly 94 characters in length.
     * Each line of characters is known as a “record” and is comprised of various “fields” that are at specific positions within that line.
     * In a properly formatted file, records must follow a specific order.
     *
     * ACH files contain one or more batches. Batches consist of one or more transactions.
     * Certain data elements are captured at different levels within the ACH format (file, batch or transaction).
     * As such, certain transactions will be batched together – where the Standard Entry Class (SEC) Code,
     * effective entry date, company ID, and batch descriptor are identical, based on similar information required at the element level.
     */

    public class NachaFile
    {
        // One file header per ACH file
        public FileHeaderRecord FileHeader { get; set; }

        // You can have multiple batches
        public List<NachaBatch> Batches { get; set; } = new List<NachaBatch>();

        // One file control record per ACH file
        public FileControlRecord FileControl { get; set; }

        public NachaFile(FileHeaderRecord fileHeader)
        {
            FileHeader = fileHeader;
        }

        // Generate full NACHA file contents
        public string Generate()
        {
            var sb = new StringBuilder();

            // File Header Record
            sb.AppendLine(FileHeader.Generate());

            int batchCount = 0;
            int entryAndAddendaCount = 0;
            long entryHash = 0;
            decimal totalDebit = 0;
            decimal totalCredit = 0;

            foreach (var batch in Batches)
            {
                batchCount++;

                // Generate Batch (Header + Entries + Control)
                var batchData = batch.Generate();

                // Accumulate values for FileControlRecord
                entryAndAddendaCount += batch.EntryAndAddendaCount;
                entryHash += batch.EntryHash;
                totalDebit += batch.TotalDebitDollarAmount;
                totalCredit += batch.TotalCreditDollarAmount;

                sb.Append(batchData);
            }

            // Build the FileControlRecord based on batch data
            FileControl = new FileControlRecord(
                batchCount: batchCount,
                blockCount: CalculateBlockCount(batchCount, entryAndAddendaCount),
                entryAndAddendaCount: entryAndAddendaCount,
                entryHash: entryHash,
                totalDebitDollarAmount: totalDebit,
                totalCreditDollarAmount: totalCredit
            );

            sb.AppendLine(FileControl.Generate());

            return sb.ToString();
        }

        // NACHA files must be in multiples of 10 records (Blocking Factor)
        private int CalculateBlockCount(int batchCount, int entryAndAddendaCount)
        {
            int totalRecords = 1                              // FileHeader
                             + (batchCount * 2)               // Each BatchHeader + BatchControl
                             + entryAndAddendaCount           // Entry + Addenda records
                             + 1;                             // FileControl

            int blockCount = (int)Math.Ceiling(totalRecords / 10.0);
            return blockCount;
        }
    }
}
