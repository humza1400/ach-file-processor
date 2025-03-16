using System.ComponentModel;

namespace ach_prototype.Helpers
{
    public enum NachaTransactionType
    {
        [Description("Credit")]
        Credit,
        [Description("Debit")]
        Debit,
        [Description("Unknown")]
        Unknown
    }

    public static class NachaHelper
    {
        public static bool IsDebit(string transactionCode)
        {
            if (string.IsNullOrWhiteSpace(transactionCode) || transactionCode.Length != 2)
                throw new ArgumentException("Transaction code must be a 2-character string.", nameof(transactionCode));

            char firstDigit = transactionCode[0];
            char secondDigit = transactionCode[1];

            return (firstDigit == '2' || firstDigit == '3') &&
                   (secondDigit == '7' || secondDigit == '8' || secondDigit == '9');
        }

        public static bool IsCredit(string transactionCode)
        {
            if (string.IsNullOrWhiteSpace(transactionCode) || transactionCode.Length != 2)
                throw new ArgumentException("Transaction code must be a 2-character string.", nameof(transactionCode));

            char firstDigit = transactionCode[0];
            char secondDigit = transactionCode[1];

            return (firstDigit == '2' || firstDigit == '3') &&
                   (secondDigit == '2' || secondDigit == '3' || secondDigit == '4');
        }

        public static NachaTransactionType GetTransactionType(string transactionCode)
        {
            if (IsCredit(transactionCode)) return NachaTransactionType.Credit;
            if (IsDebit(transactionCode)) return NachaTransactionType.Debit;
            return NachaTransactionType.Unknown;
        }

        public static bool IsValidTransactionCode(string transactionCode)
        {
            return IsCredit(transactionCode) || IsDebit(transactionCode);
        }

        // Calculates the check digit for a routing number (first 8 digits)
        public static int CalculateCheckDigit(string routing)
        {
            if (routing.Length != 8)
                throw new ArgumentException("Routing number must be exactly 8 digits.");

            int sum = 0;
            int[] weights = { 3, 7, 1, 3, 7, 1, 3, 7 };
            for (int i = 0; i < 8; i++)
            {
                sum += int.Parse(routing[i].ToString()) * weights[i];
            }

            return (10 - (sum % 10)) % 10;
        }

        // Pads a field to the right
        public static string PadRight(string input, int length)
        {
            return (input ?? "").PadRight(length);
        }

        // Pads a field to the left with optional padding character (default '0')
        public static string PadLeft(string input, int length, char paddingChar = '0')
        {
            return (input ?? "").PadLeft(length, paddingChar);
        }

        // Converts an amount to cents, formatted to 10 digits with leading zeroes
        public static string FormatAmount(decimal amount)
        {
            return ((long)(amount * 100)).ToString().PadLeft(10, '0');
        }

        public static string FormatPaymentTypeCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return "  "; // Default to 2 spaces

            code = code.Trim().ToUpper();

            if (code != "R" && code != "S")
                throw new ArgumentException($"Invalid PaymentTypeCode: {code}. Must be 'R' or 'S'.");

            return code.PadRight(2); // Always right-pad to 2 characters
        }

    }
}
