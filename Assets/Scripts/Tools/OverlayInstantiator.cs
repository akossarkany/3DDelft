using Netherlands3D.Twin.Layers;
using UnityEngine;

namespace Netherlands3D.Twin.UI.LayerInspector
{
    public class OverlayInstantiator : MonoBehaviour
    {
        [SerializeField] private OverlayInspector overlayPrefab;
        
        [Header("(Optional)")]
        [SerializeField] private ReferencedLayer referencedLayer;
        [SerializeField] private bool instantiateOnStart = false;
        
        private void Start() 
        {   
            if(instantiateOnStart)
                InstantiateOverlay(true);
        }

        public void InstantiateOverlay(bool clearExistingContent = true)
        {
            var spawnedOverlay = ContentOverlayContainer.Instance.ShowOverlay(overlayPrefab, clearExistingContent);

            if(referencedLayer != null)
                spawnedOverlay.SetReferencedLayer(referencedLayer);
        }
    }
}
