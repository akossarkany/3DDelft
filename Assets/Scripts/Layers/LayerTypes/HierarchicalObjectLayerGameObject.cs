using System;
using System.Collections.Generic;
using System.Linq;
using Netherlands3D.Coordinates;
using Netherlands3D.Twin.Layers.LayerTypes;
using Netherlands3D.Twin.Layers.Properties;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Netherlands3D.Twin.Layers
{
    public class HierarchicalObjectLayerGameObject : LayerGameObject, IPointerClickHandler, ILayerWithProperties
    {
        private ToggleScatterPropertySectionInstantiator toggleScatterPropertySectionInstantiator;
        [SerializeField] private UnityEvent<GameObject> objectCreated = new();
        private List<IPropertySectionInstantiator> propertySections = new();
        public TransformLayerProperty TransformProperty;
        private Vector3 previousPosition;
        private Quaternion previousRotation;
        private Vector3 previousScale;

        protected void Awake()
        {
            TransformProperty = new TransformLayerProperty
            {
                Position = new Coordinate(CoordinateSystem.Unity, transform.position.x, transform.position.y, transform.position.z),
                EulerRotation = transform.eulerAngles,
                LocalScale = transform.localScale
            };

            LayerData.AddProperty(TransformProperty);
            propertySections = GetComponents<IPropertySectionInstantiator>().ToList();
            toggleScatterPropertySectionInstantiator = GetComponent<ToggleScatterPropertySectionInstantiator>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ClickNothingPlane.ClickedOnNothing.AddListener(OnMouseClickNothing);
            TransformProperty.OnPositionChanged.AddListener(UpdatePosition);
            TransformProperty.OnRotationChanged.AddListener(UpdateRotation);
            TransformProperty.OnScaleChanged.AddListener(UpdateScale);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            TransformProperty.OnPositionChanged.RemoveListener(UpdatePosition);
            TransformProperty.OnRotationChanged.RemoveListener(UpdateRotation);
            TransformProperty.OnScaleChanged.RemoveListener(UpdateScale);
        }

        private void UpdatePosition(Coordinate newPosition)
        {
            if (newPosition.ToUnity() != transform.position)
                transform.position = newPosition.ToUnity();
        }

        private void UpdateRotation(Vector3 newAngles)
        {
            if (newAngles != transform.eulerAngles)
                transform.eulerAngles = newAngles;
        }

        private void UpdateScale(Vector3 newScale)
        {
            if (newScale != transform.localScale)
                transform.localScale = newScale;
        }

        private void Start()
        {
            previousPosition = transform.position;
            previousRotation = transform.rotation;
            previousScale = transform.localScale;

            objectCreated.Invoke(gameObject);
        }

        private void Update()
        {
            // We cannot user transform.hasChanged, because this flag is not correctly set when adjusting this transform using runtimeTransformHandles, instead we have to compare the values directly
            // Check for position change
            if (transform.position != previousPosition)
            {
                var rdCoordinate = new Coordinate(CoordinateSystem.Unity, transform.position.x, transform.position.y, transform.position.z);
                TransformProperty.Position = rdCoordinate;
                previousPosition = transform.position;
            }

            // Check for rotation change
            if (transform.rotation != previousRotation)
            {
                TransformProperty.EulerRotation = transform.eulerAngles;
                previousRotation = transform.rotation;
            }

            // Check for scale change
            if (transform.localScale != previousScale)
            {
                TransformProperty.LocalScale = transform.localScale;
                previousScale = transform.localScale;
            }
        }

        public override void OnLayerActiveInHierarchyChanged(bool isActive)
        {
            if (!isActive && LayerData.IsSelected)
            {
                LayerData.DeselectLayer();
            }

            gameObject.SetActive(isActive);
        }

        private void OnMouseClickNothing()
        {
            if (LayerData.IsSelected)
            {
                LayerData.DeselectLayer();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            LayerData.SelectLayer(true);
        }

        public override void OnSelect()
        {
            var transformInterfaceToggle = FindAnyObjectByType<TransformHandleInterfaceToggle>(FindObjectsInactive.Include); //todo remove FindObjectOfType

            if (transformInterfaceToggle)
                transformInterfaceToggle.SetTransformTarget(gameObject);
        }

        public override void OnDeselect()
        {
            var transformInterfaceToggle = FindAnyObjectByType<TransformHandleInterfaceToggle>(FindObjectsInactive.Include);

            if (transformInterfaceToggle)
                transformInterfaceToggle.ClearTransformTarget();
        }

        public List<IPropertySectionInstantiator> GetPropertySections()
        {
            return propertySections;
        }

        public override void OnProxyTransformParentChanged()
        {
            if (toggleScatterPropertySectionInstantiator.PropertySection != null)
                toggleScatterPropertySectionInstantiator.PropertySection?.TogglePropertyToggle();
        }

        protected override void LoadProperties(List<LayerProperty> layerDataLayerProperties)
        {
            var transformProperty = (TransformLayerProperty)LayerData.LayerProperties.FirstOrDefault(p => p is TransformLayerProperty);
            if (transformProperty != null)
            {
                TransformProperty = transformProperty; //take existing TransformProperty to overwrite the unlinked one of this class
                print("loading properties " );

                UpdatePosition(TransformProperty.Position);
                UpdateRotation(TransformProperty.EulerRotation);
                UpdateScale(TransformProperty.LocalScale);
            }
            else
            {
                print("adding properties " + layerDataLayerProperties.Count);
                LayerData.AddProperty(TransformProperty); //the layer does not yet have a TransformProperty, so add the one of this class to link it to the layerData
            }

        }

        public static ObjectScatterLayerGameObject ConvertToScatterLayer(HierarchicalObjectLayerGameObject objectLayerGameObject)
        {
            print("converting to scatter layer");
            var scatterLayer = new GameObject(objectLayerGameObject.Name + "_Scatter");
            var layerComponent = scatterLayer.AddComponent<ObjectScatterLayerGameObject>();

            var originalGameObject = objectLayerGameObject.gameObject;
            objectLayerGameObject.LayerData.KeepReferenceOnDestroy = true;
            Destroy(objectLayerGameObject); //destroy the component, not the gameObject, because we need to save the original GameObject to allow us to convert back 
            layerComponent.Initialize(originalGameObject, objectLayerGameObject.LayerData.ParentLayer as PolygonSelectionLayer, UnparentDirectChildren(objectLayerGameObject.LayerData));

            return layerComponent;
        }

        private static List<LayerData> UnparentDirectChildren(LayerData layer)
        {
            var list = new List<LayerData>();
            foreach (var child in layer.ChildrenLayers)
            {
                list.Add(child);
            }

            foreach (var directChild in list)
            {
                directChild.SetParent(null);
            }

            return list;
        }
    }
}