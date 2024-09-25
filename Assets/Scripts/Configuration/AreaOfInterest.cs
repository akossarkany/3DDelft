using Netherlands3D.Coordinates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Netherlands3D.Twin
{
    [CreateAssetMenu(fileName = "AOI", menuName = "ScriptableObjects/AOI")]
    public class AreaOfInterest : ScriptableObject
    {
        public Vector2 min;
        public Vector2 max;

        public bool isInside() 
        {
            var cameraCoordinate = new Coordinate(
                CoordinateSystem.Unity,
                Camera.main.transform.position.x,
                Camera.main.transform.position.y,
                Camera.main.transform.position.z
             );

            var pos = CoordinateConverter.ConvertTo(cameraCoordinate, CoordinateSystem.RDNAP);
            
            Vector3 v = new Vector3(
                (float)pos.Points[0],
                (float)pos.Points[1],
                (float)pos.Points[2]
            );
            return (v.x >= min.x) && (v.y >= min.y) && (v.x <= max.x) && (v.y <= max.y);
        }

    }


}
