using ach_prototype.Records;

namespace ach_prototype
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Generating NACHA File with 4 Batches (PPD, CCD, WEB, TEL)...");

            // Create File Header
            var fileHeader = new FileHeaderRecord(
                immediateDestination: " 123456789",    // Add space for Bnnnnnnnn format
                immediateOrigin: " 987654321",         // Add space for Bnnnnnnnn format
                immediateDestinationName: "DEST BANK",
                immediateOriginName: "YOUR COMPANY"
            );

            // Initialize the NACHA File
            var nachaFile = new NachaFile(fileHeader);

            // Batch 1: PPD Entries
            var ppdBatchHeader = new BatchHeaderRecord(
                serviceClassCode: "220",
                companyName: "YOUR COMPANY",
                companyIdentification: "9876543210",
                standardEntryClassCode: "PPD",
                companyEntryDescription: "PAYROLL",
                effectiveEntryDate: DateTime.Now.AddDays(1).ToString("yyMMdd"),
                originatingDFIIdentification: "12345678",
                batchNumber: "1"
            );

            var ppdBatch = new NachaBatch(ppdBatchHeader);

            ppdBatch.Entries.Add(new PPDEntryDetailRecord(
                transactionCode: "22",                         // Credit Checking
                receivingDFIIdentification: "87654321",
                checkDigit: "0",
                dfiAccountNumber: "123456789",
                amount: 100.00m,
                receivingIndividualName: "JOHN DOE",
                traceNumber: "123456780000001"
            ));

            // Batch 2: CCD Entries
            var ccdBatchHeader = new BatchHeaderRecord(
                serviceClassCode: "225",
                companyName: "YOUR COMPANY",
                companyIdentification: "9876543210",
                standardEntryClassCode: "CCD",
                companyEntryDescription: "VENDOR PAY",
                effectiveEntryDate: DateTime.Now.AddDays(1).ToString("yyMMdd"),
                originatingDFIIdentification: "12345678",
                batchNumber: "2"
            );

            var ccdBatch = new NachaBatch(ccdBatchHeader);

            ccdBatch.Entries.Add(new CCDEntryDetailRecord(
                transactionCode: "27",                         // Debit Checking
                receivingDFIIdentification: "87654321",
                checkDigit: "0",
                dfiAccountNumber: "987654321",
                amount: 250.00m,
                receivingCompanyName: "ACME CORP",
                traceNumber: "123456780000002"
            ));

            // Batch 3: WEB Entries
            var webBatchHeader = new BatchHeaderRecord(
                serviceClassCode: "225",
                companyName: "YOUR COMPANY",
                companyIdentification: "9876543210",
                standardEntryClassCode: "WEB",
                companyEntryDescription: "ONLINE PAY",
                effectiveEntryDate: DateTime.Now.AddDays(1).ToString("yyMMdd"),
                originatingDFIIdentification: "12345678",
                batchNumber: "3"
            );

            var webBatch = new NachaBatch(webBatchHeader);

            webBatch.Entries.Add(new WEBEntryDetailRecord(
                transactionCode: "27",                         // Debit Checking
                receivingDFIIdentification: "87654321",
                checkDigit: "0",
                dfiAccountNumber: "111222333",
                amount: 75.00m,
                individualName: "JANE SMITH",
                paymentTypeCode: "",
                traceNumber: "123456780000003"
            ));

            // Batch 4: TEL Entries
            var telBatchHeader = new BatchHeaderRecord(
                serviceClassCode: "225",
                companyName: "YOUR COMPANY",
                companyIdentification: "9876543210",
                standardEntryClassCode: "TEL",
                companyEntryDescription: "PHONE PAY",
                effectiveEntryDate: DateTime.Now.AddDays(1).ToString("yyMMdd"),
                originatingDFIIdentification: "12345678",
                batchNumber: "4"
            );

            var telBatch = new NachaBatch(telBatchHeader);

            telBatch.Entries.Add(new TELEntryDetailRecord(
                transactionCode: "27",                         // Debit Checking
                receivingDFIIdentification: "87654321",
                checkDigit: "0",
                dfiAccountNumber: "222333444",
                amount: 50.00m,
                individualName: "BOB JOHNSON",
                paymentTypeCode: "",
                traceNumber: "123456780000004"
            ));

            // Add all batches to the file
            nachaFile.Batches.Add(ppdBatch);
            nachaFile.Batches.Add(ccdBatch);
            nachaFile.Batches.Add(webBatch);
            nachaFile.Batches.Add(telBatch);

            // Generate the NACHA file content
            var nachaFileContent = nachaFile.Generate();

            // Output to file
            var outputFilePath = "nacha-output.ach";

            File.WriteAllText(outputFilePath, nachaFileContent);

            Console.WriteLine($"NACHA File generated successfully: {outputFilePath}");
        }
    }
}
