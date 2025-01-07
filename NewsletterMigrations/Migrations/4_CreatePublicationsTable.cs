using FluentMigrator;
using NewsletterSDK.Models;

namespace NewsletterMigrations;

[Migration(4)]
public class CreatePublicationsTable : Migration
{
    public override void Up()
    {
        Create.Table("publications")
            .WithColumn("id")
                .AsGuid()
                .NotNullable()
                .PrimaryKey()
            .WithColumn("title")
                .AsString(Publication.TitleMaxLength)
                .NotNullable()
            .WithColumn("content")
                .AsString(int.MaxValue)
                .NotNullable()
            .WithColumn("status_id")
                .AsInt16()
                .NotNullable()
                .ForeignKey("publication_status", "id")
            .WithColumn("created_at")
                .AsDateTime()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("sending_date")
                .AsDateTime()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("deleted_at")
                .AsDateTime()
                .Nullable();

        Execute.Sql($"CREATE INDEX publications_status_pending_index ON publications(status_id) WHERE status_id = {(short)PublicationStatus.Pending};");
    }

    public override void Down()
    {
        Delete.Index("publications_status_pending_index").OnTable("publications");
        Delete.Table("publications");
    } 
}