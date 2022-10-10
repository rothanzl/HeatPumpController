sudo groupadd --system prometheus
sudo useradd -s /sbin/nologin --system -g prometheus prometheus
sudo mkdir /var/lib/prometheus
for i in rules rules.d files_sd; do sudo mkdir -p /etc/prometheus/${i}; done

mkdir -p /tmp/prometheus
mkdir -p /media/pi/USB1/prometheus-data
sudo cp -f prometheus.yml /tmp/prometheus/.
sudo cp -f prometheus.service /etc/systemd/system/prometheus.service

sudo pat-get -y update
sudo apt -y install wget curl vim

cd /tmp/prometheus
curl -s https://api.github.com/repos/prometheus/prometheus/releases/latest | grep browser_download_url | grep linux-armv7 | cut -d '"' -f 4 | wget -qi -

tar xvf prometheus*.tar.gz
cd prometheus*/

sudo mv prometheus promtool /usr/local/bin/

prometheus --version
promtool --version

sudo cp ../prometheus.yml /etc/prometheus/prometheus.yml
sudo mv consoles/ console_libraries/ /etc/prometheus/


for i in rules rules.d files_sd; do sudo chown -R prometheus:prometheus /etc/prometheus/${i}; done
for i in rules rules.d files_sd; do sudo chmod -R 775 /etc/prometheus/${i}; done
sudo chown -R prometheus:prometheus /var/lib/prometheus/

sudo systemctl daemon-reload
sudo systemctl start prometheus
sudo systemctl enable prometheus

cd /tmp
sudo rm -r prometheus