using Ozon.ReportProvider.Api;
using Ozon.ReportProvider.Dal.Extensions;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
    .Build();

var env = host.Services.GetRequiredService<IWebHostEnvironment>();
if (env.IsDevelopment())
{
    host.MigrateDown();
}

host.MigrateUp();

host.Run();