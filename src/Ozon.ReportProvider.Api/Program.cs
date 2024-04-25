using Ozon.ReportProvider.Dal.Extensions;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.
services.AddGrpc();
services.AddGrpcReflection();

services.AddDal(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.Run();