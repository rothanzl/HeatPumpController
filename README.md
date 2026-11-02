# HeatPumpController

ASP.NET Core web application controlling a heat pump, deployed on a Raspberry Pi in the local network.

## Deployment server

| Item | Value |
|------|-------|
| Address | `192.168.1.54` |
| User | `pi` |
| App directory | `/media/pi/USB12/HeatPumpController/bin` |
| .NET runtime | `/home/pi/.dotnet/dotnet` |
| Web app port | `5000` (proxied by nginx on port `80`) |
| Prometheus | port `9090`, data in `/media/pi/USB1/prometheus-data` |
| Grafana | port `3000` |

### Connect via SSH

```bash
ssh pi@192.168.1.54
```

Login uses the `pi` user's password. The Pi is reachable only from the local network.

### Manage the service

The app runs as the `kestrel-webapp` systemd service (see [Install/kestrel-webapp.service](Install/kestrel-webapp.service)).

```bash
sudo systemctl status kestrel-webapp.service
sudo systemctl restart kestrel-webapp.service
sudo journalctl -u kestrel-webapp.service -f
```

### Deploy a new version

The repository is cloned directly on the Pi. To deploy, SSH in and run the publish script from the repo root
(see [publish-and-run.sh](publish-and-run.sh)), which pulls the latest code, rebuilds into `bin/` and restarts the service:

```bash
ssh pi@192.168.1.54
cd /media/pi/USB12/HeatPumpController
./publish-and-run.sh
```

### Install scripts

The [Install/](Install/) folder contains scripts for setting up a fresh Pi:

- `install_web_daemon.sh` – installs and enables the `kestrel-webapp` systemd service
- `install_nginx.sh` – installs nginx and the reverse proxy config (`nginx-default`)
- `install_prometheus.sh` – installs Prometheus with `prometheus.yml` and `prometheus.service`
- `install_grafana.sh` – installs Grafana
