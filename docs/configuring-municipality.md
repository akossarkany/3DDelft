# Delft3D: Documentation 
# 3. Configuring to new municipality


## 3.1 Setting up origin


## 3.2 Area of Interest (AOI)

Since this project is intended for a certain municipality, it is possible to restric the movement of the users to a certain area. This is done by specifying the desired region as a polygon in any GIS software and exporting it as a `geojson` file, named `aoi.geojson`. To apply this area of interes to the project, the `aoi.geojson` have to be uploaded alongside the build files. Read more information on the upload [here](./deployment.md#71-upload-application-files).

### Implementation

The area of interest configureation mananges to restrict the movement by continiusly checking if the position of the Camera Game object is inside the provided polygon. This is done by a simple ray casting algorithm to check the number of intersections between a line throught the position and the polygon. At each frame it is checkd whether the camera is inside the polygon  or not. If it is inside, its positsion is saved. If it is outside, the position of the camera is reverted to the last valid position.

The functionality automatically loads on the startup ofthe appplication. It tries to load the aoi polygon from the same server where the applicaion is hosted, at path `https://server/aoi.geojson`. If there is no such file on the server, the aoi restriction is disabled.


[< Getting Started](./getting-started.md) | [Home](./index.md) | [Next: Loading 2D Layers >](./loading-2D-layers.md)
