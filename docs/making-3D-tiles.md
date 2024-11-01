# 5. Creating 3D Tiles

#### What is 3D Tiles?
3D Tiles is an open standard developed by the Open Geospatial Consortium (OGC) designed for streaming and rendering massive amounts of 3D geospatial data. A 3D Tiles dataset, referred to as a tileset, organizes 3D data into a hierarchical structure, allowing for efficient visualization and analysis. This standard is applicable to various types of 3D data, including city models, point clouds, and photogrammetry.

For more detailed information, you can refer to the [OGC 3D Tiles Standard](https://www.ogc.org/publications/standard/3dtiles/#:~:text=A%203D%20Tiles%20data%20set%2C%20called%20a%20tileset%2C,and%20applicable%20to%20various%20types%20of%203D%20data) and [OGC Document on 3D Tiles](https://docs.ogc.org/cs/22-025r4/22-025r4.html).

With 3D Tiles functionality in Netherlands3D, users can upload, navigate and explore complex urban models in real-time, making it easier to perform spatial analyses, and conduct urban planning. The hierarchical structure of 3D Tiles allows for on-demand loading, which means that only the necessary level of detail is rendered based on the user's viewpoint.

 
#### Converting 2D GIS Vegetation Data to 3D Models Using FME

*Disclaimer: There are multiple approaches to converting 2D GIS data to 3D models in CityGML format. The following method is one approach, using FME and following the CityGML creation workflow from Safe Software.*

The Feature Manipulation Engine (FME) was used to transform 2D GIS data for vegetation, specifically for elements like bushes and grass, into 3D models in the CityGML format. This process is adapted from Safe Software, which provides an approach to creating solitary vegetation objects in 3D. For further guidance, see Safe Software’s [CityGML Solitary Vegetation Object Creation Guide](https://hub.safe.com/publishers/con-terra-lab/templates/citygml-create-solitaryvegetationobject-with-implicit-representation#description).

Our FME workbench:
![alt text](images/FME_2D_3D.JPG)

Our input (2D points of Trees managed by Delft Municipality):
![alt text](images/Step_1_Add%20Source%20Data%20Containing%20Tree%20Positions.JPG)

Our Output (3D Models of Vegetation in CityGML) result:
![alt text](images/Result_CityGML.JPG)


#### Create 3D Tiles
*Disclaimer: There are multiple approaches to creating 3D Tiles. The following method is one of the approahces using FME. For more information regarding 3D Tiles resources, refer to [this page](https://github.com/CesiumGS/3d-tiles/blob/main/RESOURCES.md).*



#### Importing 3D Tiles into the project
The 3D Tiles should be hosted on a server to be able to be loaded into the project. 
Please refer to [Netherlands3D documentation](https://netherlands3d.eu/docs/developers/introduction/) for future updates on working with 3D Tiles for this project.

- sth about `tileset.json` and screenshots of Netherlands3D

[< Loading 2D layers](./loading-2D-layers.md) | [Home](./index.md) | [Next: Loading and Storing OBJ>](./loading-obj.md)

[Pages info](./pages/example/pages.md)