# Netherlands3D Digital Twin

The goal of this project is twofold:

1. To provide a basic, easy-to-use foundation for your local Digital Twin, based on Unity Gaming Technology.
2. To provide a skeleton project for building your own Digital Twin using Netherlands3D based on Unity Gaming Technology.

## Do not commit these files

There are several files that we use as ScriptableObjects that will be changed in while running the editor, but which
should not be changed. These are the following:

- Assets/Scriptables/Projects/CurrentProject.asset

It is recommended to set up your local git repository to ignore changes to these files using the `git update-index`
command, on which you can read more here: https://medium.com/@adi.ashour/dont-git-angry-skip-in-worktree-e9c77dec9d15

The command should be:

```bash
$ git update-index --skip-worktree .\Assets\Scriptables\Projects\CurrentProject.asset
```

At time of writing, no convenient way has been found to use a ScriptableObject for runtime configuration without
this drawback.

# Setting up custom 3D Netherlands Project

## Introduction
## Getting Started
## Configuring the new municipality

## Loading new layers
### WFS
#### What is WFS?
WFS (Web Feature Service) is a powerful web-based protocol designed for accessing and manipulating geographic features over the internet. Unlike WMS which only provides map images, WFS delivers actual geographic data with geometry and attributes, typically in formats like GML or GeoJSON. The service supports essential operations, enabling users to not only retrieve but also edit geographic features. This makes WFS valuable for applications such as downloading geographic data for local analysis, remote feature editing, and creating interactive mapping applications.

#### The 3D Netherlands WFS functionality
It allows users to access and retrieve geospatial vector data (e.g., buildings, terrain) in real time from the 3D Netherlands platform. By using WFS, users can query and download specific datasets, such as building footprints or terrain models, as vector features rather than static images. This functionality enables interactive integration of updated geospatial data into applications like Unity for 3D visualization, analysis, or rendering within custom projects like the 3D Delft basemap.

#### How to import a WFS layer into 3D Netherlands
<img width="1098" alt="LoadWFSLayer1" src="https://github.com/user-attachments/assets/99f8bbb0-a7c8-4cc5-9b81-b64d8882b662">
1. Go to [Netherlands 3D](https://netherlands3d.eu/twin/) and located at the Area of Interest(Delft).

<img width="1100" alt="LoadWFSLayer2" src="https://github.com/user-attachments/assets/daa67c00-23df-4a6a-8db8-061ddd8f89b7">
2. Open lagen, click the plus on the right downside, click importeren and Bestand via URL.

3. Open [Delft Open Data Portal](https://data.delft.nl/search), copy and load GeoJSON links for the layers you want.
<img width="877" alt="LoadWFSLayer4" src="https://github.com/user-attachments/assets/9be96ab7-79cd-4cdb-ac3c-f69e5ad3aa1e">

4. Save the .nl3d file by clicking the down-pointing arrow and the bestand opslaan.

#### Customizing WFS layers in Unity
<img width="1259" alt="image" src="https://github.com/user-attachments/assets/773c9f58-512c-4448-9e7d-906865e0402c">
1. As the project is initialized by the previous steps, you can open "Main Delft" and Click "ProjectDataHandler", and change the Default Project File to the.nl3d file which you saved from the last step.
2. Click the ConfigLoader and enter Play Mode to check if the WFS layers are loaded.
3. Go to File > Building Settings, build and run the project again.

### WMS
#### What is WMS?
WMS (Web Map Service) is a standardized protocol that serves georeferenced map images over the Internet, providing pre-rendered maps in formats like PNG or JPEG. Unlike WFS which returns raw data, WMS focuses on visualization by delivering ready-to-display map images. This makes it ideal for base maps, background layers, and quick visualization.

## Making and loading 3D tiles
## Loading and storing OBJ
## Future
