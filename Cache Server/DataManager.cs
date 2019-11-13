using Overlay;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Cache_Server
{
    class DataManager : ICache
    {
        private Dictionary<string, object> _cacheData = null;

        private Dictionary<string, int> _frequency = null;
        private SortedList<int, List<string>> _sortedList = null;
        private readonly object _cacheLock = new object();
        //private ManualResetEvent _resetEvent = new ManualResetEvent(true);
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
            Evict();
        }


        private void Evict()

        {
            Console.WriteLine("Size before eviction:" + _cacheData.Count);

            int target = _maxCount - _maxCount * 20 / 100;
            if (_frequency != null)
            {
                lock (_cacheLock)
                {
                    //_resetEvent.Reset();
                    if (_frequency.Count > target)
                        OnRaiseEvent(new CustomEventArgs("Eviction Started"));
                    int iterator = 0;
                    try
                    {
                        while (_frequency.Count > target)
                        {
                            int checker = 0; //used to delete 10 items at a time
                            for (int i = _sortedList[iterator].Count - 1; i >= 0; i--) // if the inner list has value. then it will iterate over it
                            {
                                var key = _sortedList[iterator][i];

                                if (checker < 10 || _frequency.Count > target) // 
                                {
                                    if (checker >= 10) //we have deleted 10 items but target is still to be met.
                                        checker = 0;        //set checker to 0 and delete 10 more
                                    _cacheData.Remove(key);
                                    _frequency.Remove(key);
                                    _sortedList[iterator].Remove(key);
                                    checker++;
                                }
                            }
                            iterator++;

                            //var first = _frequency.First();
                            //foreach (var item in _frequency)
                            //{
                            //    if (item.Value < first.Value)
                            //        first = item;
                            //}
                            //_cacheData.Remove(first.Key);
                            //_frequency.Remove(first.Key);
                        }
                    }
                    catch (KeyNotFoundException)
                    { }
                    catch (Exception ex)
                    {
                        OnRaiseEvent(new CustomEventArgs(ex.Message));
                        throw ex;
                    }
                    Console.WriteLine("Size after eviction:" + _cacheData.Count);
                    //_resetEvent.Set();
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
                _sortedList = new SortedList<int, List<string>>();
                //OnRaiseEvent(new CustomEventArgs("Cache Initialized"));
                _timer.Enabled = true;
            }
        }

        public void Add(string key, object value)
        {
            if (_cacheData.Count >= _maxCount)
                Evict();

            lock (_cacheLock)
            {
                _cacheData.Add(key, value);
                _frequency.Add(key, 0);
                if (_sortedList.ContainsKey(0))
                    _sortedList[0].Add(key);
                else
                    _sortedList.Add(0, new List<string> { key });
            }
            OnRaiseEvent(new CustomEventArgs("added key:" + key));

        }

        public void Remove(string key)
        {
            lock (_cacheLock)
            {
                //_resetEvent.Reset();
                {
                    _cacheData.Remove(key);
                    if (_frequency.ContainsKey(key))
                        _sortedList[_frequency[key]].Remove(key);
                    _frequency.Remove(key);
                }
                //_resetEvent.Set();
            }
        }
        public object Get(string key)
        {

            //_resetEvent.WaitOne();
            if (_cacheData.TryGetValue(key, out object value))
            {
                lock (_cacheLock)
                {
                    int old = _frequency[key];
                    //Console.WriteLine("frequency before updation" + old + " at SL:" + _sortedList[old].Contains(key));

                    int newFrequency = ++_frequency[key]; //frequency after updation


                    _sortedList[old].Remove(key); //removes the key from old frequency index

                    if (_sortedList.ContainsKey(newFrequency)) // if there's already a list at new index
                        _sortedList[newFrequency].Add(key); // adds the key to new frequency index

                    else // if there's no list at new index
                        _sortedList.Add(newFrequency, new List<string> { key }); // create a new key and a new list and add key in that new list

                }
                //Console.WriteLine("frequency after updation" + _frequency[key] + " at SL:" + _sortedList[newFrequency].Contains(key));

                //Console.WriteLine("is key at old frequency??:"+ _sortedList[old].Contains(key));

                return value;
            }
            else
            {
                return null;
            }

        }

        public void Clear()
        {
            lock (_cacheLock)
            {
                //_resetEvent.Reset();
                _cacheData.Clear();
                _sortedList.Clear();
                _frequency.Clear();
                //_resetEvent.Set();
            }
        }

        public void Dispose()
        {
            lock (_cacheLock)
            {
                _cacheData = null;
                _frequency = null;
                _sortedList = null;
            }
            //_resetEvent.Dispose();
        }
    }
}
