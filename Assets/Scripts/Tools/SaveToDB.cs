using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Text;
using Newtonsoft.Json;  // Use Unity's JsonUtility if you prefer, or add Newtonsoft.Json via NuGet or Unity Package Manager

public class SaveToDB : MonoBehaviour
{
    [SerializeField] private string saveDirectory = "Assets/ImportedOBJ_permanent";  // Directory to save models and metadata
    private string postUrl = "https://your-database-api-url.com/api/save"; // POST request URL

    public void SaveModelToDirectory(GameObject model, string modelName, string description, Vector3 localPosition, Vector3 rotation, Vector3 scale, Vector3 rdnPosition)
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
            name = modelName,
            description = description,
            location = new GeoPoint(rdnPosition.x, rdnPosition.y, rdnPosition.z),
            local_x = localPosition.x,
            local_y = localPosition.y,
            local_z = localPosition.z,
            rotation_x = rotation.x,
            rotation_y = rotation.y,
            rotation_z = rotation.z,
            scale_x = scale.x,
            scale_y = scale.y,
            scale_z = scale.z,
            obj_file_path = objFilePath,
            rdn_x = rdnPosition.x,
            rdn_y = rdnPosition.y,
            rdn_z = rdnPosition.z,
            additional_metadata = new AdditionalMetadata() // Add additional data if necessary
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
        form.AddField("objFilePath", objFilePath);

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
        public string name;
        public string description;
        public GeoPoint location;
        public float local_x;
        public float local_y;
        public float local_z;
        public float rotation_x;
        public float rotation_y;
        public float rotation_z;
        public float scale_x = 1.0f;
        public float scale_y = 1.0f;
        public float scale_z = 1.0f;
        public string obj_file_path;
        public float rdn_x;
        public float rdn_y;
        public float rdn_z;
        public AdditionalMetadata additional_metadata;
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
        string description = "Opis modelu";
        Vector3 localPosition = new Vector3(0, 0, 0);
        Vector3 rotation = new Vector3(0, 0, 0);
        Vector3 scale = new Vector3(1, 1, 1);
        Vector3 rdnPosition = new Vector3(0, 0, 0);

        SaveModelToDirectory(model, modelName, description, localPosition, rotation, scale, rdnPosition);
    }
}
