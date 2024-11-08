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
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Netherlands3D.Twin.Projects;
using System.Linq;
using Netherlands3D.Twin.Layers;
using Netherlands3D.Coordinates;
using System.Security.Cryptography;

public class SaveToDB : MonoBehaviour
{
    [SerializeField] private GameObject modelSpecificationPopup;  // Reference to the popup window
    [SerializeField] private Button toggleMenuButton;  // Button to toggle the popup menu visibility
    [SerializeField] private InputField modelNameInput;
    [SerializeField] private InputField descriptionInput;
    [SerializeField] private Dropdown modelDropdown;  // Assign this in the Inspector
    [SerializeField] private Toggle masterplanToggle;  // Assign this in the Inspector (Toggle for setting as masterplan)
    [SerializeField] private Toggle deleteToggle;  // Assign this in the Inspector (Toggle for setting for deletion)
    [SerializeField] private Text statusMessage; // Text element to display messages to the user

    [SerializeField] private string saveDirectory = "Assets/ImportedOBJ_permanent";  // Directory to save models and metadata
    [SerializeField] private string senObjTo = "https://3ddelft01.bk.tudelft.nl:80/upload/object";  // URL for POST request (replace with actual URL)
    [SerializeField] private string sendMetaTo = "https://3ddelft01.bk.tudelft.nl:80/upload/meta";
    [SerializeField] private string existUrl = "https://3ddelft01.bk.tudelft.nl:80/exists/object";
    [SerializeField] private string removeUrl = "https://3ddelft01.bk.tudelft.nl:80/remove";

    private List<GameObject> selectableModels = new List<GameObject>();  // Store available models
    private bool isPopupVisible = false;  // Track the popup visibility state
    private GameObject selectedModel;
    private static LayerData selectedlayer;
    private string ogLayerName;


    //private string jsonMetadata;
    private string objFilePath;

    private void Start()
    {
        modelSpecificationPopup.SetActive(false); // Initially hide the popup
        toggleMenuButton.gameObject.SetActive(false); // Hide toggle button until sign-in
        toggleMenuButton.onClick.AddListener(TogglePopupMenu);  // Add listener to toggle button
        PopulateModelDropdown();  // Populate model dropdown on start
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
        if (!isPopupVisible) {
            modelNameInput.text = "";
            descriptionInput.text = "";
            masterplanToggle.isOn = false;
        }
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
            masterplanToggle.isOn = false; // Reset masterplan toggle

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

    private IEnumerator remove(string uuid)
    {
        if (LoginManager.token is not string)
        {
            Debug.LogError("No authentication token.");
            yield break;
        }

        string url = Path.Combine(removeUrl, uuid);
        using (UnityWebRequest www = new UnityWebRequest(url, "DELETE"))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Authorization", $"Bearer {LoginManager.token}");
            www.SetRequestHeader("Content-Type", "application/json");
            www.certificateHandler = new BypassCertificateHandler();
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                statusMessage.text = "Model removed successfully!";
                statusMessage.gameObject.SetActive(true);  // Show the status message
                StartCoroutine(ClearStatusMessageAfterDelay(5));
                Debug.Log("Item removed from database successfully.");
            }
            else
            {
                Debug.LogError("Failed to remove object!");
            }
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

        Debug.Log("Delete is " + (deleteToggle.isOn ? "on" : "off"));
        if (deleteToggle.isOn)
        {
            StartCoroutine(remove(selectedlayer.Id));
            return;
        }

        //string modelName = modelNameInput.text;
        string description = descriptionInput.text;

        //// Fetch **world space** position, rotation, and scale from the model's Transform
        //Vector3 worldPosition = selectedModel.transform.position;  // World position
        //Vector3 worldRotation = selectedModel.transform.rotation.eulerAngles;  // World rotation
        //Vector3 worldScale = selectedModel.transform.lossyScale;  // World scale (global)

        //Debug.Log($"Model: {selectedModel.name} | Position: {worldPosition}, Rotation: {worldRotation}, Scale: {worldScale}");

        // Save the model data
        //SaveModelToDirectory(selectedModel, description);
        StartCoroutine(SendFileDataToDatabase(uploadMeta));
        // Show success message
        statusMessage.text = "Model saved successfully!";
        statusMessage.gameObject.SetActive(true);  // Show the status message
        StartCoroutine(ClearStatusMessageAfterDelay(5));  // Hide after 5 seconds
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

    // Raycast for model selection when clicking
    private void SelectModelByClick()
    {
        if (isPopupVisible)
        {
            var selectedLayers = ProjectData.Current.RootLayer.SelectedLayers.ToList();
            if (selectedLayers.Count() != 1)
            {
                Debug.LogError("Only one layer should be selected");
                return;
            }

            selectedlayer = selectedLayers.First();

            if (modelNameInput.text != ogLayerName && modelNameInput.text != "")
            {
                Debug.Log("Cannot overrite modified name. Delete the name before changing it.");
                return;
            }


            modelNameInput.text = selectedlayer.Name;
            ogLayerName = selectedlayer.Name;
            selectedModel = GameObject.Find(selectedlayer.Name);
            if (selectedModel != null)
            {
                Debug.Log($"Found {selectedModel.name}");
                Description d = selectedModel.GetComponent<Description>();
                if (d != null) { 
                    descriptionInput.text = d.description;
                    masterplanToggle.isOn = d.master;
                }
            }
            else
            {
                Debug.LogError($"No Game Object is named {selectedlayer.Name}");
            }
        }
        //Ray ray = Camera.main.ScreenPointToRay(Pointer.current.position.ReadValue());
        //RaycastHit hit;

        //if (Physics.Raycast(ray, out hit, 100000f))
        //{
        //    // Check if the clicked object has a MeshRenderer (assuming models have this component)
        //    if (true || hit.collider != null && hit.collider.gameObject.GetComponent<MeshRenderer>() != null)
        //    {
        //        selectedModel = hit.collider.gameObject;  // Set the selected model
        //        Debug.Log($"Model selected by click: {selectedModel.name}");

        //        // You can also update the UI to show the selected model's name
        //        modelNameInput.text = selectedModel.name;  // Update model name in UI
        //    }
        //}
    }

    private void SelectModelFromLayerUI()
    {
        var selectedLayers = ProjectData.Current.RootLayer.SelectedLayers.ToList();
        if (selectedLayers.Count() > 1)
        {
            Debug.LogError("Only one layer should be selected");
            return;
        }

        selectedlayer = selectedLayers.First();
        modelNameInput.text = selectedlayer.Name;
        selectedModel = GameObject.Find(selectedlayer.Name);
    }

    private Vector3 ConvertToRDNAP(Vector3 vec)
    {
        var truePosition = new Coordinate(CoordinateSystem.Unity, vec.x, vec.y, vec.z);
        var pos = CoordinateConverter.ConvertTo(truePosition, CoordinateSystem.RDNAP);
        return new Vector3((float)pos.Points[0], (float)pos.Points[1], (float)pos.Points[2]);
    }


    // Save the model's metadata and .obj file to a directory
    private void SaveModelToDirectory(GameObject model, string description)
    {
        Debug.Log("Model Type: " + model.name);
        // Ensure the directory exists
        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }

        objFilePath = Path.Combine(saveDirectory, model.name + ".obj");

        // 1. Save the OBJ file (This part requires an OBJ exporter, replace this with actual OBJ saving logic)
        SaveOBJFile(model, objFilePath);

        // 2. Create a metadata object and serialize to JSON

        //File.WriteAllText(jsonFilePath, jsonMetadata, Encoding.UTF8);

        // 3. Optionally, send metadata to a database
        StartCoroutine(SendFileDataToDatabase(uploadMeta));
    }

    // Placeholder for actual OBJ file saving logic
    private void SaveOBJFile(GameObject model, string filePath)
    {
        // You can add your own logic for exporting a model to OBJ here
        Debug.Log("OBJ file saved to: " + filePath);
    }

    private void uploadMeta(string ObjId)
    {
        StartCoroutine(SendMetaDataToDatabase(ObjId));
    }

    private IEnumerator SendMetaDataToDatabase(string ObjId)
    {
        if (LoginManager.token is not string)
        {
            Debug.LogError("No authentication token.");
            yield break;
        }
        selectedlayer.Name = modelNameInput.text;
        selectedModel.name = modelNameInput.text;
        //WWWForm form = new WWWForm();
        //form.AddField("metadata", jsonMetadata);
        var metadata = new BuildingMetadata
        {
            obj_id = ObjId,  // Generate a unique ID
            name = modelNameInput.text,
            description = descriptionInput.text,
            position = ConvertToRDNAP(selectedModel.transform.position),
            rotation = selectedModel.transform.rotation.eulerAngles,
            scale = selectedModel.transform.localScale,
            is_masterplan = masterplanToggle.isOn // Save the toggle state
        };

        // Add the OBJ file to the form
        byte[] objFileMetadata = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(metadata, Formatting.Indented));
        //form.AddBinaryData("objFile", objFileData, Path.GetFileName(objFilePath), "text/plain");

        string url = Path.Combine(sendMetaTo, ObjId);

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(objFileMetadata);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Authorization", $"Bearer {LoginManager.token}");
            www.SetRequestHeader("Content-Type", "application/json");
            www.certificateHandler = new BypassCertificateHandler();
            Debug.Log($"Sending metadata to: {url}");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Metadata saved to database successfully.");
            }
            else
            {
                Debug.LogError("Failed to save metadata: " + www.downloadHandler.text);
            }
        }
    }

    // Send metadata to a database
    private IEnumerator SendFileDataToDatabase(Action<string> onObjUploaded)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(existUrl + selectedlayer.Id))
        { 
            request.certificateHandler = new BypassCertificateHandler();
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onObjUploaded.Invoke(selectedlayer.Id);
                yield break;
            }

        }
        if (LoginManager.token is not string)
        {
            Debug.LogError("No authentication token.");
            yield break;
        }
        //objFilePath = Path.Combine(Application.persistentDataPath, selectedModel.name + ".temp");
        //if (!File.Exists(objFilePath))
        //{
        //    Debug.LogError("File does not exist: " + objFilePath);
        //    yield break;
        //}

        //WWWForm form = new WWWForm();
        //form.AddField("metadata", jsonMetadata);

        // Add the OBJ file to the form
        //byte[] objFileData = File.ReadAllBytes(objFilePath);
        //form.AddBinaryData("objFile", objFileData, Path.GetFileName(objFilePath), "text/plain");

        using (UnityWebRequest www = new UnityWebRequest(senObjTo, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(ObjectDB.get(selectedModel.name));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Authorization", $"Bearer {LoginManager.token}");
            www.SetRequestHeader("Content-Type", "text/plain");
            www.certificateHandler = new BypassCertificateHandler();
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                selectedlayer.Id = www.downloadHandler.text;
                Debug.Log($"Succesfully saved with Id: {www.downloadHandler.text}");
                onObjUploaded.Invoke(www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Failed to save data: " + www.error);
            }
        }
    }

    // Populate the dropdown with selectable objects
    private void PopulateModelDropdown()
    {
        modelDropdown.ClearOptions();  // Clear existing options
        selectableModels.Clear();  // Clear the current list of selectable models

        // Find all selectable objects with a MeshRenderer component (including .temp files)
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        List<string> modelNames = new List<string>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.GetComponent<MeshRenderer>() != null)  // Assuming models have MeshRenderer
            {
                selectableModels.Add(obj);
                modelNames.Add(obj.name);

                // Check for .temp extension in the model name and handle it
                if (obj.name.EndsWith(".temp"))
                {
                    string cleanedName = obj.name.Replace(".temp", ".obj");
                    modelNames[modelNames.Count - 1] = cleanedName;  // Replace the temp extension for display
                    Debug.Log($"Model detected and renamed: {cleanedName}");
                }
            }
        }

        // Populate the dropdown with model names
        modelDropdown.AddOptions(modelNames);

        // Add listener for dropdown selection
        modelDropdown.onValueChanged.AddListener(delegate { SelectModelFromDropdown(); });
    }

    // Select a model from the dropdown
    private void SelectModelFromDropdown()
    {
        int selectedIndex = modelDropdown.value;
        selectedModel = selectableModels[selectedIndex];
        Debug.Log($"Model selected from dropdown: {selectedModel.name}");

        // Update UI input fields
        modelNameInput.text = selectedModel.name;
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
        // Listen for L key to check the selected model
        if (Input.GetKeyDown(KeyCode.L))
        {
            CheckSelectedModel();
        }

        // Listen for K key to list all objects
        if (Input.GetKeyDown(KeyCode.K))
        {
            ListAllSelectableObjects();
        }

        // Raycast for model selection when clicking
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            SelectModelByClick();
        }
    }


    // Custom certificate handler to bypass SSL certificate validation
    private class BypassCertificateHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            // Always return true to bypass SSL certificate validation
            return true;
        }
    }

}

