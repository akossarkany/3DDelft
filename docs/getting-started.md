# 2. Getting Started

## Prerequisites
- If you want to make changes to the structure of the application, you should have a Unity installed. For all other changes within the application, running it in your browser is sufficient.

###  Downloading the Correct Version of Unity
Download the Unity Hub installer from [https://unity.com/download](https://unity.com/download) and install the hub. When prompted, do NOT install a specific Unity version.

### How to Clone Netherlands3D
You can use Git to download the project. Open a terminal window and navigate to the folder where you want to copy the project, then run the following command:

```bash
git clone https://github.com/Netherlands3D/twin.git netherlands
```

This might take some time. It will create a folder containing the project files.

### Requirements for Deployment

During this project we tested the application using a Unix based server. In this documentation we only explain how to deploy the application to a webserver which runs Apache2.

This setup was tested and is working. We cannot guarantee the application will work with other setups.

## Editing the source code in Unity
1. In the Projects tab of Unity, click on the arrow beside "Add," then select "Add project from disk." Navigate to the recently downloaded folder and choose the project folder.  
   ![alt text](./image-4.png)

2. A yellow warning triangle will appear once you add the project. Click on it and then install the missing engine version.
   - **IMPORTANT**: If you do NOT have *Visual Studio Community Edition* installed, make sure to check the respective box under *Dev Tools*.
   - **IMPORTANT**: Also check the box for *WebGL Build Support* under *Platforms*.

3. Once the engine is downloaded, you can open the project.


### Support

Netherlands3D is an onging development. New features are added incrementaly. Therefore, if you want to make changes to the source code, you should consult the [official documentation]() of the project. This documentation dives deaper into the structure of the Netherlands3D unity project and explains the concept behind the features the project implements.

[< Introduction](./introduction.md) | [Home](./index.md) | [Next: Configuring Municipality >](./configuring-municipality.md)

[Pages info](./pages/example/pages.md)