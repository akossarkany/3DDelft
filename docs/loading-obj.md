# 6. OBJ Masterplans and Detailed Building Storage in Database

## Prerequisites
3D Delft uses a PostgreSQL database and Python to establish and authenticate the connection with the database. Before inserting OBJ models, ensure that you understand the logic and network of the connections.

### Install PostgreSQL
Refer to the official PostgreSQL installation guide [here](https://www.postgresql.org/download/).

### Install Python and Required Modules
Ensure that [Python3+](https://www.python.org/downloads/) is installed on your system along with the necessary modules: `psycopg2` for PostgreSQL database interaction and `Flask` for managing the web server and authentication.

```bash
pip install psycopg2 flask
```


## Introduction

This part covers the process of storing OBJ files in the server database, enabling the replacement of city parts with masterplans of future developments and detailed models of landmarks in Delft. The data is managed using a PostgreSQL database, which stores file paths and associated metadata for the models.

- [Quick Start](./pages/3dobjects/quickstart.md): For administators. The process of uploading `.obj` files.
- [Server Implementation](./pages/3dobjects/server.md): For developers. The documentation of the server side scripts.
- [Client Implementation](./pages/3dobjects/client.md): For developers. The documentation of the client side `.obj` loader functionality.

