using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Netherlands3D.DB
{
    public class ObjectDB
    {
        private static Dictionary<string, byte[]> DB = new Dictionary<string, byte[]>();

        public static void insert(string key, string value)
        {
            key = clearKey(key);
            DB.Add(key, Encoding.UTF8.GetBytes(value));
        }

        public static string get(string key)
        {
            key = clearKey(key);
            if (DB.ContainsKey(key))
            {
                return DB[key].ToString();
            }
            else
            {
                Debug.LogError("Key not found in the in-memory database");
                return null;
            }
        }

        public static MemoryStream stream(string key)
        {
            key = clearKey(key);
            if (DB.ContainsKey(key))
            {
                return new MemoryStream(DB[key]);
            }
            return null;
        }

        private static string clearKey(string key)
        {
            return key.Split(".")[0];
        }
    }
}
