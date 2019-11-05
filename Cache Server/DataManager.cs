using Overlay;
using System;
using System.Collections.Generic;

namespace Cache_Server
{
    class DataManager : ICache
    {
        private Dictionary<string, object> CacheData = null;


        private static DataManager instance = null;
        private DataManager(EventHandler<CustomEventArgs> handler)
        {
            RaiseEvent += handler;
            OnRaiseEvent(new CustomEventArgs("Data manager instance created"));
        }
        public static DataManager GetInstance(EventHandler<CustomEventArgs> handler)
        {
            if (instance == null)
            {
                instance = new DataManager(handler);
            }
            return instance;
        }

        #region event handler
        public event EventHandler<CustomEventArgs> RaiseEvent;

        protected virtual void OnRaiseEvent(CustomEventArgs args)
        {
            if (RaiseEvent != null)
                RaiseEvent(this, args);
        }
        #endregion









        public void Initialize()
        {
            if (CacheData == null)
            {
                CacheData = new Dictionary<string, object>();
                OnRaiseEvent(new CustomEventArgs("Cache Initialized"));

            }
        }

        public void Add(string key, object value)
        {
            if (!CacheData.TryGetValue(key, out _))
                CacheData.Add(key, value);
            //OnRaiseEvent(new CustomEventArgs("added key:" + key));
            //Console.WriteLine("added key:" + key);
        }

        public void Remove(string key)
        {
            CacheData.Remove(key);
        }
        public object Get(string key)
        {
            if (CacheData.TryGetValue(key, out _))
                return CacheData[key];
            return "Not Found";
        }

        public void Clear()
        {
            CacheData.Clear();
        }

        public void Dispose()
        {
            CacheData.Clear();
        }
    }
}
