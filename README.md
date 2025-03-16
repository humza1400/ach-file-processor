# ACH File Processor

A C# library for generating NACHA ACH files with support for multiple SEC codes (PPD, CCD, WEB, TEL) and batch types.

## Features

- Generate NACHA-compliant ACH files
- Validate ACH Files (planned, not implemented)
- Extensive Documentation on how to structure ACH Files
- Support for PPD, CCD, WEB, and TEL entry types (more planned)
- Routing number check digit validation (planned)
- Debit and credit transaction support
- Automatic batch total and entry count calculations
- File header, batch header, and control record generation
- Utility methods for padding, formatting, and check digit calculation
- Easy-to-read and extendable codebase

## Use Cases

- Learn how ACH files are constructed
- Payroll Direct Deposits (PPD)
- Vendor Payments (CCD)
- Online Payments (WEB)
- Telephone Payments (TEL)

## Getting Started

### Prerequisites

- .NET 6.0 SDK or later
- Visual Studio or Visual Studio Code (recommended)

### Installation

1. Clone the repository:
    ```bash
    git clone https://github.com/humza1400/ach-file-processor.git
    ```

2. Open the solution in Visual Studio or VS Code.

3. Build the solution:
    ```bash
    dotnet build
    ```

### Usage Example

Here’s a quick example of how to generate a simple ACH file:

```csharp
using ach_prototype.Records;

// Create File Header
var fileHeader = new FileHeaderRecord(
    immediateDestination: " 091000019",     // Example routing number with space prefix
    immediateOrigin: " 987654320",          // Example routing number with space prefix
    immediateDestinationName: "DEST BANK",
    immediateOriginName: "YOUR COMPANY"
);

// Initialize the NACHA File
var nachaFile = new NachaFile(fileHeader);

// Create a PPD Batch
var ppdBatchHeader = new BatchHeaderRecord(
    serviceClassCode: "220",
    companyName: "YOUR COMPANY",
    companyIdentification: "9876543210",
    standardEntryClassCode: "PPD",
    companyEntryDescription: "PAYROLL",
    effectiveEntryDate: DateTime.Now.AddDays(1).ToString("yyMMdd"),
    originatingDFIIdentification: "09100001",
    batchNumber: "1"
);

var ppdBatch = new NachaBatch(ppdBatchHeader);

ppdBatch.Entries.Add(new PPDEntryDetailRecord(
    transactionCode: "22",                         // Credit to checking
    receivingDFIIdentification: "87654321",
    checkDigit: "0",
    dfiAccountNumber: "123456789",
    amount: 100.00m,
    receivingIndividualName: "JOHN DOE",
    traceNumber: "091000010000001"
));

nachaFile.Batches.Add(ppdBatch);

// Generate the file content
var nachaFileContent = nachaFile.Generate();

// Save to file
File.WriteAllText("nacha-output.ach", nachaFileContent);

Console.WriteLine("ACH file generated successfully!");
