using System.Collections.Generic;
using System.IO;
using System.Linq;
using Netherlands3D.Twin.Layers.Properties;
using UnityEngine;

namespace Netherlands3D.Twin.Layers
{
    public class ObjDownloadSpawner : MonoBehaviour, ILayerWithPropertyData
    {
        [Header("Required input")] 
        [SerializeField] private Material baseMaterial;
        [SerializeField] private ObjImporter.ObjDownloadImporter importerPrefab;

        [Header("Settings")] 
        [SerializeField] private bool createSubMeshes = false;
        
        private ObjPropertyData propertyData = new();
        public LayerPropertyData PropertyData => propertyData;

        private ObjImporter.ObjDownloadImporter importer;

        private void Start()
        {
            StartImport();
        }

        public void LoadProperties(List<LayerPropertyData> properties)
        {
            var propertyData = properties.OfType<ObjPropertyData>().FirstOrDefault();
            if (propertyData == null) return;

            // Property data is set here, and the parsing and loading of the actual data is done
            // in the start method, there a coroutine is started to load the data in a streaming fashion.
            // If we do that here, then this may conflict with the loading of the project file and it would
            // cause duplication when adding a layer manually instead of through the loading mechanism
            this.propertyData = propertyData;
        }

        private void StartImport()
        {
            DisposeImporter();

            importer = Instantiate(importerPrefab);

            var path = propertyData.ObjFile.LocalPath.TrimStart('/', '\\');
            
            ImportObj(path);
        }

        private void ImportObj(string path)
        {
            // the obj-importer deletes the obj-file after importing.
            // because we want to keep the file, we let the importer read a copy of the file
            // the copying can be removed after the code for the importer is changed

            importer.objFilePath = path;
            importer.mtlFilePath = "";
            importer.imgFilePath = "";

            importer.BaseMaterial = baseMaterial;
            importer.createSubMeshes = createSubMeshes;
            importer.StartImporting(OnObjImported);
        }

        private void OnObjImported(GameObject returnedGameObject)
        {
            returnedGameObject.transform.SetParent(this.transform, false);
            AddLayerScriptToObj(returnedGameObject);

            DisposeImporter();
        }

        private void DisposeImporter()
        {
            if (importer != null) Destroy(importer.gameObject);
        }
        
        private void AddLayerScriptToObj(GameObject parsedObj)
        {
            var spawnPoint = ObjectPlacementUtility.GetSpawnPoint();

            gameObject.transform.position = spawnPoint;

            parsedObj.AddComponent<MeshCollider>();

            // CreatedMoveableGameObject.Invoke(parsedObj);
        }
    }
}