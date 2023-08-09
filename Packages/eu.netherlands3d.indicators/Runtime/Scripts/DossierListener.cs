using GeoJSON.Net.Feature;
using Netherlands3D.Indicators.Dossiers;
using UnityEngine;
using UnityEngine.Events;

namespace Netherlands3D.Indicators
{
    public class DossierListener : MonoBehaviour
    {
        [SerializeField] private DossierSO Dossier;
        
        public UnityEvent<Dossier> onOpen = new();
        public UnityEvent<Variant?> onSelectedVariant = new();
        public UnityEvent onFailedToOpen = new();
        public UnityEvent onClose = new();
        public UnityEvent<FeatureCollection> onLoadedProjectArea = new();

        private void OnEnable()
        {
            Dossier.onOpen.AddListener(OnOpen);
            Dossier.onClose.AddListener(OnClose);
            Dossier.onSelectedVariant.AddListener(OnSelectedVariant);
            Dossier.onFailedToOpen.AddListener(OnFailedToOpen);
            Dossier.onLoadedProjectArea.AddListener(OnLoadedProjectArea);
        }

        private void OnDisable()
        {
            Dossier.onOpen.RemoveListener(OnOpen);
            Dossier.onClose.RemoveListener(OnClose);
            Dossier.onSelectedVariant.RemoveListener(OnSelectedVariant);
            Dossier.onFailedToOpen.RemoveListener(OnFailedToOpen);
        }

        private void OnOpen(Dossier dossier)
        {
            onOpen.Invoke(dossier);
        }

        private void OnFailedToOpen()
        {
            onFailedToOpen.Invoke();
        }

        private void OnSelectedVariant(Variant? variant)
        {
            onSelectedVariant.Invoke(variant);
        }

        private void OnClose()
        {
            onClose.Invoke();
        }

        private void OnLoadedProjectArea(FeatureCollection featureCollection)
        {
            onLoadedProjectArea.Invoke(featureCollection);
        }
    }
}