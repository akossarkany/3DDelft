using System.Collections.Generic;
using System.Linq;
using Netherlands3D.Twin.Layers.LayerTypes;
using Netherlands3D.Twin.Layers.Properties;
using RuntimeHandle;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Netherlands3D.Twin
{
    public class HierarchicalObjectLayer : ReferencedLayer, IPointerClickHandler, ILayerWithProperties
    {
        [SerializeField] private UnityEvent<GameObject> objectCreated = new();
        private List<IPropertySectionInstantiator> propertySections = new();

        public override bool IsActiveInScene
        {
            get => gameObject.activeSelf;
            set
            {
                gameObject.SetActive(value);
                ReferencedProxy.UI.MarkLayerUIAsDirty();
            }
        }
        
        private void OnEnable()
        {
            ClickNothingPlane.ClickedOnNothing.AddListener(OnMouseClickNothing);
        }

        protected override void Awake()
        {
            propertySections = GetComponents<IPropertySectionInstantiator>().ToList();
            base.Awake();
        }

        private void Start()
        {
            objectCreated.Invoke(gameObject);
        }

        private void OnMouseClickNothing()
        {
            if (ReferencedProxy.UI.IsSelected)
            {
                ReferencedProxy.UI.Deselect();
            }
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if(ReferencedProxy.UI == null) return;

            ReferencedProxy.UI.Select(true);
        }
        
        public override void OnSelect()
        {
            var transformHandle = FindAnyObjectByType<RuntimeTransformHandle>(FindObjectsInactive.Include); //todo remove FindObjectOfType
            
            if(transformHandle)
                transformHandle.SetTarget(gameObject);
        }

        public override void OnDeselect()
        {
            var transformHandle = FindAnyObjectByType<RuntimeTransformHandle>(FindObjectsInactive.Include);
            
            if (transformHandle && transformHandle.target == transform)
                transformHandle.SetTarget(transformHandle.gameObject); //todo: update RuntimeTransformHandles Package to accept null 
        }

        public List<IPropertySectionInstantiator> GetPropertySections()
        {
            return propertySections;
        }
    }
}
