using FluentMigrator;

namespace Ozon.ReportProvider.Dal.Migrations;

[Migration(20240427130700, TransactionBehavior.None)]
public class AddUserIdIndexToReportRequestsTable : Migration {
    public override void Up()
    {
        const string sql = $@"
create index if not exists user_id_idx on report_requests (user_id)
        ";
        Execute.Sql(sql);
    }

    public override void Down()
    {
        const string sql = @"
drop index if exists user_id_idx
";
        Execute.Sql(sql);
    }
}