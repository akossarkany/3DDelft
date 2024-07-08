using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Netherlands3D.Twin.Layers;
using Netherlands3D.Twin.Projects;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

namespace Netherlands3D.Twin.UI.LayerInspector
{
    public abstract class LayerNL3DBase
    {
        [SerializeField, JsonProperty] private string name;
        [SerializeField, JsonProperty] private bool activeSelf = true;
        [SerializeField, JsonProperty] private Color color = new Color(86f / 256f, 160f / 256f, 227f / 255f);
        [SerializeField, JsonProperty] private LayerNL3DBase parent;
        [SerializeField, JsonProperty] private List<LayerNL3DBase> children = new();
        [JsonIgnore] public bool IsSelected { get; private set; }

        public RootLayer Root => ProjectData.Current.RootLayer; //todo: when creating a layer the root layer reference should be set instead of this static reference
        public LayerNL3DBase ParentLayer => parent;
        public List<LayerNL3DBase> ChildrenLayers => children;

        [JsonIgnore]
        public string Name
        {
            get => name;
            set
            {
                name = value;
                NameChanged.Invoke(value);
            }
        }

        [JsonIgnore]
        public bool ActiveSelf
        {
            get => activeSelf;
            set
            {
                activeSelf = value;
                foreach (var child in ChildrenLayers)
                {
                    child.LayerActiveInHierarchyChanged.Invoke(child.ActiveInHierarchy);
                }

                OnLayerActiveInHierarchyChanged(ActiveInHierarchy);
                LayerActiveInHierarchyChanged.Invoke(ActiveInHierarchy);
            }
        }

        [JsonIgnore]
        public Color Color
        {
            get => color;
            set
            {
                color = value;
                ColorChanged.Invoke(value);
            }
        }

        public int SiblingIndex
        {
            get
            {
                if (ParentLayer == null) //todo: this is only needed for the initial SetParent and should be removed if possible
                    return -1;

                return parent.ChildrenLayers.IndexOf(this);
            }
        }

        public UnityEvent<string> NameChanged = new();
        public UnityEvent<bool> LayerActiveInHierarchyChanged = new();
        public UnityEvent<Color> ColorChanged = new();
        public UnityEvent LayerDestroyed = new();

        public UnityEvent<LayerNL3DBase> LayerSelected = new();
        public UnityEvent<LayerNL3DBase> LayerDeselected = new();

        public LayerUI UI { get; set; } //todo: remove

        public int Depth //todo: remove if possible
        {
            get
            {
                if (ParentLayer != Root)
                    return ParentLayer.Depth + 1;

                return 0;
            }
        }

        public bool ActiveInHierarchy
        {
            get
            {
                if (ParentLayer != null) //todo: if root layer is also of this type, maybe this check is unneeded
                    return ParentLayer.ActiveInHierarchy && activeSelf;

                return activeSelf;
            }
        }

        public virtual void SelectLayer(bool deselectOthers = false)
        {
            if (deselectOthers)
                Root.DeselectAllLayers();

            IsSelected = true;
            Root.AddLayerToSelection(this);
            LayerSelected.Invoke(this);
        }

        public virtual void DeselectLayer()
        {
            IsSelected = false;
            Root.RemoveLayerFromSelection(this);
            LayerDeselected.Invoke(this);
        }

        protected virtual void OnLayerActiveInHierarchyChanged(bool activeInHierarchy)
        {
        }

        public LayerNL3DBase(string name) //todo: replace with constructor when this is no longer a monobehaviour
        {
            Name = name;

            if (this is RootLayer)
                return;

            // if (!LayerData.AllLayers.Contains(this))
            // ProjectData.Current.AddStandardLayer(this);

            //for initialization calculate the parent and children here
            // OnTransformParentChanged();
            // OnTransformChildrenChanged();
            foreach (var child in ChildrenLayers)
            {
                Debug.Log("child: "+child);
                child.UI.SetParent(UI); //Update the parents to be sure the hierarchy matches. needed for example when grouping selected layers that make multiple hierarchy adjustments in one frame
            }
        }

        protected virtual void OnTransformChildrenChanged()
        {
            UI?.RecalculateCurrentTreeStates();
        }

        protected virtual void OnTransformParentChanged()
        {
        }

        protected virtual void OnSiblingIndexOrParentChanged(int newSiblingIndex) //called when the sibling index changes, or when the parent changes but the sibling index stays the same
        {
            UI?.SetParent(ParentLayer?.UI, newSiblingIndex);
            LayerActiveInHierarchyChanged.Invoke(UI?.State == LayerActiveState.Enabled || UI?.State == LayerActiveState.Mixed); // Update the active state to match the calculated state
        }

        public void SetParent(LayerNL3DBase newParent, int siblingIndex = -1)
        {
            Debug.Log("setting parent of: " + Name + " to: " + newParent?.Name);

            if (newParent == null)
                newParent = Root;

            if (newParent == this)
                return;

            var parentChanged = ParentLayer != newParent;
            var oldSiblingIndex = SiblingIndex;

            if (parent != null)
                parent.children.Remove(this);

            if (siblingIndex < 0)
                siblingIndex = newParent.children.Count;

            parent = newParent;
            Debug.Log("new parent: " + newParent);
            Debug.Log("children: " + children);
            newParent.children.Insert(siblingIndex, this);

            if (parentChanged || siblingIndex != oldSiblingIndex)
            {
                OnSiblingIndexOrParentChanged(siblingIndex);
            }
        }

        public virtual void DestroyLayer()
        {
            DeselectLayer();
            ProjectData.Current.RemoveLayer(this);
            foreach (var child in ChildrenLayers)
            {
                child.DestroyLayer();
            }

            LayerDestroyed.Invoke();
        }
    }
}