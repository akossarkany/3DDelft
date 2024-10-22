using System.IO;
using Netherlands3D.Twin.Layers;
using Netherlands3D.Twin.Layers.Properties;
using Netherlands3D.Twin.Projects;
using UnityEngine;
using UnityEngine.Events;

namespace Netherlands3D.Twin
{
    [CreateAssetMenu(menuName = "Netherlands3D/Adapters/OBJDownloadImportAdapter", fileName = "OBJDownloadImportAdapter", order = 0)]
    public class ObjDownloadImportAdapter : ScriptableObject, IDataTypeAdapter
    {
        [SerializeField] private ObjSpawner layerPrefab;

        [SerializeField] private UnityEvent<string, ObjSpawner> loadMetaData;

        public bool Supports(LocalFile localFile)
        {
            return localFile.LocalFilePath.EndsWith(".obj");
        }

        public void Execute(LocalFile localFile)
        {
            var fullPath = localFile.LocalFilePath;
            var fileName = Path.GetFileName(fullPath);
            ObjSpawner newLayer = Instantiate(layerPrefab);
            newLayer.gameObject.name = fileName;

            var propertyData = newLayer.PropertyData as ObjPropertyData;
            propertyData.ObjFile = AssetUriFactory.CreateProjectAssetUri(fullPath);

            if (this.loadMetaData != null)
            {
                loadMetaData.Invoke(fileName.Split(".")[0], newLayer);
            }

        }
    }
}