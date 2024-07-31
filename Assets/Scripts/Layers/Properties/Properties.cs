using System;
using Netherlands3D.Twin.Layers.LayerTypes;
using Netherlands3D.Twin.Projects;
using Netherlands3D.Twin.UI.LayerInspector;
using UnityEngine;

namespace Netherlands3D.Twin.Layers.Properties
{
    public class Properties : MonoBehaviour
    {
        public static Properties Instance { get; private set; }

        [SerializeField] private GameObject card;
        [SerializeField] private RectTransform sections;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                return;
            }

            Destroy(gameObject);
        }

        private void Start()
        {
            Hide();
        }
        
        public void Show(ILayerWithProperties layer)
        {
            card.SetActive(true);
            sections.ClearAllChildren();
            foreach (var propertySection in layer.GetPropertySections())
            {
                propertySection.AddToProperties(sections);
            }
        }

        public void Hide()
        {
            card.gameObject.SetActive(false);
            sections.ClearAllChildren();
        }
        
        public static ILayerWithProperties TryFindProperties(LayerData layer)
        {
            var layerProxy = layer as ReferencedLayerData;

            return (layerProxy == null) ? layer as ILayerWithProperties : layerProxy.Reference as ILayerWithProperties;
        }
    }
}