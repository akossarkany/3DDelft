using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Netherlands3D.Twin
{
    [CreateAssetMenu(fileName = "AOI", menuName = "ScriptableObjects/AOI/Polyline")]
    public class PolyAreaOfInterest : ScriptableObject, AOI
    {
        public bool isInside()
        {
            int c = 0;
            return 0%2 == 1;
        }
    }
}
