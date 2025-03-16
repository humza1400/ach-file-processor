using System.Text;

namespace ach_prototype.Records
{
    public class FileControlRecord
    {
        // FIELD 1: Record Type Code
        // Position: 01-01 | Length: 1 | Required: Yes | Content: "9"
        // Always "9"
        private const string RecordTypeCode = "9";

        // FIELD 2: Batch Count
        // Position: 02-07 | Length: 6 | Required: Yes | Content: numeric
        // Count of the number of batches within the file
        public int BatchCount { get; private set; }

        // FIELD 3: Block Count
        // Position: 08-13 | Length: 6 | Required: Yes | Content: numeric
        // Count of the number of blocks of 10 rows within the file
        public int BlockCount { get; private set; }

        // FIELD 4: Entry/Addenda Count
        // Position: 14-21 | Length: 8 | Required: Yes | Content: numeric
        // Total count of the number of entries and addenda records within the file
        public int EntryAndAddendaCount { get; private set; }

        // FIELD 5: Entry Hash
        // Position: 22-31 | Length: 10 | Required: Yes | Content: numeric
        // The sum of the Entry Hash fields contained within the Batch Control Records of the file
        public long EntryHash { get; private set; }

        // FIELD 6: Total Debit Entry Dollar Amount in File
        // Position: 32-43 | Length: 12 | Required: Yes | Content: $$$$$$$$$$cc
        // Sum of the amount of debit entries contained within the file (in cents)
        public decimal TotalDebitDollarAmount { get; private set; }

        // FIELD 7: Total Credit Entry Dollar Amount in File
        // Position: 44-55 | Length: 12 | Required: Yes | Content: $$$$$$$$$$cc
        // Sum of the amount of credit entries contained within the file (in cents)
        public decimal TotalCreditDollarAmount { get; private set; }

        // FIELD 8: Reserved
        // Position: 56-94 | Length: 39 | Required: No | Content: blank
        // Reserved for future use, typically blank
        private const string Reserved = "";

        public FileControlRecord(
            int batchCount,                    // Field 2: Batch Count
            int blockCount,                    // Field 3: Block Count
            int entryAndAddendaCount,          // Field 4: Entry/Addenda Count
            long entryHash,                    // Field 5: Entry Hash
            decimal totalDebitDollarAmount,    // Field 6: Total Debit Entry Dollar Amount
            decimal totalCreditDollarAmount    // Field 7: Total Credit Entry Dollar Amount
        )
        {
            BatchCount = batchCount;
            BlockCount = blockCount;
            EntryAndAddendaCount = entryAndAddendaCount;
            EntryHash = entryHash;
            TotalDebitDollarAmount = totalDebitDollarAmount;
            TotalCreditDollarAmount = totalCreditDollarAmount;
        }

        // Generate the NACHA-formatted 94-character File Control Record line
        public string Generate()
        {
            var record = new StringBuilder();

            record.Append(RecordTypeCode);                                                      // Field 1: Record Type Code | Length: 1 | Always "9"
            record.Append(BatchCount.ToString().PadLeft(6, '0'));                               // Field 2: Batch Count | Length: 6
            record.Append(BlockCount.ToString().PadLeft(6, '0'));                               // Field 3: Block Count | Length: 6
            record.Append(EntryAndAddendaCount.ToString().PadLeft(8, '0'));                     // Field 4: Entry/Addenda Count | Length: 8
            record.Append(EntryHash.ToString().PadLeft(10, '0'));                               // Field 5: Entry Hash | Length: 10
            record.Append(((long)(TotalDebitDollarAmount * 100)).ToString().PadLeft(12, '0'));  // Field 6: Total Debit Dollar Amount | Length: 12 | $$$$$$$$$$cc
            record.Append(((long)(TotalCreditDollarAmount * 100)).ToString().PadLeft(12, '0')); // Field 7: Total Credit Dollar Amount | Length: 12 | $$$$$$$$$$cc
            record.Append(Reserved.PadRight(39));                                               // Field 8: Reserved | Length: 39

            return record.ToString();                                                           // Returns the full 94-character File Control Record line
        }
    }
}
