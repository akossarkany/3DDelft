using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;
using Newtonsoft.Json;

namespace Netherlands3D.Twin
{
    public class ObjDownloadAdapter : MonoBehaviour
    {

        [SerializeField] public string RemoteHost;
        [SerializeField] public string Port;
        [SerializeField] public string QueryEndpoint;
        [SerializeField] public string MetaEndpoint;
        [SerializeField] public string ObjEndpoint;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        IEnumerator LoadRemoteObjects()
        {
            string url = "https://" + this.RemoteHost + ":" + this.Port + "/queryall";
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

                    // iterate ids in list

                    // download each file

                    // load each file aby invoking FileTypeAdapter.ProcessFile


                }
                else
                {
                    Debug.LogError("Failed to query available files. Error: " + request.error);
                }
            }

        }


        IEnumerator DownloadObjFile(string tempPath, string filename)
        {
            string url = "https://" + this.RemoteHost + ":" + this.Port + "/" + this.ObjEndpoint + "/" + filename;
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                // Development only
                request.certificateHandler = new BypassCertificateHandler();

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string objFileContent = request.downloadHandler.text;
                    string tempFilePath = Path.Combine(tempPath, filename + ".obj");

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

        // For development only
        private class BypassCertificateHandler : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData) { return true; }
        }
    }
}