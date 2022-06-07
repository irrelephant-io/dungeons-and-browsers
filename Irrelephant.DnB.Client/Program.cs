using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Irrelephant.DnB.Client;
using Irrelephant.DnB.Client.Infrastructure;
using Irrelephant.DnB.DataTransfer.Extensions;
using Irrelephant.DnB.DataTransfer.Options;
using Irrelephant.DnB.DataTransfer.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("app");
builder.Logging.SetMinimumLevel(LogLevel.Trace);

builder.Services
    .AddOptions()
    .Configure<OidcProviderOptions>(opts => builder.Configuration.GetSection("OpenIdConnect").Bind(opts))
    .Configure<ApiConnectionOptions>(opts => builder.Configuration.GetSection("Api").Bind(opts));

builder.Services.AddOidcAuthentication(options => {
    builder.Configuration.Bind("OpenIdConnect", options.ProviderOptions);
});

builder.Services.AddDungeonsClients();

builder.Services
    .AddScoped<IApiTokenProvider, ApiTokenProvider>()
    .AddScoped<IRemoteCombatListenerFactory, RemoteCombatListenerFactory>();

await builder.Build().RunAsync();