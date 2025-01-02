using System.Security;
using Ganss.Xss;

namespace NewsletterSDK.Services;

internal static class ContentSanitizer
{
    public static string Sanitize(string htmlContent)
    {
        var sanitizer = new HtmlSanitizer();

        sanitizer.AllowedSchemes.Add("mailto");

        sanitizer.RemovingTag += (sender, e) =>
        {
            if (string.Equals(e.Tag.TagName, "script", StringComparison.OrdinalIgnoreCase))
                throw new SecurityException("No <script> tags allowed in the content.");
        };

        var html = htmlContent.Replace("\\\"", "\"");
        
        return sanitizer.SanitizeDocument(html);
    }
}