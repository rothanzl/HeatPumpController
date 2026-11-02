# HeatPumpController

ASP.NET Core web application controlling a heat pump, deployed on a Raspberry Pi in the local network.

## Deployment server

| Item | Value |
|------|-------|
| Address | `192.168.1.54` |
| User | `pi` |
| App directory | `/mnt/usb/HeatPumpController/bin` (USB stick, see below) |
| .NET runtime | `/home/pi/.dotnet/dotnet` |
| Web app port | `5000` (proxied by nginx on port `80`) |
| Prometheus | port `9090`, data in `/media/pi/USB1/prometheus-data` |
| Grafana | port `3000` |

### Connect via SSH

```bash
ssh pi@192.168.1.54
```

Login uses the `pi` user's password. The Pi is reachable only from the local network.

### USB stick mount

The repository and build output live on a USB stick (label `USB1`, `UUID=F6E7-DC7F`). It is mounted at a fixed
path via `/etc/fstab`, and the service declares `RequiresMountsFor=/mnt/usb` so it waits for the mount:

```
UUID=F6E7-DC7F  /mnt/usb  vfat  defaults,nofail,uid=1000,gid=1000,fmask=0022,dmask=0022,x-systemd.device-timeout=15s  0  0
```

Do not rely on the desktop auto-mount under `/media/pi/USB*` – it picks a new folder name (`USB1`, `USB11`, `USB12`, …)
whenever the stick is remounted, which previously left the service crash-looping on a stale path.

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
cd /mnt/usb/HeatPumpController
./publish-and-run.sh
```

### Install scripts

The [Install/](Install/) folder contains scripts for setting up a fresh Pi:

- `install_web_daemon.sh` – installs and enables the `kestrel-webapp` systemd service
- `install_nginx.sh` – installs nginx and the reverse proxy config (`nginx-default`)
- `install_prometheus.sh` – installs Prometheus with `prometheus.yml` and `prometheus.service`
- `install_grafana.sh` – installs Grafana
