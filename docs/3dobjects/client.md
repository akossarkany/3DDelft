# Client (Unity WebGL)

This section focuses on the Unity WebGL application, detailing how it interacts with the Flask server for downloading, uploading, and managing metadata of OBJ files. It also covers the login and authentication workflow using Bearer tokens.

## Object Download

In the Unity WebGL client, OBJ files are requested from the server and rendered in the Unity scene. The download process includes web requests, object download, and metadata management to ensure correct object placement.

### Data Flow in Unity

The data flow in Unity starts when a user requests an OBJ file, which is fetched via a `UnityWebRequest`. Once the request is completed, the file is parsed and placed into the scene.

The main flow:
- A GET request is sent to retrieve the OBJ file. Once its received, it is stored in memory for further processing
- The obj data is processed using a modified verion of Netherland3D's objet reader.
- Once a game object is created for the downloaded obj file, the metadata, such as position and scale, is also downloaded and applied.

### Web Request Flow

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

### OBJ Download and Layer Spawning

After the OBJ file is successfully downloaded, it is parsed and placed into the appropriate layer within Unity. Each layer represents different parts of the scene (terrain, buildings, etc.).

For example:
- The OBJ is loaded and applied to a mesh.
- The object is spawned into the correct Unity layer.
- The metadata is downloaded separately and applied to ensure correct placement.

### Metadata Downloads and Overwrites

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

## Object Upload

Uploading OBJ models to the server involves sending the object data along with the metadata, using Bearer token authentication.

### Implementation of Login Interface

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

### Object Retrieval and Upload

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

### Metadata Collection and Upload to Server

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

## Authentication

Authentication is managed using Bearer tokens, which are received after the user logs in and used in subsequent requests.

### Basic Authentication

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

### Bearer Token Usage

Once the Bearer token is obtained, it is included in the headers of future requests, such as object uploads and downloads.

```csharp
request.SetRequestHeader("Authorization", "Bearer " + bearerToken);
```

### Authentication Workflow

1. **Login**: Users are directed to the login page and obtain a Bearer token.
2. **Token Submission**: Users paste the token into Unity and submit it for verification.
3. **Token Validation**: The token is sent to the server for validation.
4. **Authenticated Requests**: Upon successful validation, the Bearer token is used in all subsequent requests for uploading/downloading objects and metadata.