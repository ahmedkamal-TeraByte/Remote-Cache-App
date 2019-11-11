using Overlay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;

namespace Cache_Server
{
    class DataManager : ICache
    {
        private Dictionary<string, object> _cacheData = null;

        private Dictionary<string, int> _frequency = null;
        private readonly object _cacheWriteLock = new object();
        private ManualResetEvent _resetEvent = new ManualResetEvent(true);
        private int _maxCount;

        private System.Timers.Timer _timer;


        private static DataManager instance = null;
        private DataManager(EventHandler<CustomEventArgs> handler, int max, int time)
        {
            RaiseEvent += handler;
            _maxCount = max;
            _timer = new System.Timers.Timer(time);

            _timer.Elapsed += StartEviction;
            _timer.Enabled = true;
            //OnRaiseEvent(new CustomEventArgs("Data manager instance created"));
        }

        private void StartEviction(object sender, ElapsedEventArgs e)
        {
            if (_frequency != null)
            {
                if (_frequency.Count > (_maxCount - _maxCount * 20 / 100))
                    OnRaiseEvent(new CustomEventArgs("Eviction Started"));
                while (_frequency.Count > (_maxCount - _maxCount * 20 / 100))
                {
                    var first = _frequency.First();
                    foreach (var item in _frequency)
                    {
                        if (item.Value < first.Value)
                            first = item;
                    }
                    _cacheData.Remove(first.Key);
                    _frequency.Remove(first.Key);
                }
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

            if (_cacheData == null)
            {
                _cacheData = new Dictionary<string, object>();
                _frequency = new Dictionary<string, int>();
                //OnRaiseEvent(new CustomEventArgs("Cache Initialized"));
                _timer.Enabled = true;
            }
        }

        public void Add(string key, object value)
        {
            lock (_cacheWriteLock)
            {
                _cacheData.Add(key, value);
                _frequency.Add(key, 0);
            }
            OnRaiseEvent(new CustomEventArgs("added key:" + key));

        }

        public void Remove(string key)
        {
            lock (_cacheWriteLock)
            {
                _resetEvent.Reset();
                {
                    _cacheData.Remove(key);
                    _frequency.Remove(key);
                }
                _resetEvent.Set();
            }
        }
        public object Get(string key)
        {

            _resetEvent.WaitOne();
            if (_cacheData.TryGetValue(key, out object value))
            {
                _frequency[key]++;
                return value;
            }
            else
            {
                //return "Not Found";
                throw new ArgumentException("Key Value Pair not Found");
            }

        }

        public void Clear()
        {
            lock (_cacheWriteLock)
            {
                _resetEvent.Reset();
                _cacheData.Clear();
                _frequency.Clear();
                _resetEvent.Set();
            }
        }

        public void Dispose()
        {
            Clear();
            _resetEvent.Dispose();
        }
    }
}
