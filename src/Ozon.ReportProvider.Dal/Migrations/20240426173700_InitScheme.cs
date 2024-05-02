using FluentMigrator;

namespace Ozon.ReportProvider.Dal.Migrations;

[Migration(20240426173700, TransactionBehavior.None)]
public class InitScheme : Migration {
    public override void Up()
    {
        const string sql = @"
create table reports
(
    request_id       bigint not null primary key,
    conversion_ratio numeric not null,
    sold_count       bigint not null
);
";
        Execute.Sql(sql);
    }

    public override void Down()
    {
        Delete.Table("reports");
    }
}