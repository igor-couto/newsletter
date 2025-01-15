namespace NewsletterIntegrationTests.Extension;

internal static class StringExtensions
{
    internal static string GetString(this string value) 
    {
        if (string.Equals(value, "null", StringComparison.OrdinalIgnoreCase))
            return null;

        if (string.Equals(value, "\"\"", StringComparison.OrdinalIgnoreCase))
            return string.Empty;

        if (string.Equals(value, "\" \"", StringComparison.OrdinalIgnoreCase))
            return " ";

        return value;
    }

    internal static string RemoveDoubleQuotes(this string value) 
    {
        return value.Replace("\"", string.Empty);;
    }
}
