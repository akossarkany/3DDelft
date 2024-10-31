using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Netherlands3D.Twin
{
    public class Description : MonoBehaviour
    {
        private string content;
        private bool isMaster;
        public string description {  get { return content; } set { content = value; } }
        public bool master { get { return isMaster; } set { isMaster = value; } }

        public Description(string text)
        {
            content = text;
            isMaster = true;
        }
    }
}
