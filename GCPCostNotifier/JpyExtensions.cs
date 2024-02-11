namespace GCPCostNotifier;

using System.Globalization;

public static class JpyExtensions
{
    public static string ToJpyStyleString(this decimal value) =>
        value.ToString("C", CultureInfo.GetCultureInfo("ja-JP"));
}
