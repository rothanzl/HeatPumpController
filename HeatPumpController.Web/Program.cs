using HeatPumpController.Controller.Svc;
using HeatPumpController.Web.Services;
using Prometheus;


var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterControllerSvc(builder);

builder.Services.AddSingleton<IViewModel, ViewModel>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseMetricServer();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
