using System.Text;

namespace ach_prototype.Records
{
    public class BatchHeaderRecord
    {
        // FIELD 1: Record Type Code
        // Position: 01-01 | Length: 1 | Required: Yes | Content: "5"
        // Always "5"
        private const string RecordTypeCode = "5";

        // FIELD 2: Service Class Code
        // Position: 02-04 | Length: 3 | Required: Yes | Content: numeric
        // Identifies the general classification of dollar entries to be exchanged.
        public string ServiceClassCode { get; private set; }

        // FIELD 3: Company Name
        // Position: 05-20 | Length: 16 | Required: Yes | Content: alphanumeric
        // Name of the Originator known and recognized by the Receiver.
        public string CompanyName { get; private set; }

        // FIELD 4: Company Discretionary Data
        // Position: 21-40 | Length: 20 | Required: No | Content: alphanumeric
        // Originator/ODFI may include codes of significance only to them.
        public string CompanyDiscretionaryData { get; private set; }

        // FIELD 5: Company Identification
        // Position: 41-50 | Length: 10 | Required: Yes | Content: alphanumeric
        // Used to identify the Originator. Assigned by the ODFI.
        public string CompanyIdentification { get; private set; }

        // FIELD 6: Standard Entry Class Code
        // Position: 51-53 | Length: 3 | Required: Yes | Content: alphanumeric
        // Three-character code identifying the type of entries (PPD, CCD, etc.).
        public string StandardEntryClassCode { get; private set; }

        // FIELD 7: Company Entry Description
        // Position: 54-63 | Length: 10 | Required: Yes | Content: alphanumeric
        // Description of the purpose of the entry (e.g., "PAYROLL").
        public string CompanyEntryDescription { get; private set; }

        // FIELD 8: Company Descriptive Date
        // Position: 64-69 | Length: 6 | Required: No | Content: alphanumeric
        // Date displayed to the Receiver for descriptive purposes.
        public string CompanyDescriptiveDate { get; private set; }

        // FIELD 9: Effective Entry Date
        // Position: 70-75 | Length: 6 | Required: No | Content: YYMMDD
        // The banking day the Originator intends entries to be settled.
        public string EffectiveEntryDate { get; private set; }

        // FIELD 10: Settlement Date (Julian)
        // Position: 76-78 | Length: 3 | Populated by ACH Operator | Content: numeric
        // Inserted by the ACH Operator (leave blank when sending the file).
        public string SettlementDate { get; private set; }

        // FIELD 11: Originator Status Code
        // Position: 79-79 | Length: 1 | Required: Yes | Content: alphanumeric
        // Code referring to the ODFI initiating the entry. Always "1".
        private const string OriginatorStatusCode = "1";

        // FIELD 12: Originating DFI Identification
        // Position: 80-87 | Length: 8 | Required: Yes | Content: TTTTAAAA
        // The routing number of the DFI originating the entries (first 8 digits).
        public string OriginatingDFIIdentification { get; private set; }

        // FIELD 13: Batch Number
        // Position: 88-94 | Length: 7 | Required: Yes | Content: numeric
        // ODFI assigned ascending number for each batch in the file.
        public string BatchNumber { get; private set; }

        public BatchHeaderRecord(
            string serviceClassCode,
            string companyName,        
            string companyIdentification,
            string standardEntryClassCode,
            string companyEntryDescription,
            string effectiveEntryDate, 
            string originatingDFIIdentification,
            string batchNumber,
            string companyDiscretionaryData = "",
            string companyDescriptiveDate = null 
        )
        {
            ServiceClassCode = serviceClassCode.PadLeft(3, '0');                                    // Field 2: Always 3 digits
            CompanyName = companyName.PadRight(16);                                                 // Field 3: Always 16 characters
            CompanyIdentification = companyIdentification.PadRight(10);                             // Field 5: Always 10 characters
            StandardEntryClassCode = standardEntryClassCode.PadRight(3);                            // Field 6: Always 3 characters
            CompanyEntryDescription = companyEntryDescription.PadRight(10);                         // Field 7: Always 10 characters
            EffectiveEntryDate = effectiveEntryDate;                                                // Field 9: Always 6 characters (YYMMDD)
            OriginatingDFIIdentification = originatingDFIIdentification.PadRight(8);                // Field 12: Always 8 characters
            BatchNumber = batchNumber.PadLeft(7, '0');                                              // Field 13: Always 7 digits (right-justified)

            CompanyDiscretionaryData = companyDiscretionaryData.PadRight(20);                       // Field 4: Always 20 characters (optional)
            CompanyDescriptiveDate = (companyDescriptiveDate ?? DateTime.Now.ToString("MMddyy"));   // Field 8: Always 6 characters (optional)

            SettlementDate = "".PadRight(3);                                                        // Field 10: Always 3 characters, left blank for ACH Operator
        }

        // Generate the NACHA-formatted 94-character Batch Header Record line
        public string Generate()
        {
            var record = new StringBuilder();

            record.Append(RecordTypeCode);                                  // Field 1: Record Type Code | Length: 1 | Always "5"
            record.Append(ServiceClassCode);                                // Field 2: Service Class Code | Length: 3
            record.Append(CompanyName);                                     // Field 3: Company Name | Length: 16
            record.Append(CompanyDiscretionaryData);                        // Field 4: Company Discretionary Data | Length: 20
            record.Append(CompanyIdentification);                           // Field 5: Company Identification | Length: 10
            record.Append(StandardEntryClassCode);                          // Field 6: Standard Entry Class Code | Length: 3
            record.Append(CompanyEntryDescription);                         // Field 7: Company Entry Description | Length: 10
            record.Append(CompanyDescriptiveDate);                          // Field 8: Company Descriptive Date | Length: 6
            record.Append(EffectiveEntryDate);                              // Field 9: Effective Entry Date | Length: 6
            record.Append(SettlementDate);                                  // Field 10: Settlement Date | Length: 3 (blank)
            record.Append(OriginatorStatusCode);                            // Field 11: Originator Status Code | Length: 1
            record.Append(OriginatingDFIIdentification);                    // Field 12: Originating DFI Identification | Length: 8
            record.Append(BatchNumber);                                     // Field 13: Batch Number | Length: 7

            return record.ToString();                                       // Returns the full 94-character Batch Header Record line
        }
    }
}
