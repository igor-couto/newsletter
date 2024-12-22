using FluentMigrator;
using NewsletterSDK.Models;

namespace NewsletterMigrations;

[Migration(7)]
public class CreatePublicationDeliveriesTable : Migration
{
    public override void Up()
    {
        Create.Table("publication_deliveries")
            .WithColumn("id")
                .AsInt32()
                .Identity()
                .PrimaryKey()
            .WithColumn("subscriber_email")
                .AsString(255)
                .NotNullable()
            .WithColumn("publication_id")
                .AsGuid()
                .NotNullable()
                .ForeignKey("publications", "id")
            .WithColumn("status_id")
                .AsInt16()
                .NotNullable()
                .ForeignKey("publication_delivery_status", "id")
            .WithColumn("sending_date")
                .AsDateTime()
                .NotNullable();

        Execute.Sql($"CREATE INDEX publication_deliveries_status_id_index ON publication_deliveries(status_id) WHERE status_id = {(short) DeliveryStatus.Failed};");
    }

    public override void Down()
    {
        Delete.Index("publication_deliveries_status_id_index").OnTable("publication_deliveries");
        Delete.Table("publication_deliveries");
    } 
}