sudo apt-get update
sudo apt-get -y install nginx
sudo service nginx start
sudo cp -f nginx-default /etc/nginx/sites-available/default
sudo nginx -s reload