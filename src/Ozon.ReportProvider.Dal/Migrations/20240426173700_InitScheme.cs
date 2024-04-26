using FluentMigrator;

namespace Ozon.ReportProvider.Dal.Migrations;

[Migration(20240426173700, TransactionBehavior.None)]
public class InitScheme : Migration {
    public override void Up()
    {
        Create.Table("reports")
            .WithColumn("id").AsInt64().PrimaryKey("reports_pk").Identity()
            .WithColumn("good_id").AsGuid().NotNullable()
            .WithColumn("layout_id").AsInt64().NotNullable()
            .WithColumn("start_of_period").AsDateTimeOffset().NotNullable()
            .WithColumn("end_of_period").AsDateTimeOffset().NotNullable()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable();

        Create.Table("user_reports")
            .WithColumn("user_id").AsGuid().NotNullable()
            .WithColumn("report_id").AsInt64().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("reports");
        Delete.Table("user_reports");
    }
}