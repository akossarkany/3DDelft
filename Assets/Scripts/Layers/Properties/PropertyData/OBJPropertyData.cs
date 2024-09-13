using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Events;

namespace Netherlands3D.Twin.Layers.Properties
{
    public class OBJPropertyData : LayerPropertyData, ILayerPropertyDataWithAssets
    {
        [SerializeField, JsonProperty] private Uri data;

        [JsonIgnore] public readonly UnityEvent<Uri> OnDataChanged = new();

        [JsonIgnore]
        public Uri Data
        {
            get => data;
            set
            {
                data = value;
                OnDataChanged.Invoke(value);
            }
        }

        public IEnumerable<LayerAsset> GetAssets()
        {
            return new List<LayerAsset>()
            {
                new (this, data != null ? data : null)
            };
        }
    }
}
