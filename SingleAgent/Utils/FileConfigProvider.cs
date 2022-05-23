﻿using System.Collections.Generic;
using System.IO;

namespace SingleAgent.Utils
{
    public static class FileConfigProvider
    {
        private static readonly Dictionary<string, object> CachedConfig = new Dictionary<string, object>();
        private static readonly object SyncObj = new object();

        public static bool Exists(string name)
        {
            var file = CommonUtils.ConfigFolder + name + ".json";

            return File.Exists(file);
        }

        public static T Load<T>(string name)
        {
            if (CachedConfig.ContainsKey(name))
            {
                return (T)CachedConfig[name];
            }

            lock (SyncObj)
            {
                if (CachedConfig.ContainsKey(name))
                {
                    return (T)CachedConfig[name];
                }

                var file = CommonUtils.ConfigFolder + name + ".json";
                if (!File.Exists(file))
                {
                    throw new FileNotFoundException("File not found", name);
                }

                var obj = System.Text.Json.JsonSerializer.Deserialize<T>(File.ReadAllText(file));
                CachedConfig.Add(name, obj);
                return obj;
            }
        }

        public static void Update<T>(T value)
        {
            Update(typeof(T).Name, value);
        }

        public static void Update<T>(string name, T value)
        {
            var file = CommonUtils.ConfigFolder + name + ".json";
            lock (SyncObj)
            {
                File.WriteAllText(file, System.Text.Json.JsonSerializer.Serialize(value));

                if (CachedConfig.ContainsKey(name))
                {
                    CachedConfig[name] = value;
                }
                else
                {
                    CachedConfig.Add(name, value);
                }
            }
        }
    }
}
