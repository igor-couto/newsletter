using NewsletterMigrations.Services;

var direction = args.Length > 0 ? args[0].ToLower() : "up";

if (direction == "down")
    MigrationService.MigrateDown();

if (direction == "up")
    MigrationService.MigrateUp();