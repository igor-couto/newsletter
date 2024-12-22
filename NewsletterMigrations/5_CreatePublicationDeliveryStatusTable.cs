using FluentMigrator;

namespace NewsletterMigrations;

[Migration(5)]
public class CreatePublicationDeliveryStatusTable : Migration
{
    public override void Up()
    {
        Create.Table("publication_delivery_status")
            .WithColumn("id")
                .AsInt16()
                .NotNullable()
                .PrimaryKey()
            .WithColumn("name")
                .AsString(128)
                .NotNullable()
            .WithColumn("description")
                .AsString(255)
                .Nullable();
    }

    public override void Down()
    {
        Delete.Table("publication_delivery_status");
    } 
}