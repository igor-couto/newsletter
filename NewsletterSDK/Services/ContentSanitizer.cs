using Ganss.Xss;

namespace NewsletterSDK.Services;

internal static class ContentSanitizer
{
    public static string Sanitize(string htmlContent)
    {
        var sanitizer = new HtmlSanitizer();

        sanitizer.AllowedSchemes.Add("mailto");

        var html = htmlContent.Replace("\\\"", "\"");
        
        return sanitizer.SanitizeDocument(html);
    }
}