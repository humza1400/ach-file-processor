using System.Text;

namespace ach_prototype.Records
{
    public class FileHeaderRecord
    {
        // FIELD 1: Record Type Code
        // Position: 01-01 | Length: 1 | Required: Yes | Content: "1"
        // Always "1"
        private const string RecordTypeCode = "1";

        // FIELD 2: Priority Code
        // Position: 02-03 | Length: 2 | Required: Yes | Content: "01"
        // Always "01"
        public const string PriorityCode = "01";

        // FIELD 3: Immediate Destination
        // Position: 04-13 | Length: 10 | Required: Yes | Content: Bnnnnnnnn
        // The nine-digit routing number of the institution receiving the ACH file for processing, preceded by a blank. Typically, this is your bank’s routing and transit number.
        public string ImmediateDestination { get; private set; }

        // FIELD 4: Immediate Origin
        // Position: 14-23 | Length: 10 | Required: Yes | Content: Bnnnnnnnn
        // The nine-digit routing transit number of the institution sending (originating) the ACH file, preceded by a blank. (Often your ODFI will have you insert your company ID in this field.)
        public string ImmediateOrigin { get; private set; }

        // FIELD 5: File Creation Date
        // Position: 24-29 | Length: 6 | Required: Yes | Content: YYMMDD
        // The date that the ACH file was created.
        public string FileCreationDate { get; private set; }

        // FIELD 6: File Creation Time
        // Position: 30-33 | Length: 4 | Required: No | Content: HHMM
        // The time the ACH file was created (24-hour format).
        public string FileCreationTime { get; private set; }

        // FIELD 7: File ID Modifier
        // Position: 34-34 | Length: 1 | Required: Yes | Content: A-Z, 0-9
        // For a single processing day, each file submitted by the financial institution should have a unique ID to allow for thorough duplicate file identification.
        public string FileIDModifier { get; private set; }

        // FIELD 8: Record Size
        // Position: 35-37 | Length: 3 | Required: Yes | Content: "094"
        // Always "094" because every record contains 94 characters.
        public const string RecordSize = "094";

        // FIELD 9: Blocking Factor
        // Position: 38-39 | Length: 2 | Required: Yes | Content: "10"
        // Always "10" because the blocking factor is 10.
        public const string BlockingFactor = "10";

        // FIELD 10: Format Code
        // Position: 40-40 | Length: 1 | Required: Yes | Content: "1"
        // Always "1"
        public const string FormatCode = "1";

        // FIELD 11: Immediate Destination Name
        // Position: 41-63 | Length: 23 | Required: No | Content: Alphanumeric
        // Name of the financial institution receiving the payment file.
        public string ImmediateDestinationName { get; private set; }

        // FIELD 12: Immediate Origin Name
        // Position: 64-86 | Length: 23 | Required: No | Content: Alphanumeric
        // Name of the financial institution sending the payment file.
        public string ImmediateOriginName { get; private set; }

        // FIELD 13: Reference Code
        // Position: 87-94 | Length: 8 | Required: No | Content: Alphanumeric
        // Reserved for information pertinent to the business.
        public string ReferenceCode { get; private set; }

        public FileHeaderRecord(
            string immediateDestination,         // Field 3: Immediate Destination (Routing Number + space prefix)
            string immediateOrigin,              // Field 4: Immediate Origin (Routing Number + space prefix)
            string immediateDestinationName,     // Field 11: Immediate Destination Name
            string immediateOriginName,          // Field 12: Immediate Origin Name
            string fileIDModifier = "A"          // Field 7: File ID Modifier (A-Z)
        )
        {
            ImmediateDestination = immediateDestination;              // Receiving routing number (Bnnnnnnnn)
            ImmediateOrigin = immediateOrigin;                        // Originating routing number (Bnnnnnnnn)
            FileCreationDate = DateTime.Now.ToString("yyMMdd");       // Current date (YYMMDD)
            FileCreationTime = DateTime.Now.ToString("HHmm");         // Current time (HHMM)
            FileIDModifier = fileIDModifier;                          // Unique for each file that day (A-Z)
            ImmediateDestinationName = immediateDestinationName;      // Receiving institution name
            ImmediateOriginName = immediateOriginName;                // Originating institution name
            ReferenceCode = "";                                       // Optional, typically blank
        }

        // Generate the formatted 94-character File Header Record line
        public string Generate()
        {
            var record = new StringBuilder();

            record.Append(RecordTypeCode);                                      // Field 1: Record Type Code | Length: 1
            record.Append(PriorityCode);                                        // Field 2: Priority Code | Length: 2
            record.Append(ImmediateDestination.PadLeft(10));                    // Field 3: Immediate Destination | Length: 10
            record.Append(ImmediateOrigin.PadLeft(10));                         // Field 4: Immediate Origin | Length: 10
            record.Append(FileCreationDate);                                    // Field 5: File Creation Date | Length: 6
            record.Append(FileCreationTime);                                    // Field 6: File Creation Time | Length: 4
            record.Append(FileIDModifier);                                      // Field 7: File ID Modifier | Length: 1
            record.Append(RecordSize);                                          // Field 8: Record Size | Length: 3
            record.Append(BlockingFactor);                                      // Field 9: Blocking Factor | Length: 2
            record.Append(FormatCode);                                          // Field 10: Format Code | Length: 1
            record.Append(ImmediateDestinationName.PadRight(23));               // Field 11: Immediate Destination Name | Length: 23
            record.Append(ImmediateOriginName.PadRight(23));                    // Field 12: Immediate Origin Name | Length: 23
            record.Append(ReferenceCode.PadRight(8));                           // Field 13: Reference Code | Length: 8

            return record.ToString();                                           // Returns the full 94-character File Header Record line
        }
    }
}
