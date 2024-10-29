using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;

namespace Netherlands3D.Twin
{
    public class AOIGeometry
    {
        private bool loaded;
        private bool isValid;
        private bool isLoading;
        private List<Vector2> boundary;

        public AOIGeometry()
        { 
            this.loaded = false;
            this.isValid = false;
            this.isLoading = false;
            this.boundary = new List<Vector2> ();
        }

        public bool Loaded { get { return this.loaded; } }
        public bool IsValid { get { return this.isValid; } }
        public List<Vector2> Boundary { get { return this.boundary; } }


        public IEnumerator Load(string fileURL)
        {
            Debug.Log("Creating Web Request.");
            this.isLoading = true;
            using (UnityWebRequest File = UnityWebRequest.Get(fileURL))
            {
                // Development only
                var cert = new ForceAcceptAll();
                File.certificateHandler = cert;


                Debug.Log("Requesting AOI.");
                yield return File.SendWebRequest();
                Debug.Log("AOI recieved.");
                if (File.result == UnityWebRequest.Result.Success)
                {
                    // Check if the file has JSON content
                    if (LooksLikeAJSONFile(File.downloadHandler.text))
                    {
                        ParseAOI(File.downloadHandler.text);
                    }
                }
                else
                {
                    Debug.LogError("Failed to load AOI from server. Error: " + File.error);
                }
            }
            this.isLoading = false;
        }

        private void ParseAOI(string file)
        {
            Debug.Log("Reading AOI");
            // Streamread the JSON until we find some GeoJSON properties
            using var reader = new StringReader(file);
            using var jsonReader = new JsonTextReader(reader);

            while (jsonReader.Read())
            {
                if (jsonReader.Value == null)
                {
                    continue;
                }
                if (jsonReader.TokenType == JsonToken.PropertyName)
                {
                    if ((string)jsonReader.Value != "coordinates")
                    {
                        Debug.Log(string.Format("Skipping: {0}: {1}", jsonReader.TokenType, jsonReader.Value));
                    }
                    else
                    {
                        break;
                    }
                }
            }
            Debug.Log("Coodinates:");
            bool isX = true;
            float x = 0.0f;
            while (jsonReader.Read())
            {
                if (jsonReader.TokenType == JsonToken.Float)
                {
                    if (isX)
                    {
                        isX = !isX;
                        x = (float)(double)jsonReader.Value;
                    }
                    else
                    {
                        isX = !isX;
                        this.boundary.Add(new Vector2(x, (float)(double)jsonReader.Value));
                        Debug.Log(string.Format("Added point <{0}, {1}> to the boundary.", x, jsonReader.Value));
                    }
                }
            }
            this.loaded = true;
            this.isValid = this.boundary.Count > 0;

        }

        private bool LooksLikeAJSONFile(string text)
        {
            return text[0] == '{' || text[0] == '[';
        }


    }


    /// <summary>
    /// For development only. Once released the server should have a valid CA certificate.
    /// </summary>
    public class ForceAcceptAll : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}
