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
1. Go to [Netherlands 3D](https://netherlands3d.eu/twin/) and located at the Area of Interest(Delft)



## Making and loading 3D tiles
## Loading and storing OBJ
## Future
