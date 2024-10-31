# Loading 2D Layers
    - WFS
        - What is WFS?
        - The 3D Netherlands WFS functionality
        - How to import a WFS layer into 3D Netherlands
            - Working data format(s) (so far)
                - GeoJSON
        - Customizing WFS layers in Unity
    - WMS
        - What is WMS?
        - The 3D Netherlands WMS functionality
        - How to import a WMS layer into 3D Netherlands

<--- Add introduction on how 2D layers are added in Netherlands 3D --->

#### What is WFS?
WFS (Web Feature Service) is a powerful web-based protocol designed for accessing and manipulating geographic features over the internet. Unlike WMS which only provides map images, WFS delivers actual geographic data with geometry and attributes, typically in formats like GML or GeoJSON. The service supports essential operations, enabling users to not only retrieve but also edit geographic features. This makes WFS valuable for applications such as downloading geographic data for local analysis, remote feature editing, and creating interactive mapping applications.

#### The 3D Netherlands WFS functionality
It allows users to access and retrieve geospatial vector data (e.g., buildings, terrain) in real time from the 3D Netherlands platform. By using WFS, users can query and download specific datasets, such as building footprints or terrain models, as vector features rather than static images. This functionality enables interactive integration of updated geospatial data into applications like Unity for 3D visualization, analysis, or rendering within custom projects like the 3D Delft basemap.

#### How to import a WFS layer into 3D Netherlands
1. Go to [Netherlands 3D](https://netherlands3d.eu/twin/) and located at the Area of Interest(Delft).
<img width="1098" alt="LoadWFSLayer1" src="https://github.com/user-attachments/assets/df6cf278-0fb7-4e98-a18e-00635cb1cb01">

2. Open lagen, click the plus on the right downside, click importeren and Bestand via URL.
<img width="1100" alt="LoadWFSLayer2" src="https://github.com/user-attachments/assets/f58e5ddd-f372-45f3-88a4-fe0a49cb220d">

3. Open [Delft Open Data Portal](https://data.delft.nl/search), copy and load GeoJSON links for the layers you want.

4. Save the .nl3d file by clicking the down-pointing arrow and the bestand opslaan.
<img width="877" alt="LoadWFSLayer4" src="https://github.com/user-attachments/assets/1c41a9ff-998d-44ee-b726-6a824b18f354">

#### Customizing WFS layers in Unity
1. As the project is initialized by the previous steps, you can open "Main Delft" and Click "ProjectDataHandler", and change the Default Project File to the.nl3d file which you saved from the last step.
2. Click the ConfigLoader and enter Play Mode to check if the WFS layers are loaded.
3. Go to File > Building Settings, build and run the project again.
<img width="1259" alt="image" src="https://github.com/user-attachments/assets/773c9f58-512c-4448-9e7d-906865e0402c">

### WMS test change
#### What is WMS?
WMS (Web Map Service) is a standardized protocol that serves georeferenced map images over the Internet, providing pre-rendered maps in formats like PNG or JPEG. Unlike WFS which returns raw data, WMS focuses on visualization by delivering ready-to-display map images. This makes it ideal for base maps, background layers, and quick visualization.

[< Configuring Municipality](./configuring-municipality.md) | [Home](./index.md) | [Next: Making 3D Tiles >](./making-3D-tiles.md)

[Pages info](./pages/example/pages.md)