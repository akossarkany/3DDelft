# Postgres Database Setup

The the metadatas of object files are stored in a postgres database alongside with the authorized users. This database is independent from the application therefore it has to be set up independently.

## Create database and tables

First create a new database for the project files. You can do this by connecting to postresql as the root user (default: `postgres`). Once you connected, create the new databas with:
```sql
CREATE DATABASE delft3d;
```

Once you created the new database connect to it with `\c delft3d`. Then you can start creating the tables by executing the following scripts. Make sure you use exactly the same names for the tables since this is not configurable in the application.
```sql

CREATE TABLE buildings (
    obj_id UUID PRIMARY KEY, 
    name VARCHAR(255) NOT NULL,
    description TEXT,
    position_x DOUBLE PRECISION NOT NULL,
    position_y DOUBLE PRECISION NOT NULL,
    position_z DOUBLE PRECISION NOT NULL,
    rotation_x DOUBLE PRECISION NOT NULL,
    rotation_y DOUBLE PRECISION NOT NULL,
    rotation_z DOUBLE PRECISION NOT NULL,
    scale_x DOUBLE PRECISION NOT NULL,
    scale_y DOUBLE PRECISION NOT NULL,
    scale_z DOUBLE PRECISION NOT NULL,
    is_master BOOLEAN NOT NULL
);

```

```sql
CREATE TABLE authorized_tokens (
    id UUID PRIMARY KEY,
    created_at TIMESTAMPTZ DEFAULT NOW()
);
```

## Create user for the application

As the root user (default: `postgres`), connect to the previously created `delft3d`. Then run the following command to create a new user:

```sql
CREATE USER unityuser WITH PASSWORD 'your-secret-pwd';
```
Then grant priviliges to the new users to the database with:
```sql
GRANT ALL PRIVILEGES ON DATABASE delft3d TO unityuser;
```
And for both tables as well:
```sql
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE buildings TO unityuser;GRANT SELECT, INSERT, DELETE ON TABLE authorized_tokens TO unityuser;
```

## Configure application

Now that you have set up the databases, open `flaskapp/dbconnect/connection.py` and make sure the connection's configuration matches the values you used during the setup:

```python
# connection.py

DATABASE = "delft3d"
USER = "unityuser"
PASSWORD = 'your-secret-pwd'
HOST = "localhost"
PORT = "5432"
```
