using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(w =>
    {
        w.UseNewtonsoftJson();
    })
    .Build();

host.Run();
