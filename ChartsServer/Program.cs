using ChartsServer.Hubs;
using ChartsServer.Models;
using ChartsServer.Subscription;
using ChartsServer.Subscription.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
           policy.AllowAnyMethod().AllowAnyHeader().AllowCredentials().SetIsOriginAllowed(origin => true)
       ));
builder.Services.AddSignalR();

//satislar ve personeller tablosu dinleniyor
builder.Services.AddSingleton<DatabaseSubscription<Satislar>>();
builder.Services.AddSingleton<DatabaseSubscription<Personeller>>();
var app = builder.Build();

app.UseDatabaseSubscription<DatabaseSubscription<Satislar>>("Satislar");
app.UseDatabaseSubscription<DatabaseSubscription<Personeller>>("Personeller");
app.UseCors();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<SatisHub>("/satishub");
});
app.Run();
