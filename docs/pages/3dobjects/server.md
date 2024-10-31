# 7. Server Setup and Object Management

This chapter outlines how the Flask server interacts with the Apache2 web server using WSGI to handle secure connections and how authentication and object upload/download processes are managed, including metadata storage in PostgreSQL.

## 7.1 Server Information

To manage RESTful services for OBJ file storage and manipulation, the following setup is required:

- **Flask**: A Python package used to implement the RESTful server.
- **Apache2**: Web server software, used here to handle SSL (for secure connections).
- **WSGI**: A tool that allows Flask to interface with Apache2, enabling the server to handle HTTP requests.

## 7.2 Authentication System

The authentication system for uploading, validating, and downloading OBJ files uses a combination of **basic authentication** and **Bearer tokens**.

### 7.2.1 Login and Token Generation

Users must log in to authenticate. Flask handles this in the `/login` route using basic auth (username and password). Upon successful authentication, a **Bearer token** is generated, which serves as a session token. The token does not expire for now, but can be extended with session timeouts in the future.

Here is an example of the login flow:

- The login route (`/login`) checks for the authorization header in the request.
- If the credentials match the admin's username and password, a UUID-based token is generated and stored.
- This token is sent back to the user and used for subsequent requests to access restricted endpoints.

Code example from `auth.py`:

```python
@auth.route('/login', methods=['GET', 'POST'])
def login():
    if request.authorization:
        autho = request.headers.get('Authorization')
        if autho.startswith('Basic'):
            if (request.authorization.username == USERNAME) & (request.authorization.password == PASSWORD):
                VALID_TOKENS.append(str(uuid.uuid4()))
                return jsonify({"token": f"Bearer {VALID_TOKENS[-1]}"})
            else:
                return jsonify({"message": "Incorrect username or password."}), 400
    return jsonify({"message": "Enter login credentials."}), 401
```

### 7.2.2 Token Validation

Token validation is handled by the `/validate` route, where the server checks if a Bearer token is valid. Only valid tokens can access certain endpoints, like object uploads or downloads.

```python
@auth.route("/validate", methods=['GET', 'POST'])
def validate():
    auth_header = request.headers.get('Authorization')
    if auth_header and auth_header.startswith('Bearer '):
        if auth_header.split(" ")[1] in VALID_TOKENS:
            return "authenticated"
    return "", 401
```

## 7.3 Object Upload and Download

OBJ files and their metadata are stored both in the local file system and in a PostgreSQL database.

### 7.3.1 Object Upload

OBJ files are uploaded as plain text. The file's UUID serves as its identifier and is stored in the `/var/www/flaskapp/files` directory. Metadata associated with the OBJ file (such as position, rotation, and scale) is stored in the PostgreSQL database using a separate endpoint.

The `obj_upload()` function in `main.py` handles the file upload:

```python
@main.route('/upload/object', methods=['POST'])
@token_required
def obj_upload():
    if request.content_type == 'text/plain':
        fname = uuid.uuid4()
        with open(f'{OBJ_DIR}/{fname}.obj', 'w') as f:
            f.write(request.data.decode())
        return jsonify({"message": "File successfully created.", "id": f"{fname}"}), 201
    return jsonify({"message": "Incompatible content type. Upload obj as text"}), 400
```

### 7.3.2 Metadata Upload

After uploading the OBJ file, metadata like position, rotation, scale, and the model's name are submitted through the `/upload/meta/<uuid:name>` route. The metadata is stored in PostgreSQL, with an entry associated with the UUID of the object file.

Example metadata structure (JSON format):

```json
{
  "name": "Landmark Building",
  "description": "A detailed model of a landmark.",
  "position": [100, 200, 50],
  "rotation": [0, 0, 90],
  "scale": [1, 1, 1],
  "is_masterplan": [false]
}
```

The metadata is inserted into PostgreSQL using the `meta_handler.insert_metadata()` function from the `meta_upload()` route:

```python
@main.route('/upload/meta/<uuid:name>', methods=['POST'])
@token_required
def meta_upload(name):
    if os.path.isfile(f"{OBJ_DIR}/{name}.obj"):
        if request.is_json:
            body = request.get_json()
            meta_handler.insert_metadata(str(name), body['name'], body['description'], body['position'], body['rotation'], body['scale'], meta_handler.exists(str(name)))
            return jsonify({"message": "Metadata successfully inserted."}), 201
        return jsonify({"message": "Request mime type has to be 'application/json'"}), 400
    return jsonify({"message": "No file exists with the given identifier."}), 400
```

### 7.3.3 Object Download

Objects can be downloaded by querying their UUID. The object and its metadata are retrieved through two separate routes. The `/download/object/<uuid:name>` endpoint serves the OBJ file, while `/download/meta/<uuid:name>` returns the associated metadata.

For example, the `obj_download()` function allows downloading an OBJ file as plain text:

```python
@main.route('/download/object/<uuid:name>', methods=['GET'])
def obj_download(name):
    if os.path.isfile(f"{OBJ_DIR}/{name}.obj"):
        response = Response()
        with open(f"{OBJ_DIR}/{name}.obj", mode='r') as f:
            response.content_type = "text/plain"
            response.set_data(f.read())
        return response
    return jsonify({"message": "Requested resource does not exist."}), 404
```

## 7.4 Database Management

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

This allows storing metadata, like the object's position, rotation, and scale, along with the file path in the database for future retrieval.