# 2D Layers


**On this page:**
- [Overview](#overview)
- [What is WFS?](#what-is-wfs)
- [What is WMS?](#what-is-wms)
- [The 3D Netherlands WFS Functionality](#the-3d-netherlands-wfs-functionality)
- [How to Import a WFS Layer into 3D Netherlands](#how-to-import-a-wfs-layer-into-3d-netherlands)
- [Adding Layers to the Default Application](#adding-layers-to-the-default-application)

---

On this page, we explain how to load WFS and WMS layers into your `.nl3D` project. The examples provided are from the *Netherlands3D* application; however, since this framework is also used for the *Delft3D* application, the process will be the same in both cases.  
**Note**: WMS functionality is not yet operational in the application but is scheduled to be implemented in early 2025.

#### What is WFS?
**WFS** (Web Feature Service) is a powerful web-based protocol designed for accessing and manipulating geographic features over the internet. Unlike WMS, which only provides map images, WFS delivers actual geographic data with geometry and attributes, typically in formats like GML or GeoJSON. The service supports essential operations, enabling users to not only retrieve but also edit geographic features. This makes WFS valuable for applications such as downloading geographic data for local analysis, remote feature editing, and creating interactive mapping applications.  
For more information on WFS, please see the official OGC website: [https://www.ogc.org/nl/publications/standard/wfs/](https://www.ogc.org/nl/publications/standard/wfs/)

#### What is WMS?
**WMS** (Web Map Service) is a standardized protocol that serves georeferenced map images over the internet, providing pre-rendered maps in formats like PNG or JPEG. Unlike WFS, which returns raw data, WMS focuses on visualization by delivering ready-to-display map images. This makes it ideal for base maps, background layers, and quick visualization.  
For more information on WMS, please see the official OGC website: [https://www.ogc.org/nl/publications/standard/wms/](https://www.ogc.org/nl/publications/standard/wms/)

#### The 3D Netherlands WFS Functionality
This functionality allows users to access and retrieve geospatial vector data (e.g., buildings, terrain) in real time from the *3D Netherlands* platform. By using WFS, users can query and download specific datasets, such as building footprints or terrain models, as vector features rather than static images. This enables the interactive integration of updated geospatial data into applications like Unity for 3D visualization, analysis, or rendering within custom projects like the *3D Delft basemap*.

#### How to Import a WFS Layer into 3D Netherlands

1. Open *Lagen*, click the plus icon at the bottom-right, select *Importeren*, then choose **Bestand via URL** to add a WFS link directly. Alternatively, if you’ve downloaded the WFS layer data, you can upload it by selecting **Eigen Bestand**.  
   **Note**: Currently, the NL3D framework only supports GeoJSON WFS types for both adding a WFS layer via a URL or your own file.

   <img width="1100" alt="LoadWFSLayer2" src="https://github.com/user-attachments/assets/f58e5ddd-f372-45f3-88a4-fe0a49cb220d">

2. Copy the WFS link into the text box to load the WFS layer. For the *Delft3D* project, we primarily used links from the [Delft Open Data Portal](https://data.delft.nl/search).

   ![Loading WFS Layer](image-2.png)

3. To save the current layers in your project, click the down-pointing arrow and select *Bestand Opslaan* to save your `.nl3d` file. You can load this setup again next time by using the load button above the save button.

   <img width="877" alt="Save WFS Layer" src="https://github.com/user-attachments/assets/1c41a9ff-998d-44ee-b726-6a824b18f354">

#### Adding Layers to the Default Application
The previous steps will only add layers for the current user using the viewer. If you have your own application running and want certain layers to be part of your default setup, follow these steps:

1. Save the `.nl3d` file you created in the previous step. Place it in *Your 3D Project* -> *Assets* -> *StreamingAssets*.

2. Open *Main Delft* (or the name of your main scene), go to the *Hierarchy* tab, click on *ProjectDataHandler*, and set the Default Project File to the `.nl3d` file you saved.

3. Go to *File* -> *Build Settings*, and select the scenes you want to include in the *Scenes to Build* box. This usually includes your main scene, the configuration loader, and the setup window.

4. Click *ConfigLoader* and enter Play Mode to check if the WFS layers are loaded.

5. Finally, go to *File* -> *Build Settings*, then build and run the project again.

   ![Adding Layers](https://github.com/user-attachments/assets/773c9f58-512c-4448-9e7d-906865e0402c)

#### Advanced customization

In the **Assets -> Scripts -> Layers -> LayerTypes** folder, you can find three scripts to further work on advanced settings for your WFS layers. These scripts are: **GeoJSONLineLayer**, **GeoJSONPointLayer**, and **GeoJSONPolygonLayer**. They are all subclasses of the **GeoJsonLayerGameObject**. These scripts are also partly or mainly the building blocks of the functionality, so any custom changes can be made here.

These scripts can be used to:
- **Add and Visualize Features**: Adds the features to the visualization GameObject and sets visualizations.
- **Material and Color Management**: For the Delft3D project, this is where we made alterations to the colors for subpolygons.
- **Layer Visibility Control**
- **Out-of-View Feature Removal**
- **Destruction of Visualizations** and how it is handled.

[< Configuring Municipality](./configuring-municipality.md) | [Home](./index.md) | [Next: Making 3D Tiles >](./making-3D-tiles.md)

[Pages info](./pages/example/pages.md)

