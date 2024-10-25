using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Text;
using Newtonsoft.Json;

public class SaveToDB : MonoBehaviour
{
    [SerializeField] private string saveDirectory = "Assets/ImportedOBJ_permanent";  // Directory to save models and metadata
    [SerializeField] private string postUrl = "https://3ddelft01.bk.tudelft.nl:8080"; // CHECK
    [SerializeField] private GameObject modelSpecificationPopup;  // Reference to the popup window
    [SerializeField] private InputField modelNameInput;
    [SerializeField] private InputField descriptionInput;
    [SerializeField] private Text statusMessage; // Text element to display messages to the user

    private GameObject selectedModel;

    private void Start()
    {
        modelSpecificationPopup.SetActive(false); // Hide the popup initially
    }

    // Call this method when the user selects a model
    public void ShowModelSpecificationPopup(GameObject model)
    {
        selectedModel = model;

        // Populate the fields with default values
        modelNameInput.text = model.name;
        descriptionInput.text = "Enter a description...";

        // Show the popup window
        modelSpecificationPopup.SetActive(true);
    }

    // When the user clicks the "Save" button in the popup
    public void OnSaveButtonClick()
    {
        if (selectedModel == null)
        {
            statusMessage.text = "No model selected.";
            return;
        }

        string modelName = modelNameInput.text;
        string description = descriptionInput.text;

        // Fetch actual position, rotation, and scale from the model's Transform
        Vector3 localPosition = selectedModel.transform.localPosition;
        Vector3 rotation = selectedModel.transform.localRotation.eulerAngles;
        Vector3 scale = selectedModel.transform.localScale;

        SaveModelToDirectory(selectedModel, modelName, description, localPosition, rotation, scale);

        // Hide the popup after saving
        modelSpecificationPopup.SetActive(false);
    }

    private void SaveModelToDirectory(GameObject model, string modelName, string description, Vector3 localPosition, Vector3 rotation, Vector3 scale)
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
            obj_id = "uuid",  // Generate a unique ID
            name = modelName,
            description = description,
            position = localPosition,  // Using Vector3 directly
            rotation = rotation,       // Using Vector3 directly
            scale = scale,             // Using Vector3 directly
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
        statusMessage.text = "OBJ file saved to: " + filePath;
    }

    private IEnumerator SendDataToDatabase(string jsonMetadata, string objFilePath)
    {
        WWWForm form = new WWWForm();
        form.AddField("metadata", jsonMetadata);

        // Add the OBJ file to the form
        byte[] objFileData = File.ReadAllBytes(objFilePath);
        form.AddBinaryData("objFile", objFileData, Path.GetFileName(objFilePath), "application/octet-stream");

        using (UnityWebRequest www = UnityWebRequest.Post(postUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Data saved to database successfully: " + www.downloadHandler.text);
                statusMessage.text = "Data saved successfully.";
            }
            else
            {
                Debug.LogError("Failed to save data: " + www.error);
                statusMessage.text = "Error: Failed to save data.";
            }
        }
    }

    // Metadata structure using Vector3
    [System.Serializable]
    public class BuildingMetadata
    {
        public string obj_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public Vector3 position { get; set; }  // Use Vector3 for position
        public Vector3 rotation { get; set; }  // Use Vector3 for rotation
        public Vector3 scale { get; set; }     // Use Vector3 for scale
        public bool is_masterplan { get; set; }
    }
}
