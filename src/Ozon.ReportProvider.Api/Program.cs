using Ozon.ReportProvider.Api.Config;
using Ozon.ReportProvider.Api.Services;
using Ozon.ReportProvider.Bll.Extensions;
using Ozon.ReportProvider.Dal.Extensions;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.
services.AddGrpc();
services.AddGrpcReflection();

MapsterConfig.Configure();
services
    .AddBllServices()
    .AddDal(configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MigrateDown();
}

app.MapGrpcService<ReportsGrpcService>();
app.MapGrpcReflectionService();

app.MigrateUp();

app.Run();