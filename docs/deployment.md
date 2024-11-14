# 7. Deploying Delft3D

To deploy Delft3D you will need a server with a public domain name. Furthermore, the server must allow remote connections on port 80 and 443. Make sure you have administator (sudo) priviliges.

## 7.1 Download the files from github

Links to be added once zipped project files uploaded to github.

- [delft3D]()
- [flaskapp]()
- [tileset]()

## 7.2 Upload application files

### WebGL application files

Upload files the build files of Unity (`delft3d.zip`) to your server and move them to `/var/www/delft3d`, such that `/var/www/delft3d/index.html` is a valid path. Then upload the tileset data: `tileset.zip` to `/var/www/delft3d and unzip it. Furthermore, upload `aoi.geojson` to the same folder. 

### OBJ Upload backend scripts

Upload `flaskapp.zip` to `/var/www` and unzip it. This will create a new folder `/var/www/flaskapp`. Then execute the following command:
```bash
$ sudo chown www-data:www-data /var/www/flaskapp/files
```

Furthermore, make sure you set up the database for the application as described [here](./3dobjects/postgresdb.md).

### Finally 

Make sure all files under `/var/www/delft3d` and `/var/www/flaskapp` are readable for all users.

For folders use: 
```bash
$ sudo chmod 755 <folder_name>
```
For files use:
```bash
$ sudo chmod 644 <file_name>
```


## 7.3 HTTP server
During development the application was tested with [Apache2](https://httpd.apache.org/) webserver. To install Apache2 on [Ubuntu](https://ubuntu.com/) refer to [this tutorial](https://ubuntu.com/tutorials/install-and-configure-apache#1-overview). Once Apache2 is installed, follow [this tutorial]() to enable `SSL` for the website. In the end the `VirtualHost` configuration should look like:

```xml
<VirtualHost *:443>
    ServerAdmin serveradmin@example.nl
    ServerName example.nl

    DocumentRoot /var/www/delft3d

    CustomLog ${APACHE_LOG_DIR}/access.log combined

    SSLEngine on

    SSLCertificateFile      /path/to/ssl_certificate
    SSLCertificateKeyFile   /path/to/ssl_key

    <FilesMatch "\.(?:cgi|shtml|phtml|php)$">
            SSLOptions +StdEnvVars
    </FilesMatch>
    <Directory /usr/lib/cgi-bin>
            SSLOptions +StdEnvVars
    </Directory>
    <IfModule mod_headers.c>
            Header set Access-Control-Allow-Origin "*"
    </IfModule>
</VirtualHost>
```

For the flask application you will have to configure Apache2 to use WSGI. Make sure you [enable MOD-WSGI](./loading-obj.md#prerequisites) for Apache2. Then create a new configuration file `/etc/apache2/sites-available/flaskapp.conf` and copy the following configuration:

```xml
<VirtualHost *:80>
    ServerAdmin serveradmin@example.nl
    ServerName  example.nl

    WSGIDaemonProcess flaskapp python-path=/var/www/flaskapp
    WSGIScriptAlias / /var/www/flaskapp/flaskapp.wsgi
    WSGIPAssAuthorization On

    <Directory /var/www/flaskapp>
        Require all granted
    </Directory>

    Alias /static /var/www/flaskapp/static
    <Directory /var/www/flaskapp/static/>
        Require all granted
    </Directory>

    ErrorLog ${APACHE_LOG_DIR}/flaskapp_error.log
    CustomLog ${APACHE_LOG_DIR}/flaskapp_access.log combined

    SSLEngine on

    SSLCertificateFile      /path/to/ssl_certificate
    SSLCertificateKeyFile   /path/to/ssl_key

    <FilesMatch "\.(?:cgi|shtml|phtml|php)$">
            SSLOptions +StdEnvVars
    </FilesMatch>
    <Directory /usr/lib/cgi-bin>
            SSLOptions +StdEnvVars
    </Directory>
</VirtualHost>
```
#### Note 1
Change `ServerAdmin`, `ServerName`, `SSLCertificateFile`, and `SSLCertificateKeyFile` to your configuration.

#### Note 2
If you make changes to the Files or File Structure of the sites of apache2, make sure to reload it with
```bash
$ sudo systemctl reload apache2
```
If you enable/disable sits or mods, make sure to restart apache2 with
```bash
$ sudo systemctl restart apache2
```

[< Loading and Storing OBJ](./loading-obj.md) | [Home](./index.md) | [Next: Future >](./future.md)




