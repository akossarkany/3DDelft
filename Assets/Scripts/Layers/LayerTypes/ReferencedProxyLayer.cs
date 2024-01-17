using System.Collections;
using System.Collections.Generic;
using Netherlands3D.Twin.UI.LayerInspector;
using UnityEngine;

namespace Netherlands3D.Twin
{
    public class ReferencedProxyLayer : LayerNL3DBase
    {
        public ReferencedLayer Reference { get; set; }

        protected override void OnLayerActiveInHierarchyChanged(bool activeInHierarchy)
        {
            Reference.IsActiveInScene = activeInHierarchy;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (Reference)
                Destroy(Reference.gameObject);
        }
    }
}