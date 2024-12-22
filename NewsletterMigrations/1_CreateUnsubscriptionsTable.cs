using FluentMigrator;
using NewsletterSDK.Models;

namespace NewsletterMigrations;

[Migration(1)]
public class CreateUnsubscriptionsTable : Migration
{
    public override void Up()
    {
        Create.Table("unsubscriptions")
            .WithColumn("id")
                .AsInt32()
                .PrimaryKey()
                .Identity()
            .WithColumn("email")
                .AsString(255)
                .NotNullable()
            .WithColumn("name")
                .AsString(255)
                .Nullable()
            .WithColumn("subscribed_at")
                .AsDateTime()
                .Nullable()
            .WithColumn("unsubscribed_at")
                .AsDateTime()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime)
            .WithColumn("reason")
                .AsString(Unsubscription.ReasonMaxLength)
                .Nullable()
                .WithDefaultValue(null);

        Create.Index("unsubscriptions_email_index")
            .OnTable("unsubscriptions")
            .OnColumn("email");
    }

    public override void Down() => Delete.Table("unsubscriptions");
}