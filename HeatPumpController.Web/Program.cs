using HeatPumpController.Controller.Svc;
using HeatPumpController.Web.Services;
using Prometheus;
using Prometheus.Client.AspNetCore;
using Prometheus.Client.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterControllerSvc(builder);

builder.Services.AddSingleton<IViewModel, ViewModel>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMetricFactory();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseMetricServer();
app.UsePrometheusServer();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
