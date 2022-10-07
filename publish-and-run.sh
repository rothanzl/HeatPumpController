git pull
dotnet publish -c release -o bin
cd bin
dotnet HeatPumpController.Web.dll