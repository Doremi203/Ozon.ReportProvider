using FluentMigrator;

namespace Ozon.ReportProvider.Dal.Migrations;

[Migration(20240426173700, TransactionBehavior.None)]
public class InitScheme : Migration {
    public override void Up()
    {
        Create.Table("report_requests")
            .WithColumn("request_id").AsGuid().PrimaryKey("report_requests_pk").NotNullable()
            .WithColumn("good_id").AsGuid().NotNullable()
            .WithColumn("start_of_period").AsDateTimeOffset().NotNullable()
            .WithColumn("end_of_period").AsDateTimeOffset().NotNullable()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("report_requests");
    }
}