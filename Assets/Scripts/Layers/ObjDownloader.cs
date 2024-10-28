using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;
using Netherlands3D.Twin.Layers;

namespace Netherlands3D.Twin
{
    public class ObjDownloader : MonoBehaviour
    {
        [SerializeField] public ObjDownloadAdapter remoteAdapter;
        public List<JsonObjID> jsonObjectList { get; set; }

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(remoteAdapter.LoadRemoteObjects(this));
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnEnable()
        {
            
        }

        public void LoadMetaInformation(string ObjectID, GameObject newLayer)
        {
            Debug.Log("Start reading metadata for '" + ObjectID + "'");
            if (jsonObjectList.Contains(new JsonObjID(ObjectID)))
            {
                StartCoroutine(remoteAdapter.DownloadFileMetadata(ObjectID, newLayer));
            }
        }
    }

    public class JsonObjID
    {
        public string obj_id { get; set; }

        public JsonObjID(string id)
        {
            this.obj_id = id;
        }

        public override bool Equals(object obj)
        {
            if (obj is JsonObjID iD)
            {
                return this.obj_id == iD.obj_id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return obj_id == null ? 0 : obj_id.GetHashCode();
        }

        public override string ToString()
        {
            return this.obj_id;
        }


    }
}
