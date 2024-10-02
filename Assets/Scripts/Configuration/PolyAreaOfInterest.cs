using Netherlands3D.Coordinates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Netherlands3D.Twin
{
    [CreateAssetMenu(fileName = "AOI", menuName = "ScriptableObjects/AOI/Polyline")]
    public class PolyAreaOfInterest : ScriptableObject
    {
        [SerializeField] public string fileURL;
        [SerializeField] public AOIGeometry geometry = new AOIGeometry();

        public void LoadBoundary(MonoBehaviour coroutineRunner)
        {
            Debug.Log("Initialize boundary");
            if (!geometry.Loaded) coroutineRunner.StartCoroutine(geometry.Load(fileURL));
        }


        public bool isInside()
        {
            if (!geometry.IsValid) return true;
            var cameraCoordinate = new Coordinate(
                CoordinateSystem.Unity,
                Camera.main.transform.position.x,
                Camera.main.transform.position.y,
                Camera.main.transform.position.z
             );
            var pos = CoordinateConverter.ConvertTo(cameraCoordinate, CoordinateSystem.RDNAP);
            Vector2 point = new Vector2(
                (float)pos.Points[0],
                (float)pos.Points[1]
            );

            bool isInside = false;

            // Loop through each edge of the polygon
            for (int i = 0; i < geometry.Boundary.Count - 1; i++)
            {
                Vector2 vi = geometry.Boundary[i]; // Previous vertex
                Vector2 vj = geometry.Boundary[i+1]; // Current vertex

                // Check if the point is inside using the ray-casting algorithm
                if (((vi.y > point.y) != (vj.y > point.y)) &&
                    (point.x < (vj.x - vi.x) * (point.y - vi.y) / (vj.y - vi.y) + vi.x))
                {
                    isInside = !isInside;
                }
            }

            return isInside;
        }

    }
}
