using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
using Redis_Stream;
using Redis_Stream.EventBus.Events;
using Redis_Stream.EventBus.Handlers;
using Redis_Stream.EventBus.Manager;
using Redis_Stream.EventBus.Services;
using StackExchange.Redis;
using System;

var builder = WebApplication.CreateBuilder(args);

var multiplexer = ConnectionMultiplexer.Connect("localhost:6379, abortConnect=false");
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);
builder.Services.AddScoped<IEventBus, EventBus>();
builder.Services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
builder.Services.AddSingleton<IRedisPersistentConnection, RedisPersistentConnection>();
builder.Services.AddSingleton<IWorkerService, WorkerService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var appDb = new AppDbConfiguration();
builder.Configuration.GetSection("AppDb").Bind(appDb);

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage($"Server={appDb.Server};Port={appDb.Port};User Id={appDb.UserName};Password={appDb.Password};Database={appDb.Database};"));
builder.Services.AddHangfireServer();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseHangfireServer();
app.UseHangfireDashboard();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
using var scope = app.Services.CreateScope();
var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();


await eventBus.Subscribe<TestEventHandler>(nameof(TestEvent), "testGroup", new List<string>() { "myComsumer" });
await eventBus.Subscribe<Test1EventHandler>(nameof(Test1Event), "test1Group", new List<string>() { "myComsumer1" });
var service = app.Services.GetService<IWorkerService>();
if (service != null)
{
    await service.Start(CancellationToken.None);
}

app.Run();
