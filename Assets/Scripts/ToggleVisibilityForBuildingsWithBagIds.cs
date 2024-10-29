using Netherlands3D.SubObjects;
using Netherlands3D.Twin.Projects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Netherlands3D.Twin
{
    public class ToggleVisibilityForBuildingsWithBagIds : MonoBehaviour
    {
        public ColorSetLayer ColorSetLayer { get; private set; } = new ColorSetLayer(0, new());
        private Dictionary<string, Color> buildingColors = new Dictionary<string, Color>();
        private List<string> bagIds = new List<string>()
        {
            "0503100000000209", // Nieuwe Kerk
            "0503100000014169", // Stadhuis
            "0503100000022523", // Oude Kerk
            "0503100000032799", // TU Delft Library

        };

        private void Start()
        {
            SetBuildingIdsToHide(bagIds);
        }

        private void OnEnable()
        {
            SetBuildingColorsHidden(true);
        }

        private void OnDisable()
        {
            SetBuildingColorsHidden(false);
        }

        public void SetBuildingIdsToHide(List<string> ids)
        {
            bagIds = ids;
            buildingColors.Clear();
            foreach (string id in bagIds)
                buildingColors.Add(id, Color.white);
        }

        public void SetBuildingColorsHidden(bool enabled)
        {
            for (int i = 0; i < buildingColors.Count; i++)
            {
                string key = buildingColors.ElementAt(i).Key;
                buildingColors[key] = enabled ? Color.clear : Color.white;
            }

            if (enabled)
                ColorSetLayer = GeometryColorizer.InsertCustomColorSet(-1, buildingColors);
            else
                ColorSetLayer = GeometryColorizer.InsertCustomColorSet(-1, buildingColors);
        }


    }
}