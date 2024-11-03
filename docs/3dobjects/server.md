# Server Setup and Object Management

This chapter outlines how the Flask server interacts with the Apache2 web server using WSGI to handle secure connections and how authentication and object upload/download processes are managed, including metadata storage in PostgreSQL.

## Server Information

To manage RESTful services for OBJ file storage and manipulation, the following setup is required:

- **Flask**: A Python package used to implement the RESTful server.
- **Apache2**: Web server software, used here to handle SSL (for secure connections).
- **WSGI**: A tool that allows Flask to interface with Apache2, enabling the server to handle HTTP requests.

## Authentication System

The authentication system for uploading, validating, and downloading OBJ files uses a combination of **basic authentication** and **Bearer tokens**.

### Login and Token Generation

Users must log in to authenticate. Flask handles this in the `/login` route using basic auth (username and password). Upon successful authentication, a **Bearer token** is generated, which serves as a session token. This token is then passed to a handler which inserts it into a database. Each token expires in 30 minutes.

Here is an example of the login flow:

- The login route (`/login`) checks for the authorization header in the request.
- If the credentials match the admin's username and password, a UUID-based token is generated and inserted to the database.
- The timestamp of the insertion is stored next to token.
- This token is sent back to the user and used for subsequent requests to access restricted endpoints.


### Token Validation

Token validation is handled by the `/validate` route, where the server checks if a Bearer token is valid. It does so by checking if its present in the `authorized_tokens` table of the database. If the token exists it checks the age of the token by comparing the insertion time with the current. If its more then 30 minutes, the token is removed from the database. Else the validation yields success.


## Object Upload and Download

OBJ files and their metadata are stored in the local file system and in a PostgreSQL database respectively.

### Object Upload

OBJ files are uploaded as plain text. The file's UUID serves as its identifier and is stored in the `/var/www/flaskapp/files` directory.Objects can be uploaded through the `/upload/object` endpoint with a `POST` request. The body of the request should contain the object file as plain text. Further, the authoriztion header must contain a valid Bearer token. On successful insertion, a newly generate object id (UUID) is returned to the client



### Metadata Upload

After uploading the OBJ file, metadata like position, rotation, scale, and the model's name are submitted through the `/upload/meta/<uuid:name>` route. Here `<uuid:name>` is the id of the object, which is obtained after the object upload. The metadata is stored in PostgreSQL, with an entry associated with the UUID of the object file.

Example metadata structure (JSON format):

```json
{
  "obj_id": "<uuid-of-the-object>",
  "name": "Landmark Building",
  "description": "A detailed model of a landmark.",
  "position": [100, 200, 50],
  "rotation": [0, 0, 90],
  "scale": [1, 1, 1],
  "is_masterplan": false
}
```


### Object Download

Objects and their metadata can be downloaded by their UUID. To get a list with the uuid's of all objects on the server, a request have to be sent to the `/queryall` endpoint. Then the object and its metadata are retrieved through two separate routes. The `/download/object/<uuid:name>` endpoint serves the OBJ file as a plain text in the response body, while `/download/meta/<uuid:name>` returns the associated metadata as json.


## Database Management

The metadata is stored in a PostgreSQL database using the `psycopg2` Python module. The connection to PostgreSQL is established in `connection.py`. Here’s an example of how the database connection is handled:

```python
def connect(self):
    """Establish connection to the PostgreSQL database."""
    try:
        self.connection = psycopg2.connect(
            database=self.database,
            user=self.user,
            password=self.password,
            host=self.host,
            port=self.port
        )
        print("Database connection established.")
    except psycopg2.OperationalError as e:
        print(f"Operational error connecting to the database: {e}")
        self.connection = None
```

This allows storing metadata, like the object's position, rotation, and scale, along with the file path in the database for future retrieval. To configure the connection parameters refer to [the database setup](./postgresdb.md#configure-application)