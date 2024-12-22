using System.ComponentModel;

namespace NewsletterSDK.Models;

public enum PublicationStatus : short
{
    [Description("Ready to send or scheduled")]
    Pending = 0,

    [Description("All deliveries were successfully sent")]
    Delivered = 1,

    [Description("Some deliveries failed")]
    PartiallyDelivered = 2,

    [Description("All deliveries failed")]
    Failed = 3,

    [Description("This post was deleted before it was sent.")]
    Deleted = 4
}