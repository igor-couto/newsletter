using System.ComponentModel;

namespace NewsletterSDK.Models;

public enum DeliveryStatus: short
{
    [Description("Failed to deliver (e.g., invalid email or server issue)")]
    Failed = 0,

    [Description("Successfully delivered")]
    Sent = 1
}