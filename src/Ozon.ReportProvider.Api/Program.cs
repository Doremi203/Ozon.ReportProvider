using Ozon.ReportProvider.Api.Config;
using Ozon.ReportProvider.Dal.Extensions;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.
services.AddGrpc();
services.AddGrpcReflection();

MapsterConfig.Configure();
services.AddDal(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MigrateUp();

app.Run();