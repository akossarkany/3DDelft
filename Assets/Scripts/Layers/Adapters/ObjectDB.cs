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

        public static bool contains(string key)
        {
            return DB.ContainsKey(key);
        }

        public static void insert(string key, string value)
        {
            key = clearKey(key);
            if (!DB.ContainsKey(key))
            {
                DB.Add(key, Encoding.UTF8.GetBytes(value));
            }
        }

        public static byte[] get(string key)
        {
            key = clearKey(key);
            if (DB.ContainsKey(key))
            {
                return DB[key];
            }
            else
            {
                Debug.LogError($"Key '{key}' not found in the in-memory database");
                return null;
            }
        }

        public static MemoryStream stream(string key)
        {
            if (ObjectDB.DB.ContainsKey(clearKey(key)))
            {
                return new MemoryStream(DB[key]);
            }
            Debug.LogError("Key '" + key + "' does not exists.");
            return null;
        }

        private static string clearKey(string key)
        {
            return key.Split(".")[0];
        }
    }
}
