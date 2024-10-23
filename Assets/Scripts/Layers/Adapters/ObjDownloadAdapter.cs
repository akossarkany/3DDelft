using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;
using System;
using UnityEngine.Events;
using Netherlands3D.Twin.Layers;
using Netherlands3D.Coordinates;
using Netherlands3D.DB;

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
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                // Development only
                request.certificateHandler = new BypassCertificateHandler();

                Debug.Log("Sending request to: " + url);

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Object list received: " + request.downloadHandler.text);
                    // load response as json
                    caller.jsonObjectList = JsonConvert.DeserializeObject<List<JsonObjID>>(request.downloadHandler.text);
                    Debug.Log("Object amount: " + caller.jsonObjectList.Count.ToString());

                    // iterate ids in list
                    foreach (JsonObjID jsonObj in caller.jsonObjectList)
                    {
                        Debug.Log(jsonObj.ToString());
                        if (jsonObj != null)
                        {
                            Debug.Log("Requesting object: " + jsonObj.obj_id);
                            caller.StartCoroutine(DownloadObjFile(jsonObj.obj_id));
                        }
                        else
                        {
                            Debug.LogError("Json object was null.");
                        }

                        
                    }

                }
                else
                {
                    Debug.LogError("Failed to query available files. Error: " + request.error);
                }
            }

        }


        IEnumerator DownloadObjFile(string fileName)
        {
            string url = "https://" + this.RemoteHost + ":" + this.Port + "/" + this.ObjEndpoint + "/" + fileName;
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                // Development only
                request.certificateHandler = new BypassCertificateHandler();

                Debug.Log("Requesting file: " + url);

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string objFileContent = request.downloadHandler.text;

                    ObjectDB.insert(fileName, objFileContent);
                    Debug.Log("File with key: '" + fileName + "' inserted successfully.");
                    Debug.Log("File received and saved successfully.");
                    // Spawn new game object. the name of the object will be the file name
                    fileAdapter.Invoke(fileName + ".download");
                }
                else
                {
                    Debug.LogError("Failed to download file: " + request.error);
                }
            }
        }

        public IEnumerator DownloadFileMetadata(string ObjectID, GameObject newLayer)
        {
            string url = "https://" + this.RemoteHost + ":" + this.Port + "/" + this.MetaEndpoint + "/" + ObjectID;
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                // Development only
                request.certificateHandler = new BypassCertificateHandler();
                Debug.Log("Requesting metadat for file: " + ObjectID);

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Metadata received. Processing...");
                    JsonMetaData metadata = JsonConvert.DeserializeObject<List<JsonMetaData>>(request.downloadHandler.text)[0];


                    newLayer.name = metadata.name;


                    var truePosition = new Coordinate(
                        CoordinateSystem.RDNAP,
                        metadata.position.x,
                        metadata.position.y,
                        metadata.position.z
                     );
                    var pos = CoordinateConverter.ConvertTo(truePosition, CoordinateSystem.Unity);
                    Vector3 v = new Vector3(
                        (float)pos.Points[0],
                        (float)pos.Points[1],
                        (float)pos.Points[2]
                    );
                    newLayer.transform.position = v;
                    newLayer.transform.rotation = Quaternion.Euler(metadata.rotation);
                    newLayer.transform.localScale = metadata.scale;
            
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