# 6. OBJ Masterplans and Detailed Building Storage in Database

## Prerequisites
Delft3D uses a PostgreSQL database and Python to establish and authenticate the connection with the database. Before inserting OBJ models, ensure that you understand the logic and network of the connections.

### Install PostgreSQL
Refer to the official PostgreSQL installation guide [here](https://www.postgresql.org/download/).

### Install Python and Required Modules
Ensure that Python is installed on your system along with the necessary modules: `psycopg2` for PostgreSQL database interaction and `Flask` for managing the web server and authentication.

**For Windows:**
- Use the terminal to install Python and the required modules:

```bash
pip install psycopg2 flask
```

**For macOS (using Homebrew):**
- Install Python, psycopg2, and Flask via Homebrew:

```zsh 
brew install python@3.11
pip install psycopg2 flask
```

Here is the revised and completed markdown text based on the section you provided:

---

## Introduction

This section covers the process of storing OBJ files in the server database, enabling the replacement of city parts with masterplans of future developments and detailed models of landmarks in Delft. The data is managed using a PostgreSQL database, which stores file paths and associated metadata for the models.

## Administrator Access

OBJ uploading and saving functionality is restricted to administrators. To access this functionality, administrators must log in using credentials and retrieve a bearer token for authentication.

Steps 1 and 2 can be performed in any order.

### Step 1. Import and Position OBJ Models

Users can import an OBJ model into the scene, preview, and manipulate its properties, including position, scale, and rotation.

The imported OBJ model will automatically be textured with a default material. This material renders both sides of each mesh face, meaning the orientation of normals does not affect the rendering.

#### Navigate to the **Lagen** (Layers) Tab

On the left side of the UI, you will find vertically stacked icons. Open the tab labeled **Lagen** to manage layers.

![Lagen Tab]()

#### Click the **+** Icon to Import Data

At the bottom of the **Lagen** tab, you will find a plus icon (`+`) for adding new data.

![Plus Icon]()

#### Select **Importeren** and **Eigen Bestand** to Load Your OBJ Model

From the import menu, select **Importeren** (Import) and then choose **Eigen Bestand** (Your Own File) to upload your OBJ model.

![Import Menu]()

#### Upload Your OBJ File

After selecting your file, it will be uploaded and displayed in the Unity viewer.

![OBJ File Upload]()

#### Position the OBJ Model Correctly

Once uploaded, position your OBJ model within the scene by adjusting its position, rotation, and scale. These settings can be manipulated directly in the viewer.

---

### Step 2. Log In

To begin uploading OBJ files, administrators must authenticate by logging in.

#### Click on the **Sign In** Button

In the top-right corner of the viewer UI, there is a **Sign In** button.

![Sign In Button]()

#### Enter Your Username and Password

A pop-up window will appear, prompting you to enter your administrator credentials.

![Login Prompt]()

#### Retrieve Your Bearer Token

After successful authentication, the system will generate a bearer token that looks like: `155f1caf-288f-4e9e-9e64-6eef09e60bb1`.

![Bearer Token Example]()

#### Copy and Paste the Token

Copy the generated bearer token and paste it into the designated input field within the Unity viewer.

![Token Input Field]()

#### Access the **Save** Dropdown Menu

Once logged in successfully, the **Sign In** button will change to a **Save** dropdown menu, allowing you to save your OBJ model to the database.

![Save Dropdown Menu]()

---

### Step 3. Save the OBJ Model to the Database

After positioning the OBJ model, follow these steps to save it to the database:

#### Select the Model in the Scene

Click on the model within the scene to select it.

![Model Selection]()

#### Review or Edit the Model's Name

The model's name will be automatically filled in but can be edited if necessary.

![Model Name]()

#### Add a Description and Indicate **Masterplan** Status

Provide a description for the model and indicate whether it is part of a **Masterplan** by checking the corresponding box.

![Model Description]()

#### Press **Save**

After filling in the required information, press the **Save** button to send the model and its metadata (position, rotation, scale) to the PostgreSQL database.

![Save Button]()

Upon saving, the model will be accessible in future sessions, with its metadata securely stored in the database.

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

### 8. Client (Unity WebGL)

This section focuses on the Unity WebGL application, detailing how it interacts with the Flask server for downloading, uploading, and managing metadata of OBJ files. It also covers the login and authentication workflow using Bearer tokens.

#### 8.1 Object Download

In the Unity WebGL client, OBJ files are requested from the server and rendered in the Unity scene. The download process includes web requests, object download, and metadata management to ensure correct object placement.

##### 8.1.1 Data Flow in Unity

The data flow in Unity starts when a user requests an OBJ file, which is fetched via a `UnityWebRequest`. Once the request is completed, the file is parsed and placed into the scene.

The main flow:
- A GET request is sent to retrieve the OBJ file.
- Upon success, the OBJ data is processed.
- The metadata, such as position and scale, is also downloaded and applied.

##### 8.1.2 Web Request Flow

Unity uses `UnityWebRequest.Get()` to request the OBJ files. The response is handled as follows:

```csharp
IEnumerator DownloadObject(string url, Action<string> callback)
{
    UnityWebRequest request = UnityWebRequest.Get(url);
    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.Success)
    {
        callback(request.downloadHandler.text);  // Process the OBJ data
    }
    else
    {
        Debug.LogError("Error downloading object: " + request.error);
    }
}
```

##### 8.1.3 OBJ Download and Layer Spawning

After the OBJ file is successfully downloaded, it is parsed and placed into the appropriate layer within Unity. Each layer represents different parts of the scene (terrain, buildings, etc.).

For example:
- The OBJ is loaded and applied to a mesh.
- The object is spawned into the correct Unity layer.
- The metadata is downloaded separately and applied to ensure correct placement.

##### 8.1.4 Metadata Downloads and Overwrites

Metadata is critical for placing the downloaded OBJ model accurately in the Unity scene. The metadata is requested as a JSON object from the server and then applied to the model.

```csharp
IEnumerator DownloadMetadata(string url, Action<Metadata> callback)
{
    UnityWebRequest request = UnityWebRequest.Get(url);
    request.SetRequestHeader("Authorization", "Bearer " + bearerToken);
    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.Success)
    {
        Metadata metadata = JsonUtility.FromJson<Metadata>(request.downloadHandler.text);
        callback(metadata);
    }
    else
    {
        Debug.LogError("Error downloading metadata: " + request.error);
    }
}
```

The metadata is applied to the object’s transformation:

```csharp
void ApplyMetadata(GameObject obj, Metadata metadata)
{
    obj.transform.position = metadata.position;
    obj.transform.rotation = Quaternion.Euler(metadata.rotation);
    obj.transform.localScale = metadata.scale;
}
```

#### 8.2 Object Upload

Uploading OBJ models to the server involves sending the object data along with the metadata, using Bearer token authentication.

##### 8.2.1 Implementation of Login Interface

The login flow in Unity WebGL is managed by the `LoginManager.cs` script. Upon login, the user is redirected to a login page where they obtain a Bearer token. The token is then pasted into the application for further authentication.

- `LoginManager.cs` manages the token-based login flow.
- Users are directed to a login page to retrieve the Bearer token.

```csharp
private void OnLoginButtonPressed()
{
    // Open the login page and display the token input field
    Application.OpenURL(loginUrl);
    tokenInputField.gameObject.SetActive(true);
    submitTokenButton.gameObject.SetActive(true);
}
```

Once the token is submitted, the Unity WebGL client checks the server for authentication:

```csharp
private IEnumerator CheckAuthentication(string token)
{
    UnityWebRequest www = UnityWebRequest.Post(authCheckUrl, new WWWForm());
    www.SetRequestHeader("Authorization", $"Bearer {token}");
    yield return www.SendWebRequest();

    if (www.result == UnityWebRequest.Result.Success && www.downloadHandler.text == "authenticated")
    {
        isAuthenticated = true;
        ShowToggleMenuButton();  // Show the menu to upload objects
    }
    else
    {
        Debug.LogError("Authentication failed: " + www.error);
    }
}
```

##### 8.2.2 Object Retrieval and Upload

After the user is authenticated, they can upload OBJ files to the server. The process involves:
1. Selecting an object in the scene.
2. Collecting object data (position, rotation, and scale).
3. Saving the model data as an OBJ file and uploading it.

`SaveToDB.cs` handles the object upload:

```csharp
IEnumerator UploadObject(string url, string objData)
{
    UnityWebRequest request = new UnityWebRequest(url, "POST");
    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(objData);
    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    request.downloadHandler = new DownloadHandlerBuffer();
    request.SetRequestHeader("Authorization", "Bearer " + bearerToken);
    request.SetRequestHeader("Content-Type", "text/plain");

    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.Success)
    {
        Debug.Log("File uploaded successfully!");
    }
    else
    {
        Debug.LogError("Error uploading file: " + request.error);
    }
}
```

##### 8.2.3 Metadata Collection and Upload to Server

Once the OBJ file is uploaded, the metadata is collected and sent to the server using a POST request.

`SaveToDB.cs` includes logic for sending the metadata as JSON:

```csharp
IEnumerator SendDataToDatabase(string jsonMetadata, string objFilePath)
{
    WWWForm form = new WWWForm();
    form.AddField("metadata", jsonMetadata);

    byte[] objFileData = File.ReadAllBytes(objFilePath);
    form.AddBinaryData("objFile", objFileData, Path.GetFileName(objFilePath), "application/octet-stream");

    using (UnityWebRequest www = UnityWebRequest.Post(postUrl, form))
    {
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Data saved to database successfully: " + www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Failed to save data: " + www.error);
        }
    }
}
```

#### 8.3 Authentication

Authentication is managed using Bearer tokens, which are received after the user logs in and used in subsequent requests.

##### 8.3.1 Basic Authentication

Users are first authenticated using basic HTTP authentication to obtain the Bearer token. The token is then used to authenticate future requests.

```csharp
private IEnumerator CheckAuthentication(string token)
{
    UnityWebRequest www = UnityWebRequest.Post(authCheckUrl, new WWWForm());
    www.SetRequestHeader("Authorization", $"Bearer {token}");
    yield return www.SendWebRequest();

    if (www.result == UnityWebRequest.Result.Success && www.downloadHandler.text == "authenticated")
    {
        isAuthenticated = true;
    }
}
```

##### 8.3.2 Bearer Token Usage

Once the Bearer token is obtained, it is included in the headers of future requests, such as object uploads and downloads.

```csharp
request.SetRequestHeader("Authorization", "Bearer " + bearerToken);
```

##### 8.3.3 Authentication Workflow

1. **Login**: Users are directed to the login page and obtain a Bearer token.
2. **Token Submission**: Users paste the token into Unity and submit it for verification.
3. **Token Validation**: The token is sent to the server for validation.
4. **Authenticated Requests**: Upon successful validation, the Bearer token is used in all subsequent requests for uploading/downloading objects and metadata.