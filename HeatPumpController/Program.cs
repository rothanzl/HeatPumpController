using HeatPumpController.Controller.Svc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.RegisterControllerSvc();

var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.Run();