sudo cp -f kestrel-webapp.service /etc/systemd/system/.

sudo systemctl daemon-reload
sudo systemctl enable kestrel-webapp.service
sudo systemctl restart kestrel-webapp.service