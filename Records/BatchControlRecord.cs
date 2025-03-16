using System.Text;

namespace ach_prototype.Records
{
    public class BatchControlRecord
    {
        // FIELD 1: Record Type Code
        // Position: 01-01 | Length: 1 | Required: Yes | Content: "8"
        // Always "8."
        private const string RecordTypeCode = "8";

        // FIELD 2: Service Class Code
        // Position: 02-04 | Length: 3 | Required: Yes | Content: numeric
        // Identifies the general classification of dollar entries to be exchanged.
        public string ServiceClassCode { get; private set; }

        // FIELD 3: Entry/Addenda Count
        // Position: 05-10 | Length: 6 | Required: Yes | Content: numeric
        // A tally of each Entry Detail record and each Addenda Record processed within the batch.
        public int EntryAndAddendaCount { get; private set; }

        // FIELD 4: Entry Hash
        // Position: 11-20 | Length: 10 | Required: Yes | Content: numeric
        // The sum of all the Receiving DFI Identification fields contained within the Entry Detail Records in a batch.
        public long EntryHash { get; private set; }

        // FIELD 5: Total Debit Entry Dollar Amount
        // Position: 21-32 | Length: 12 | Required: Yes | Content: numeric
        // Contains the accumulated entry detail debit totals within the batch.
        public decimal TotalDebitDollarAmount { get; private set; }

        // FIELD 6: Total Credit Entry Dollar Amount
        // Position: 33-44 | Length: 12 | Required: Yes | Content: numeric
        // Contains the accumulated entry detail credit totals within the batch.
        public decimal TotalCreditDollarAmount { get; private set; }

        // FIELD 7: Company Identification
        // Position: 45-54 | Length: 10 | Required: Yes | Content: alphanumeric
        // Contains the value of the Company Identification field 5 in the Company/Batch Header record.
        public string CompanyIdentification { get; private set; }

        // FIELD 8: Message Authentication Code
        // Position: 55-73 | Length: 19 | Required: No | Content: alphanumeric
        // Optional field used for validation and authentication (leave blank if not used).
        public string MessageAuthenticationCode { get; private set; }

        // FIELD 9: Reserved
        // Position: 74-79 | Length: 6 | Required: No | Content: blanks
        // Reserved for future use, typically blank.
        private const string Reserved = "";

        // FIELD 10: Originating DFI Identification
        // Position: 80-87 | Length: 8 | Required: Yes | Content: numeric
        // Contains the value of the Originating DFI Identification field 12 of the Company/Batch Header record.
        public string OriginatingDFIIdentification { get; private set; }

        // FIELD 11: Batch Number
        // Position: 88-94 | Length: 7 | Required: Yes | Content: numeric
        // Contains the value of the Batch Number in field 13 of the Company/Batch Header Record.
        public string BatchNumber { get; private set; }

        public BatchControlRecord(
            string serviceClassCode,
            int entryAndAddendaCount,
            long entryHash,       
            decimal totalDebitDollarAmount,
            decimal totalCreditDollarAmount,
            string companyIdentification,
            string originatingDFIIdentification,
            string batchNumber,  
            string messageAuthenticationCode = ""      // Field 8 (optional)
        )
        {
            ServiceClassCode = serviceClassCode.PadLeft(3, '0');                        // Field 2: Always 3 digits
            EntryAndAddendaCount = entryAndAddendaCount;                                // Field 3
            EntryHash = entryHash;                                                      // Field 4
            TotalDebitDollarAmount = totalDebitDollarAmount;                            // Field 5
            TotalCreditDollarAmount = totalCreditDollarAmount;                          // Field 6
            CompanyIdentification = companyIdentification.PadRight(10);                 // Field 7: Always 10 characters
            MessageAuthenticationCode = messageAuthenticationCode.PadRight(19);         // Field 8: Always 19 characters (optional)
            OriginatingDFIIdentification = originatingDFIIdentification.PadRight(8);    // Field 10: Always 8 digits
            BatchNumber = batchNumber.PadLeft(7, '0');                                  // Field 11: Always 7 digits
        }

        // Generate the NACHA-formatted 94-character Batch Control Record line
        public string Generate()
        {
            var record = new StringBuilder();

            record.Append(RecordTypeCode);                                                      // Field 1: Record Type Code | Length: 1 | Always "8"
            record.Append(ServiceClassCode);                                                    // Field 2: Service Class Code | Length: 3
            record.Append(EntryAndAddendaCount.ToString().PadLeft(6, '0'));                     // Field 3: Entry/Addenda Count | Length: 6
            record.Append(EntryHash.ToString().PadLeft(10, '0'));                               // Field 4: Entry Hash | Length: 10
            record.Append(((long)(TotalDebitDollarAmount * 100)).ToString().PadLeft(12, '0'));  // Field 5: Total Debit Dollar Amount | Length: 12
            record.Append(((long)(TotalCreditDollarAmount * 100)).ToString().PadLeft(12, '0')); // Field 6: Total Credit Dollar Amount | Length: 12
            record.Append(CompanyIdentification);                                               // Field 7: Company Identification | Length: 10
            record.Append(MessageAuthenticationCode);                                           // Field 8: Message Authentication Code | Length: 19
            record.Append(Reserved.PadRight(6));                                                // Field 9: Reserved | Length: 6
            record.Append(OriginatingDFIIdentification);                                        // Field 10: Originating DFI Identification | Length: 8
            record.Append(BatchNumber);                                                         // Field 11: Batch Number | Length: 7

            return record.ToString();                                                           // Returns the full 94-character Batch Control Record line
        }
    }
}
