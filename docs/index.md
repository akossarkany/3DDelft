# 3D-Delft: Documentation 

1. [Introduction](./introduction.md)
2. [Getting Started](./getting-started.md)
3. [Configuring Municipality](./configuring-municipality.md)
4. [Loading 2D Layers](./loading-2D-layers.md)
5. [Making and Loading 3D Tiles](./making-3D-tiles.md)
6. [Loading and Storing OBJ](./loading-obj.md)
7. [Future](./future.md)

1. Introduction
    - Overview of 3D Netherlands
    - Mission/Purpose: Why the project exists and the problems it solves.
    - Framework History & Current state: key developments, version history, and current capabilities

2. Getting Started
    - Prerequisites
    - Downloading the correct version of Unity
    - How to clone 3D Netherlands
    - Add 3D Netherlands as a project in Unity
    - Structure of the 3D Netherlands project in Unity (visual diagram)
        - Functionalities
        - Prefab Spawners and how do they work?
        - Cartesian/3D tiles
        - UI
        - File organization
        -
    - Configuring Streaming Assets with saved project (.nl3d) file

3. Configuring to new municipality
    - Setting the starting viewpoint of the viewer
    - Constraining AIO
        - Setup Polygon layer in QGIS or other GIS software
        - Export it as GeoJSON
        - Put it next to application files

4. Loading 2D layers
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

5. Making and loading 3D tiles
    - What is 3D Tiles?
    - The 3D Netherlands 3D Tiles functionality (why is it useful?)
    - How to import 3D Tiles into the 3D Netherlands (if starting from 2D, else you start with 3D)
    - Add a diagram 2D -> 3D Data -> 3D Tiles -> Import in Unity

6. Loading and storing OBJ
    - Concept
        - Communication framework between client and server (figure)
            - send login request
            - send credentials
            - get bearer token
            - Send obj data
            - Get obj Id
            - send meta data
            - query all entries
            - request each object data
            - callback: request metadata
        - Server: info and structure
            - Flask application
            - connect to apache2 with WSGI
            - connect to DB with psycopg2
    - Server
        - info
            - flask: python package to implement restful servers
            - apache2 for ssl
            - wsgi to connect flask with apache2
        - authentication (/login, /validate)
            - with flask
            - basic auth with uname+pwd
            - Bearer token as session tokens (for now: infinite session)
        - Obj upload/Download
            - Object files: store in local file system; name&id: uuid
            - Metadata
                - JSON Structure (example)
                - Use postgres db
    - Client (Unity WebGL)
        - Object download:
            - Data flow in Unity (i.e. in the WebGL application)
            - Web request flow
            - Obj Download and Layer spawning
            - Meta downloads and overwrites
        - Object upload:
            - Implementation of login interface
            - Object retrieval and upload
            - Metadata collection and upload to server
        - Authentication:
            - Uses basic authentication from HTTP to send uname & pwd
            - Uses the received bearer token when sending future requests
            - Showcase the authentication workflow

7. Future
    - Deploying the application
    - How to keep the project up to date with Netherlands 3D updates


[Pages info](./pages/example/pages.md)