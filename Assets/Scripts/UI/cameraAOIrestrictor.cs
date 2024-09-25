using Netherlands3D.Twin.FloatingOrigin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Netherlands3D.Twin
{
    public class cameraAOIrestrictor : MonoBehaviour
    {
        private Vector2 minLatLon = new Vector2(51.9800f, 4.3369f);  // South-West Corner (latitude, longitude)
        private Vector2 maxLatLon = new Vector2(52.0080f, 4.3930f);  // North-East Corner (latitude, longitude)

        private WorldTransform worldTransform;

        void Start()
        {
            worldTransform = FindObjectOfType<WorldTransform>();

            if (worldTransform == null)
            {
                Debug.LogError("WorldTransform not found!");
            }
        }

        void Update()
        {
            // Check if the camera is within the AOI
            CheckCameraPosition();
        }

        void CheckCameraPosition()
        {
            // this is what is still not working, and what I asked Mike about in the meeting
            var cameraPositionRealWorld = worldTransform.Coordinate(transform.position);


            float cameraLat = cameraPositionRealWorld.latitude;
            float cameraLon = cameraPositionRealWorld.longitude;

            if (IsInsideAOI(cameraLat, cameraLon))
            {
                Debug.Log("Camera is inside the AOI.");
            }
            else
            {
                Debug.Log("Camera is outside the AOI!");
            }
        }

        bool IsInsideAOI(float lat, float lon)
        {
            return lat >= minLatLon.x && lat <= maxLatLon.x &&
                   lon >= minLatLon.y && lon <= maxLatLon.y;
        }
    }
}
