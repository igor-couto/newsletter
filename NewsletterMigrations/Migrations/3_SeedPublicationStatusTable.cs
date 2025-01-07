using FluentMigrator;
using NewsletterSDK.Extensions;
using NewsletterSDK.Models;

namespace AdventureHubMigrations;

[Migration(3)]
public class SeedPublicationStatusTable : Migration
{
    public override void Up()
    {
        Insert.IntoTable("publication_status")
        .Row(new {
            id = (short) PublicationStatus.Pending,
            name= PublicationStatus.Pending.ToString(),
            description = PublicationStatus.Pending.GetDescription(),
        })
        .Row(new {
            id = (short) PublicationStatus.Delivered,
            name= PublicationStatus.Delivered.ToString(),
            description = PublicationStatus.Delivered.GetDescription(),
        })
        .Row(new {
            id = (short) PublicationStatus.PartiallyDelivered,
            name= PublicationStatus.PartiallyDelivered.ToString(),
            description = PublicationStatus.PartiallyDelivered.GetDescription(),
        })
        .Row(new {
            id = (short) PublicationStatus.Failed,
            name= PublicationStatus.Failed.ToString(),
            description = PublicationStatus.Failed.GetDescription(),
        })
        .Row(new {
            id = (short) PublicationStatus.Deleted,
            name= PublicationStatus.Deleted.ToString(),
            description = PublicationStatus.Deleted.GetDescription(),
        });
    }

    public override void Down() 
        => Delete.FromTable("publication_status")
            .Row(new { id = (short) PublicationStatus.Pending })
            .Row(new { id = (short) PublicationStatus.Delivered })
            .Row(new { id = (short) PublicationStatus.PartiallyDelivered })
            .Row(new { id = (short) PublicationStatus.Failed })
            .Row(new { id = (short) PublicationStatus.Deleted });
}