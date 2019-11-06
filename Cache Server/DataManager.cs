using Overlay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Cache_Server
{
    class DataManager : ICache
    {
        private Dictionary<string, object> CacheData = null;

        private Dictionary<string, int> _frequency = null;

        private int _maxCount;
        private int _evictionTime;

        private Timer _timer;


        private static DataManager instance = null;
        private DataManager(EventHandler<CustomEventArgs> handler, int max, int time)
        {
            RaiseEvent += handler;
            _maxCount = max;
            _timer = new Timer(time);
            _timer.Elapsed += StartEviction;
            _evictionTime = time;
            OnRaiseEvent(new CustomEventArgs("Data manager instance created"));
        }

        private void StartEviction(object sender, ElapsedEventArgs e)
        {
            //var First = _frequency.First();
            while (_frequency.Count > (_maxCount - _maxCount * 20 / 100))
            {
                var first = _frequency.First();
                foreach (var item in _frequency)
                {
                    if (item.Value < first.Value)
                        first = item;
                }
                CacheData.Remove(first.Key);
                _frequency.Remove(first.Key);
            }
        }

        public static DataManager GetInstance(EventHandler<CustomEventArgs> handler, int max, int time)
        {
            if (instance == null)
            {
                instance = new DataManager(handler, max, time);
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
                _frequency = new Dictionary<string, int>();
                OnRaiseEvent(new CustomEventArgs("Cache Initialized"));

            }
        }

        public void Add(string key, object value)
        {
            if (!CacheData.TryGetValue(key, out _))
            {
                CacheData.Add(key, value);
                _frequency.Add(key, 0);
            }
            OnRaiseEvent(new CustomEventArgs("added key:" + key));
        }

        public void Remove(string key)
        {
            if (CacheData.TryGetValue(key, out _))
            {
                CacheData.Remove(key);
                _frequency.Remove(key);
            }
        }
        public object Get(string key)
        {
            if (CacheData.TryGetValue(key, out _))
            {
                _frequency[key]++;
                return CacheData[key];
            }
            return "Not Found";
        }

        public void Clear()
        {
            CacheData.Clear();
            _frequency.Clear();
        }

        public void Dispose()
        {
            CacheData.Clear();
            _frequency.Clear();
        }
    }
}
