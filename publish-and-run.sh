git pull
sudo systemctl stop kestrel-webapp.service
dotnet publish -c release -o bin
sudo systemctl start kestrel-webapp.service