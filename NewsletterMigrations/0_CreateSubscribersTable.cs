using FluentMigrator;

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
                .AsString(255)
                .Nullable()
                .WithDefaultValue(null)
            .WithColumn("created_at")
                .AsDateTime()
                .NotNullable()
                .WithDefault(SystemMethods.CurrentDateTime);
    }

    public override void Down() => Delete.Table("subscribers");
}