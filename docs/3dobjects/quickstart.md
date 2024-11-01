# ObjUpload Quick Start

OBJ uploading and saving functionality is restricted to administrators. To access this functionality, administrators must log in using credentials and retrieve a bearer token for authentication.

Steps 1 and 2 can be performed in any order.

## Step 1. Import and Position OBJ Models

Users can import an OBJ model into the scene, preview, and manipulate its properties, including position, scale, and rotation.

The imported OBJ model will automatically be textured with a default material. This material renders both sides of each mesh face, meaning the orientation of normals does not affect the rendering.

### Navigate to the **Lagen** (Layers) Tab

On the left side of the UI, you will find vertically stacked icons. Open the tab labeled **Lagen** to manage layers.

<img src="./images/lagen.png" alt="Lagen Tab" width="720">

### Click the **+** Icon to Import Data

At the bottom of the **Lagen** tab, you will find a plus icon (`+`) for adding new data.

<img src="./images/plus.png" alt="Plus Icon" width="720">


### Select **Importeren** and **Eigen Bestand** to Load Your OBJ Model

From the import menu, select **Importeren** (Import) and then choose **Eigen Bestand** (Your Own File) to upload your OBJ model.

<img src="./images/importmenu.png" alt="Import Menu" width="720">


### Upload Your OBJ File

After selecting your file, it will be uploaded and displayed in the Unity viewer.

<img src="./images/objupload.png" alt="Import OBJ" width ="720">

### Position the OBJ Model Correctly

Once uploaded, position your OBJ model within the scene by adjusting its position, rotation, and scale. These settings can be manipulated directly in the viewer.

---

## Step 2. Log In

To begin uploading OBJ files, administrators must authenticate by logging in.

### Click on the **Sign In** Button

In the top-right corner of the viewer UI, there is a **Sign In** button.

<img src="./images/signInButton.png" alt="Sign In Button" width ="720">

### Enter Your Username and Password

A pop-up window will appear, prompting you to enter your administrator credentials.

<img src="./images/loginPrompt.png" alt="Login Prompt" width ="360">

### Retrieve Your Bearer Token

After successful authentication, the system will generate a bearer token that looks like: `155f1caf-288f-4e9e-9e64-6eef09e60bb1`.

<img src="./images/bearerToken.png" alt="Bearer Token Example" width ="420">

### Copy and Paste the Token

Copy the generated bearer token and paste it into the designated input field within the Unity viewer.

<img src="./images/tokenInserted.png" alt="Token Input Field" width ="720">

### Access the **Save** Dropdown Menu

Once logged in successfully, the **Sign In** button will change to a **Save** dropdown menu, allowing you to save your OBJ model to the database.

<img src="./images/saveMenu.png" alt="Save Dropdown Menu" width ="720">

---

## Step 3. Save the OBJ Model to the Database

After positioning the OBJ model, follow these steps to save it to the database:

### Select the Model in the Layer Tab

Click on the model within the layer tab to select it.

<img src="./images/modelSelection.png" alt="Model Selection" width ="720">

### Review or Edit the Model's Name

The model's name will be automatically filled in but can be edited if necessary.

### Add a Description and Indicate **Masterplan** Status

Provide a description for the model and indicate whether it is part of a **Masterplan** by checking the corresponding box.

### Press **Save**

After filling in the required information, press the **Save** button to send the model and its metadata (position, rotation, scale) to the PostgreSQL database.

<img src="./images/save.png" alt="Save Button" width ="360">

Upon saving, the model will be accessible in future sessions, with its metadata securely stored in the database.

