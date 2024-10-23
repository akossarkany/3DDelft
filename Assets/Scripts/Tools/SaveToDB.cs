using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Text;
using Newtonsoft.Json;  // Use Unity's JsonUtility if you prefer, or add Newtonsoft.Json via NuGet or Unity Package Manager

public class SaveToDB : MonoBehaviour
{
    [SerializeField] private string saveDirectory = "/upload/object";  // Directory to save models and metadata
    private string postUrl = ""; // POST request URL LOCAL HOST???

    public void SaveModelToDirectory(GameObject model, string modelName, string description, Vector3 localPosition, Vector3 rotation, Vector3 scale)
    {
        // Create directory if it doesn't exist
        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }

        string objFilePath = Path.Combine(saveDirectory, modelName + ".obj");
        string jsonFilePath = Path.Combine(saveDirectory, modelName + ".json");

        // 1. Save OBJ file to directory
        SaveOBJFile(model, objFilePath);

        // 2. Create metadata object and serialize to JSON
        var metadata = new BuildingMetadata
        {
            obj_id = "uuid",
            name = modelName,
            description = description,
            position = new GeoPoint(localPosition.x, localPosition.y, localPosition.z),
            rotation = new GeoPoint(rotation.x, rotation.y, rotation.z),
            scale = new GeoPoint(scale.x, scale.y, scale.z),
            is_masterplan = true
        };

        string jsonMetadata = JsonConvert.SerializeObject(metadata, Formatting.Indented);
        File.WriteAllText(jsonFilePath, jsonMetadata, Encoding.UTF8);

        // 3. Send metadata to database
        StartCoroutine(SendDataToDatabase(jsonMetadata, objFilePath));
    }

    private void SaveOBJFile(GameObject model, string filePath)
    {
        // Add your OBJ exporter logic here to save the model in OBJ format
        Debug.Log("OBJ file saved to: " + filePath);
    }

    private IEnumerator SendDataToDatabase(string jsonMetadata, string objFilePath)
    {
        WWWForm form = new WWWForm();
        form.AddField("metadata", jsonMetadata);

        // Dodanie pliku OBJ do formularza
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

    // Metadata structure
    [System.Serializable]
    public class BuildingMetadata
    {
        public string obj_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public GeoPoint position { get; set; }
        public GeoPoint rotation { get; set; }
        public GeoPoint scale { get; set; }
        public bool is_masterplan { get; set; }
    }

    [System.Serializable]
    public class GeoPoint
    {
        public float x;
        public float y;
        public float z;

        public GeoPoint(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    [System.Serializable]
    public class AdditionalMetadata
    {
        // Add any additional metadata here
    }

    public void OnClickSaveButton()
    {
        GameObject model = GameObject.Find("YourModelName");  // Change to GameObject name, which you want to save
        string modelName = "SampleModel";
        string description = "Description";
        Vector3 localPosition = new Vector3(0, 0, 0);
        Vector3 rotation = new Vector3(0, 0, 0);
        Vector3 scale = new Vector3(1, 1, 1);

        SaveModelToDirectory(model, modelName, description, localPosition, rotation, scale);
    }
}