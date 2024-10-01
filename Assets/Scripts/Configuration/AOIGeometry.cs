using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace Netherlands3D.Twin
{
    public class AOIGeometry
    {
        private bool loaded;
        private bool isValid;
        private List<Vector2> boundary;

        public AOIGeometry()
        { 
            this.loaded = false;
            this.isValid = false;
            this.boundary = new List<Vector2> ();
        }

        public bool Loaded { get { return this.loaded; } }
        public bool IsValid { get { return this.isValid; } }
        public List<Vector2> Boundary { get { return this.boundary; } }

        public void Load(string localFile)
        {
            // Check if the file has JSON content
            if (!LooksLikeAJSONFile(localFile))
                return;

            Debug.Log("Reading AOI");
            // Streamread the JSON until we find some GeoJSON properties
            using var reader = new StreamReader(localFile);
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
            return;

        }

        private bool LooksLikeAJSONFile(string filePath)
        {
            using var reader = new StreamReader(filePath);
            var firstChar = reader.Read();
            return firstChar == '{' || firstChar == '[';
        }


    }
}
