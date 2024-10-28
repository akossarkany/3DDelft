using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Netherlands3D.DB;
using System;
using Netherlands3D.Twin;

public class SaveToDB : MonoBehaviour
{
    [SerializeField] private GameObject modelSpecificationPopup;  // Reference to the popup window
    [SerializeField] private Button toggleMenuButton;  // Button to toggle the popup menu visibility
    [SerializeField] private InputField modelNameInput;
    [SerializeField] private InputField descriptionInput;
    [SerializeField] private Text statusMessage; // Text element to display messages to the user

    [SerializeField] private string saveDirectory = "Assets/ImportedOBJ_permanent";  // Directory to save models and metadata
    [SerializeField] private string postUrl = "https://3ddelft01.bk.tudelft.nl:80/upload/object";  // URL for POST request (replace with actual URL)

    private bool isPopupVisible = false;  // Track the popup visibility state
    private GameObject selectedModel;

    private void Start()
    {
        modelSpecificationPopup.SetActive(false); // Initially hide the popup
        toggleMenuButton.gameObject.SetActive(false); // Hide toggle button until sign-in
        toggleMenuButton.onClick.AddListener(TogglePopupMenu);  // Add listener to toggle button
    }

    // This function is called after signing in
    public void OnSignIn()
    {
        // After signing in, show the toggle menu button
        toggleMenuButton.gameObject.SetActive(true);
    }

    // Toggles the popup menu visibility
    private void TogglePopupMenu()
    {
        isPopupVisible = !isPopupVisible;
        modelSpecificationPopup.SetActive(isPopupVisible);  // Toggle the popup's visibility
    }

    // Call this method when the user selects a model
    public void ShowModelSpecificationPopup(GameObject model)
    {
        if (model != null)
        {
            selectedModel = model;
            Debug.Log($"Model selected: {selectedModel.name}");

            // Set the transformation target using TransformHandleInterfaceToggle (if needed)
            var transformHandle = selectedModel.GetComponent<TransformHandleInterfaceToggle>();
            if (transformHandle != null)
            {
                transformHandle.SetTransformTarget(selectedModel);
            }
            else
            {
                Debug.LogWarning("TransformHandleInterfaceToggle component not found on the selected model.");
            }

            // Populate the fields with default values
            modelNameInput.text = selectedModel.name;
            descriptionInput.text = "Enter a description...";

            // Show the popup
            modelSpecificationPopup.SetActive(true);
        }
        else
        {
            Debug.LogError("No model found with the given name.");
            statusMessage.text = "No model selected.";
            statusMessage.gameObject.SetActive(true);
            StartCoroutine(ClearStatusMessageAfterDelay(5)); // Hide the error message after 5 seconds
        }
    }




    // When the user clicks the "Save" button in the popup
    public void OnSaveButtonClick()
    {
        if (selectedModel == null)
        {
            Debug.LogError("No model selected when trying to save.");
            statusMessage.text = "No model selected.";
            statusMessage.gameObject.SetActive(true);  // Show the status message
            StartCoroutine(ClearStatusMessageAfterDelay(5));  // Hide after 5 seconds
            return;
        }

        string modelName = modelNameInput.text;
        string description = descriptionInput.text;

        // Fetch actual position, rotation, and scale from the model's Transform
        Vector3 localPosition = selectedModel.transform.localPosition;
        Vector3 rotation = selectedModel.transform.localRotation.eulerAngles;
        Vector3 scale = selectedModel.transform.localScale;

        // Save the model data
        SaveModelToDirectory(selectedModel, modelName, description, localPosition, rotation, scale);

        // Show success message
        statusMessage.text = "Model saved successfully!";
        statusMessage.gameObject.SetActive(true);  // Show the status message
        StartCoroutine(ClearStatusMessageAfterDelay(5));  // Hide after 5 seconds
    }




    private void SaveObjFileFromMemory(string fileName)
    {
        Debug.Log($"Attempting to retrieve .obj file with key: {fileName}");  // Log the key used for retrieval

        // Retrieve the file from ObjectDB
        string base64FileData = ObjectDB.get(fileName);

        if (base64FileData != null)
        {
            Debug.Log("Successfully retrieved the file from ObjectDB.");  // Log successful retrieval

            byte[] fileBytes = Convert.FromBase64String(base64FileData);
            string destinationPath = Path.Combine("Assets/ImportedOBJ_permanent", fileName);

            // Ensure the destination directory exists
            if (!Directory.Exists("Assets/ImportedOBJ_permanent"))
            {
                Directory.CreateDirectory("Assets/ImportedOBJ_permanent");
                Debug.Log($"Created directory: Assets/ImportedOBJ_permanent");
            }

            // Save the file to the specified directory
            File.WriteAllBytes(destinationPath, fileBytes);
            Debug.Log($"Saved .obj file from memory to: {destinationPath}");  // Log successful save
        }
        else
        {
            Debug.LogError($"Failed to retrieve .obj file from memory for key: {fileName}");
        }
    }



    // Save the model's metadata and .obj file to a directory
    private void SaveModelToDirectory(GameObject model, string modelName, string description, Vector3 localPosition, Vector3 rotation, Vector3 scale)
    {
        // Ensure the directory exists
        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }

        string objFilePath = Path.Combine(saveDirectory, modelName + ".obj");
        string jsonFilePath = Path.Combine(saveDirectory, modelName + ".json");

        // 1. Save the OBJ file (This part requires an OBJ exporter, replace this with actual OBJ saving logic)
        SaveOBJFile(model, objFilePath);

        // 2. Create a metadata object and serialize to JSON
        var metadata = new BuildingMetadata
        {
            obj_id = System.Guid.NewGuid().ToString(),  // Generate a unique ID
            name = modelName,
            description = description,
            position = new Vector3(localPosition.x, localPosition.y, localPosition.z),
            rotation = new Vector3(rotation.x, rotation.y, rotation.z),
            scale = new Vector3(scale.x, scale.y, scale.z),
            is_masterplan = true
        };

        string jsonMetadata = JsonConvert.SerializeObject(metadata, Formatting.Indented);
        File.WriteAllText(jsonFilePath, jsonMetadata, Encoding.UTF8);

        // 3. Optionally, send metadata to a database
        StartCoroutine(SendDataToDatabase(jsonMetadata, objFilePath));
    }

    // Placeholder for actual OBJ file saving logic
    private void SaveOBJFile(GameObject model, string filePath)
    {
        // You can add your own logic for exporting a model to OBJ here
        Debug.Log("OBJ file saved to: " + filePath);
    }

    // Send metadata to a database
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
            }
            else
            {
                Debug.LogError("Failed to save data: " + www.error);
            }
        }
    }

    // Coroutine to clear the status message after a delay
    private IEnumerator ClearStatusMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Clear the text and hide the Status Message
        statusMessage.text = "";
        statusMessage.gameObject.SetActive(false);  // Hide the status message UI element
    }

    // Metadata structure
    [System.Serializable]
    public class BuildingMetadata
    {
        public string obj_id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public Vector3 position { get; set; }
        public Vector3 rotation { get; set; }
        public Vector3 scale { get; set; }
        public bool is_masterplan { get; set; }
    }


    private void Update()
    {
        // Check if the "L" key is pressed for selected object check
        if (Input.GetKeyDown(KeyCode.L))
        {
            CheckSelectedModel();
        }

        // Check if the "K" key is pressed to list all selectable objects
        if (Input.GetKeyDown(KeyCode.K))
        {
            ListAllSelectableObjects();
        }
    }

    // Function to check if a model is selected
    private void CheckSelectedModel()
    {
        if (selectedModel != null)
        {
            Debug.Log($"Selected model: {selectedModel.name}");
        }
        else
        {
            Debug.LogError("No model is currently selected.");
        }
    }

    // Function to list all possible objects for selection
    private void ListAllSelectableObjects()
    {
        // Find all objects with a MeshRenderer component (or use your specific filter)
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        Debug.Log("Listing all selectable objects:");

        foreach (GameObject obj in allObjects)
        {
            if (obj.GetComponent<MeshRenderer>() != null)  // Assuming you want models with MeshRenderer
            {
                Debug.Log($"Selectable Object: {obj.name}");

                // For now, select the first object found (for testing)
                if (selectedModel == null)
                {
                    selectedModel = obj;
                    Debug.Log($"Automatically selected model: {selectedModel.name}");
                }
            }
        }
    }

}






