using FluentMigrator;

namespace Ozon.ReportProvider.Dal.Migrations;

[Migration(20240426191000, TransactionBehavior.None)]
public class AddReportEnitityV1 : Migration {
    public override void Up()
    {
        const string sql = $@"
DO $$
    begin
        IF not exists (select 1 from pg_type where typname = 'reports_v1') then
            create type reports_v1 as
            (
                  request_id       bigint
                , conversion_ratio numeric
                , sold_count       bigint
            );
        end IF;
    end
$$;
        ";
        Execute.Sql(sql);
    }

    public override void Down()
    {
        const string sql = @"
do $$
    begin
        drop type if exists reports_v1;
    end
$$;";

        Execute.Sql(sql);
    }
}