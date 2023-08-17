using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Netherlands3D.Coordinates;
using Netherlands3D.Twin.Features;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Netherlands3D.Twin.Configuration
{
    [CreateAssetMenu(menuName = "Netherlands3D/Twin/Configuration", fileName = "Configuration", order = 0)]
    public class Configuration : ScriptableObject, IConfiguration
    {
        [SerializeField] private string title = "Amersfoort";
        [SerializeField] private Coordinate origin = new(CoordinateSystem.RD, 161088, 503050, 300);

        [SerializeField] public List<Feature> Features = new();

        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnTitleChanged.Invoke(value);
            }
        }
        
        public Coordinate Origin
        {
            get => origin;
            set
            {
                origin = value;
                OnOriginChanged.Invoke(value);
            }
        }

        /// <summary>
        /// By default, we should start the setup wizard to configure the twin, unless configuration was successfully
        /// loaded from the URL or from the Configuration File.
        /// </summary>
        public bool ShouldStartSetup { get; set; } = true;

        public UnityEvent<Coordinate> OnOriginChanged = new();
        public UnityEvent<string> OnTitleChanged = new();

        /// <summary>
        /// Overwrites the contents of this Scriptable Object with the serialized JSON file at the provided location.
        /// </summary>
        public IEnumerator PopulateFromFile(string externalConfigFilePath)
        {
            Debug.Log($"Attempting to load configuration from {externalConfigFilePath}");
            using UnityWebRequest request = UnityWebRequest.Get(externalConfigFilePath);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Successfully downloaded external config: {externalConfigFilePath}");
                var json = request.downloadHandler.text;
                
                // populate object and when settings are missing, use the defaults from the provided object
                Populate(JSON.Parse(json));
                ShouldStartSetup = false;
            }
            else
            {
                Debug.LogWarning($"Could not load: {externalConfigFilePath}. Using default config.");
            }

            yield return null;
        }

        public void Populate(Uri url)
        {
            var queryParameters = new NameValueCollection();
            url.Query.ParseAsQueryString(queryParameters);
            Populate(queryParameters);
        }

        public void Populate(NameValueCollection queryParameters)
        {
            if (UrlContainsConfiguration(queryParameters))
            {
                ShouldStartSetup = false;
            }

            var originFromQueryString = queryParameters.Get("origin");
            if (string.IsNullOrEmpty(originFromQueryString) == false)
            {
                LoadOriginFromString(originFromQueryString);
            }

            var featuresFromQueryString = queryParameters.Get("features");
            if (featuresFromQueryString != null)
            {
                LoadFeaturesFromString(featuresFromQueryString);
            }

            foreach (var feature in Features)
            {
                var config = feature.configuration as IConfiguration;
                config?.Populate(queryParameters);
            }
        }

        public string ToQueryString()
        {
            var enabledFeatures = string.Join(
                ',', 
                Features.Where(feature => feature.IsEnabled).Select(feature => feature.Id).ToArray()
            );

            string url = "?";
            url += $"origin={Origin.Points[0]},{origin.Points[1]},{origin.Points[2]}";
            url += $"&features={enabledFeatures}";
            foreach (var feature in Features)
            {
                var featureConfiguration = feature.configuration as IConfiguration;
                if (featureConfiguration == null) continue;

                url += "&" + featureConfiguration.ToQueryString();
            }

            return url;
        }

        public void Populate(JSONNode jsonNode)
        {
            if (jsonNode["title"])
            {
                Title = jsonNode["title"];
            }

            Origin = new Coordinate(
                jsonNode["origin"]["epsg"],
                jsonNode["origin"]["x"],
                jsonNode["origin"]["y"],
                jsonNode["origin"]["z"]
            );
            Debug.Log($"Set origin '{Origin}' from Configuration file");

            foreach (var element in jsonNode["features"])
            {
                var feature = Features.FirstOrDefault(feature => feature.Id == element.Key);
                if (!feature) continue;

                feature.Populate(element.Value);
                if (feature.IsEnabled) Debug.Log($"Enabled feature '{feature.Id}' from Configuration file");
            }
        }

        public JSONNode ToJsonNode()
        {
            var result = new JSONObject
            {
                ["title"] = Title,
                ["origin"] = new JSONObject()
                {
                    ["epsg"] = origin.CoordinateSystem,
                    ["x"] = origin.Points[0],
                    ["y"] = origin.Points[1],
                    ["z"] = origin.Points[2],
                },
                ["features"] = new JSONObject()
            };

            foreach (var feature in Features)
            {
                result["features"][feature.Id] = feature.ToJsonNode();
            }

            return result;
        }

        private bool UrlContainsConfiguration(NameValueCollection queryParameters) 
        {
            string origin = queryParameters.Get("origin");
            string features = queryParameters.Get("features");
            
            return origin != null && features != null;
        }

        private void LoadOriginFromString(string origin)
        {
            var originParts = origin.Split(',');
            int.TryParse(originParts[0].Trim(), out int x);
            int.TryParse(originParts[1].Trim(), out int y);
            int.TryParse(originParts[2].Trim(), out int z);

            Origin = new Coordinate(CoordinateSystem.RD, x, y, z);
            Debug.Log($"Set origin '{Origin}' from URL");
        }

        private void LoadFeaturesFromString(string features)
        {
            var featureIdentifiers = features.ToLower().Split(',');
            foreach (var feature in Features)
            {
                feature.IsEnabled = featureIdentifiers.Contains(feature.Id);
                if (feature.IsEnabled) Debug.Log($"Enabled feature '{feature.Id}' from URL");
            }
        }
    }
}