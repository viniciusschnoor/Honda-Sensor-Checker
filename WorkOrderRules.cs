namespace HondaSensorChecker
{
    internal static class WorkOrderRules
    {
        internal const string ExpectedFormats =
            "OD + 15 dígitos (Dummy), O11 + 10 dígitos (produção) ou O12 + 10 dígitos (rework)";

#if DEBUG
        internal const string DebugAccBypassLabel = "O012345678912";
#endif

        internal static bool TryNormalizeScannedLabel(string? value, out string workOrderNumber)
        {
            var label = (value ?? string.Empty).Trim().ToUpperInvariant();

#if DEBUG
            if (string.Equals(label, DebugAccBypassLabel, StringComparison.Ordinal))
            {
                workOrderNumber = label[1..];
                return true;
            }
#endif

            if (label.Length == 17 &&
                label.StartsWith("OD", StringComparison.Ordinal) &&
                ContainsOnlyDigits(label, 2))
            {
                workOrderNumber = label[1..];
                return true;
            }

            if (label.Length == 13 &&
                (label.StartsWith("O11", StringComparison.Ordinal) ||
                 label.StartsWith("O12", StringComparison.Ordinal)) &&
                ContainsOnlyDigits(label, 1))
            {
                workOrderNumber = label[1..];
                return true;
            }

            workOrderNumber = string.Empty;
            return false;
        }

        internal static bool TryNormalizeForLookup(string? value, out string workOrderNumber)
        {
            var normalized = (value ?? string.Empty).Trim().ToUpperInvariant();
            if (TryNormalizeScannedLabel(normalized, out workOrderNumber))
                return true;

            return TryNormalizeScannedLabel($"O{normalized}", out workOrderNumber);
        }

        internal static string FormatStoredNumber(string? workOrderNumber)
        {
            if (string.IsNullOrWhiteSpace(workOrderNumber))
                return "N/D";

            var normalized = workOrderNumber.Trim().ToUpperInvariant();
            return normalized.StartsWith('O') ? normalized : $"O{normalized}";
        }

        private static bool ContainsOnlyDigits(string value, int startIndex)
        {
            for (var index = startIndex; index < value.Length; index++)
            {
                if (!char.IsDigit(value[index]))
                    return false;
            }

            return true;
        }
    }
}
