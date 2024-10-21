using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;
using System;
using UnityEngine.Events;
using Netherlands3D.Twin.Layers;

namespace Netherlands3D.Twin
{
    [CreateAssetMenu(menuName = "Netherlands3D/Adapters/ObjDownloadAdapter", fileName = "ObjDownloadAdapter")]
    public class ObjDownloadAdapter : ScriptableObject
    {

        [SerializeField] public string RemoteHost;
        [SerializeField] public string Port;
        [SerializeField] public string QueryEndpoint;
        [SerializeField] public string MetaEndpoint;
        [SerializeField] public string ObjEndpoint;
        [SerializeField] public UnityEvent<string> fileAdapter = new();


        public IEnumerator LoadRemoteObjects(ObjDownloader caller)
        {
            string url = "https://" + this.RemoteHost + ":" + this.Port + "/" + QueryEndpoint;
            string tempPath = Path.GetTempPath();
            tempPath = Path.Combine(tempPath, "objects");
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                // Development only
                request.certificateHandler = new BypassCertificateHandler();

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    // load response as json
                    caller.jsonObjectList = JsonConvert.DeserializeObject<List<JsonObjID>>(request.downloadHandler.text);

                    // iterate ids in list
                    foreach (JsonObjID jsonObj in caller.jsonObjectList)
                    {
                        if (jsonObj != null)
                        {
                            Debug.Log("Requesting object: " + jsonObj.obj_id);
                            caller.StartCoroutine(DownloadObjFile(tempPath, jsonObj.obj_id));
                        }
                        Debug.LogError("Json object was null.");


                        // Spawn new game object. the name of the object will be the file name
                        fileAdapter.Invoke(Path.Combine(tempPath, jsonObj.obj_id + ".obj"));
                    }

                }
                else
                {
                    Debug.LogError("Failed to query available files. Error: " + request.error);
                }
            }

        }


        IEnumerator DownloadObjFile(string tempPath, string fileName)
        {
            string url = "https://" + this.RemoteHost + ":" + this.Port + "/" + this.ObjEndpoint + "/" + fileName;
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                // Development only
                request.certificateHandler = new BypassCertificateHandler();

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string objFileContent = request.downloadHandler.text;
                    string tempFilePath = Path.Combine(tempPath, fileName + ".obj");

                    try
                    {
                        File.WriteAllText(tempFilePath, objFileContent);
                        Debug.Log("File saved to: " + tempFilePath);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Failed to save file: " + e.Message);
                    }
                }
                else
                {
                    Debug.LogError("Failed to download file: " + request.error);
                }
            }
        }

        public IEnumerator DownloadFileMetadata(string ObjectID, ObjSpawner newLayer)
        {
            string url = "https://" + this.RemoteHost + ":" + this.Port + "/" + this.MetaEndpoint + "/" + ObjectID;
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                // Development only
                request.certificateHandler = new BypassCertificateHandler();

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    JsonMetaData metadata = JsonConvert.DeserializeObject<List<JsonMetaData>>(request.downloadHandler.text)[0];
                    newLayer.gameObject.name = metadata.name;
                    newLayer.gameObject.transform.position = metadata.position;
                    newLayer.gameObject.transform.rotation = Quaternion.Euler(metadata.rotation);
                    newLayer.gameObject.transform.localScale = metadata.scale;
                }
                else
                {
                    Debug.LogError("Failed to download metadata for file. " + request.error);
                }
            }
        }


        // For development only
        private class BypassCertificateHandler : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData) { return true; }
        }

        private class JsonMetaData
        {
            public string obj_id;
            public string name;
            public string description;
            public Vector3 position;
            public Vector3 rotation;
            public Vector3 scale;
            public bool is_masterplan;
            
        }


    }
}