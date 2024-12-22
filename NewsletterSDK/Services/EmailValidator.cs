using System.Text.RegularExpressions;

namespace NewsletterSDK.Services;

public static partial class EmailValidator
{
    private static readonly Regex EmailRegex = MyRegex();

    public static bool IsValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return EmailRegex.IsMatch(email);
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}
