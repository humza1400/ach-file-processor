using ach_prototype.Helpers;
using System.Text;

namespace ach_prototype.Records
{
    public class WEBEntryDetailRecord : IEntryDetailRecord
    {
        // FIELD 1: Record Type Code
        // Position: 01-01 | Length: 1 | Required: Yes | Content: "6"
        // Always "6."
        private const string RecordTypeCode = "6";

        // FIELD 2: Transaction Code
        // Position: 02-03 | Length: 2 | Required: Yes | Content: Numeric
        // Tran code identifies transaction type (e.g., 22 = credit to checking, 27 = debit to checking).
        public string TransactionCode { get; private set; }

        // FIELD 3: Receiving DFI Identification
        // Position: 04-11 | Length: 8 | Required: Yes | Content: TTTTAAAA
        // First 8 digits of the RDFI's routing number.
        public string ReceivingDFIIdentification { get; private set; }

        // FIELD 4: Check Digit
        // Position: 12-12 | Length: 1 | Required: Yes | Content: Numeric
        // The 9th digit of the RDFI routing number (Check Digit).
        public string CheckDigit { get; private set; }

        // FIELD 5: DFI Account Number
        // Position: 13-29 | Length: 17 | Required: Yes | Content: Alphanumeric
        // Receiver's account number at the RDFI.
        public string DFIAccountNumber { get; private set; }

        // FIELD 6: Amount
        // Position: 30-39 | Length: 10 | Required: Yes | Content: $$$$$$$$cc
        // Transaction amount in cents (decimal not stored).
        public decimal Amount { get; private set; }

        // FIELD 7: Individual Identification Number
        // Position: 40-54 | Length: 15 | Required: No | Content: Alphanumeric
        // For Person-to-Person WEB credit Entries, this field is required (consumer Originator's name).
        // Optional for WEB debit Entries.
        public string IndividualIdentificationNumber { get; private set; }

        // FIELD 8: Individual Name
        // Position: 55-76 | Length: 22 | Required: Yes | Content: Alphanumeric
        // Entered by the Originator to provide additional identification of the Receiver.
        public string IndividualName { get; private set; }

        // FIELD 9: Payment Type Code
        // Position: 77-78 | Length: 2 | Required: Yes | Content: Alphanumeric
        // "R" for Recurring, "S" for Single-Entry, or custom codes (no standardized interpretation).
        public string PaymentTypeCode { get; private set; }

        // FIELD 10: Addenda Record Indicator
        // Position: 79-79 | Length: 1 | Required: Yes | Content: Numeric
        // "0" if no Addenda Record, "1" if Addenda Record is attached (WEB allows one optional addenda).
        public string AddendaRecordIndicator { get; private set; }

        // FIELD 11: Trace Number
        // Position: 80-94 | Length: 15 | Required: Yes | Content: Numeric
        // Unique number assigned by the ODFI (usually ODFI Routing Number + Sequence Number).
        public string TraceNumber { get; private set; }

        // Optional Addenda Record (nullable)
        public AddendaRecord AddendaRecord { get; private set; }

        public WEBEntryDetailRecord(
            string transactionCode,                        // Field 2
            string receivingDFIIdentification,             // Field 3
            string checkDigit,                             // Field 4
            string dfiAccountNumber,                       // Field 5
            decimal amount,                                // Field 6
            string individualName,                         // Field 8
            string paymentTypeCode,                        // Field 9
            string traceNumber,                            // Field 11
            string individualIdentificationNumber = "",    // Field 7 (optional)
            string addendaRecordIndicator = "0",           // Field 10 ("0" or "1")
            AddendaRecord addendaRecord = null             // Optional AddendaRecord
        )
        {
            TransactionCode = NachaHelper.PadLeft(transactionCode, 2);                                   // Field 2: Always 2 digits
            ReceivingDFIIdentification = NachaHelper.PadLeft(receivingDFIIdentification, 8);             // Field 3: Always 8 digits
            CheckDigit = NachaHelper.CalculateCheckDigit(ReceivingDFIIdentification).ToString();         // Field 4: 1 digit
            DFIAccountNumber = NachaHelper.PadRight(dfiAccountNumber, 17);                               // Field 5: Always 17 characters
            Amount = amount;                                                                             // Field 6: Decimal value (will be converted to cents in Generate)
            IndividualIdentificationNumber = NachaHelper.PadRight(individualIdentificationNumber, 15);   // Field 7: Always 15 characters
            IndividualName = NachaHelper.PadRight(individualName, 22);                                   // Field 8: Always 22 characters
            PaymentTypeCode = NachaHelper.FormatPaymentTypeCode(paymentTypeCode);                        // Field 9: Always 2 characters
            AddendaRecord = addendaRecord;                                                               // Optional AddendaRecord
            AddendaRecordIndicator = (addendaRecord != null) ? "1" : "0";                                // Field 10: "0" or "1"
            TraceNumber = NachaHelper.PadLeft(traceNumber, 15);                                          // Field 11: Always 15 digits
        }

        // Generate the NACHA-formatted 94-character WEB Entry Detail Record line
        public string Generate()
        {
            var record = new StringBuilder();

            record.Append(RecordTypeCode);                                                   // Field 1: Record Type Code | Length: 1 | Always "6"
            record.Append(TransactionCode);                                                  // Field 2: Transaction Code | Length: 2
            record.Append(ReceivingDFIIdentification);                                       // Field 3: Receiving DFI Identification | Length: 8
            record.Append(CheckDigit);                                                       // Field 4: Check Digit | Length: 1
            record.Append(DFIAccountNumber);                                                 // Field 5: DFI Account Number | Length: 17
            record.Append(NachaHelper.FormatAmount(Amount));                                 // Field 6: Amount | Length: 10 (no decimal point)
            record.Append(IndividualIdentificationNumber);                                   // Field 7: Individual Identification Number | Length: 15
            record.Append(IndividualName);                                                   // Field 8: Individual Name | Length: 22
            record.Append(PaymentTypeCode);                                                  // Field 9: Payment Type Code | Length: 2
            record.Append(AddendaRecordIndicator);                                           // Field 10: Addenda Record Indicator | Length: 1
            record.Append(TraceNumber);                                                      // Field 11: Trace Number | Length: 15

            return record.ToString();                                                        // Returns the full 94-character WEB Entry Detail Record line
        }

        public decimal GetAmount()
        {
            return Amount;
        }
    }
}
