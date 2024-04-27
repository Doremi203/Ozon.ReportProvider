using FluentMigrator;

namespace Ozon.ReportProvider.Dal.Migrations;

[Migration(20240426191000, TransactionBehavior.None)]
public class AddReportRequestEnitityV1 : Migration {
    public override void Up()
    {
        const string sql = $@"
DO $$
    begin
        IF not exists (select 1 from pg_type where typname = 'report_requests_v1') then
            create type report_requests_v1 as
            (
                  id         bigint
                , user_id      text
                , good_id      text
                , layout_id    bigint
                , start_of_period timestamp with time zone
                , end_of_period timestamp with time zone
                , created_at timestamp with time zone
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
        drop type if exists report_requests_v1;
    end
$$;";

        Execute.Sql(sql);
    }
}