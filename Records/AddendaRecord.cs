using System.Text;

namespace ach_prototype.Records
{
    // Applicable to PPD, CCD, and WEB entries
    public class AddendaRecord
    {
        // FIELD 1: Record Type Code
        // Position: 01-01 | Length: 1 | Required: Yes | Content: "7"
        // Always "7."
        private const string RecordTypeCode = "7";

        // FIELD 2: Addenda Type Code
        // Position: 02-03 | Length: 2 | Required: Yes | Content: "05"
        // Always "05" for PPD, CCD, WEB entries.
        private const string AddendaTypeCode = "05";

        // FIELD 3: Payment Related Information
        // Position: 04-83 | Length: 80 | Required: No | Content: Alphanumeric
        // Optional freeform text field that travels with the payment instruction to the RDFI.
        // For Health Care EFT Transactions, must contain ASC X12 Version 5010 835 TRN Segment.
        public string PaymentRelatedInformation { get; private set; }

        // FIELD 4: Addenda Sequence Number
        // Position: 84-87 | Length: 4 | Required: Yes | Content: Numeric
        // Always "1" for PPD, CCD, and WEB, because only one addenda is allowed.
        public const string AddendaSequenceNumber = "0001";

        // FIELD 5: Entry Detail Sequence Number
        // Position: 88-94 | Length: 7 | Required: Yes | Content: Numeric
        // Must match the last 7 digits of the trace number from the related Entry Detail Record.
        public string EntryDetailSequenceNumber { get; private set; }

        public AddendaRecord(
            string paymentRelatedInformation,        // Field 3: Payment Related Information (max 80 chars)
            string entryDetailSequenceNumber         // Field 5: Last 7 digits of Trace Number
        )
        {
            PaymentRelatedInformation = paymentRelatedInformation.PadRight(80);         // Field 3: Always 80 characters, right-padded
            EntryDetailSequenceNumber = entryDetailSequenceNumber.PadLeft(7, '0');      // Field 5: Always 7 digits, left-padded with zeros
        }

        // Generate the NACHA-formatted 94-character Addenda Record line
        public string Generate()
        {
            var record = new StringBuilder();

            record.Append(RecordTypeCode);              // Field 1: Record Type Code | Length: 1 | Always "7"
            record.Append(AddendaTypeCode);             // Field 2: Addenda Type Code | Length: 2 | Always "05"
            record.Append(PaymentRelatedInformation);   // Field 3: Payment Related Information | Length: 80
            record.Append(AddendaSequenceNumber);       // Field 4: Addenda Sequence Number | Length: 4 | Always "0001"
            record.Append(EntryDetailSequenceNumber);   // Field 5: Entry Detail Sequence Number | Length: 7

            return record.ToString();                   // Returns the full 94-character Addenda Record line
        }
    }
}
