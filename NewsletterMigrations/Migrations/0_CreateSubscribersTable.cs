using FluentMigrator;
using NewsletterSDK.Models;

namespace NewsletterMigrations;

[Migration(0)]
public class CreateSubscribersTable : Migration
{
    public override void Up()
    {
        Create.Table("subscribers")
            .WithColumn("email")
                .AsString(255)
                .NotNullable()
                .PrimaryKey()
            .WithColumn("name")
                .AsString(Subscriber.NameMaxLength)
                .Nullable()
                .WithDefaultValue(null)
            .WithColumn("created_at")
                .AsDateTime()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime);
    }

    public override void Down() => Delete.Table("subscribers");
}