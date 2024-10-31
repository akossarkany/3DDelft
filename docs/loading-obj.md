# 6. OBJ Masterplans and Detailed Building Storage in Database

## 6.1 Prerequisites
3D Delft uses a PostgreSQL database and Python to establish and authenticate the connection with the database. Before inserting OBJ models, ensure that you understand the logic and network of the connections.

### Install a webserver

To be able to serv user requests throug the web it is neccessary to have a running webserver with a public domain name. This way users can send `HTTP` requests to this server. Make sure you read [how to deploy the Apaache2 server](./deployment.md#http-server) first.

Once you installed the server, you should enable `MOD-WSGI` in apache2 for this application to work. You can do this with the following commands:
```bash
$ sudo apt-get install libapache2-mod-wsgi-py3
$ sudo a2enmod wsgi
```


### Install PostgreSQL
Refer to the official PostgreSQL installation guide [here](https://www.postgresql.org/download/). Once postgres is installed, read [this page](./pages/3dobjects/postgresdb.md) on how to set up a postgres database for this application.

### Install Python and Required Modules
Ensure that [Python3+](https://www.python.org/downloads/) is installed on your system along with the necessary modules: `psycopg2` for PostgreSQL database interaction and `Flask` for managing the web server and authentication. You can install these dependencies with `pip`:

```bash
$ pip install psycopg2 flask flask-cors pandas
```

## 6.2 Overview

This part of the documentation covers the process of storing OBJ files in the server database, enabling the replacement of city parts with masterplans of future developments and detailed models of landmarks in Delft.



<img width="1098" alt="RequestResponseCycle" src='./figs/schema.png'>

**Figure 1.**  Schematic summary of the request response cycle between the client and the server


## 6.3 More About


- [ObjUpload Quick Start](./3dobjects/quickstart.md): For administators. The process of uploading `.obj` files.
- [Server Implementation](./3dobjects/server.md): For developers. The documentation of the server side scripts.
- [Client Implementation](./3dobjects/client.md): For developers. The documentation of the client side `.obj` loader functionality.

[< Making and Loading 3D Tiles](./loading-2D-layers.md) | [Home](./index.md) | [Next: Deployment >](./deployment.md)


