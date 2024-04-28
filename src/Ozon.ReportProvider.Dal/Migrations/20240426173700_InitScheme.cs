using FluentMigrator;

namespace Ozon.ReportProvider.Dal.Migrations;

[Migration(20240426173700, TransactionBehavior.None)]
public class InitScheme : Migration {
    public override void Up()
    {
        Create.Table("reports")
            .WithColumn("request_id").AsInt64().PrimaryKey("reports_pk").NotNullable()
            .WithColumn("conversion_ratio").AsDecimal().NotNullable()
            .WithColumn("sold_count").AsInt32().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("reports");
    }
}