using FluentMigrator;
using NewsletterSDK.Extensions;
using NewsletterSDK.Models;

namespace AdventureHubMigrations;

[Migration(6)]
public class SeedPublicationDeliveryStatusTable : Migration
{
    public override void Up()
    {
        Insert.IntoTable("publication_delivery_status")
        .Row(new {
            id = (short) DeliveryStatus.Failed,
            name= DeliveryStatus.Failed.ToString(),
            description = DeliveryStatus.Failed.GetDescription(),
        })
        .Row(new {
            id = (short) DeliveryStatus.Sent,
            name= DeliveryStatus.Sent.ToString(),
            description = DeliveryStatus.Sent.GetDescription(),
        });
    }

    public override void Down() 
        => Delete.FromTable("publication_status")
            .Row(new { id = (short) DeliveryStatus.Sent })
            .Row(new { id = (short) DeliveryStatus.Failed });
}