using System.Collections;
using System.Collections.Generic;

namespace BattleSystem
{
    /// <summary>
    ///  用于弥补C#中foreach迭代器MoveNext效率较低的问题,但注意:移除操作较慢,
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class FastIterationDictionary<TKey, TValue> : IEnumerable
    {
        //返回false则立即结束迭代
        public delegate bool IterationFunc(TKey key, TValue value);
        public delegate bool StaticIterationFunc(TKey key, TValue value, object userData);

        private List<TValue> _values = new List<TValue>();
        private List<TKey> _keys = new List<TKey>();
        private Dictionary<TKey, TValue> _mapView = new Dictionary<TKey, TValue>();

        public List<TKey> Keys
        {
            get { return _keys; }
        }

        public List<TValue> Values
        {
            get { return _values; }
        }

        public int Count
        {
            get { return _values.Count; }
        }

        public TValue this[TKey key]
        {
            get { return _mapView[key]; }
            set
            {
                Remove(key);
                Add(key, value);
            }
        }

        public void Add(TKey key, TValue value)
        {
            _mapView.Add(key, value);
            _keys.Add(key);
            _values.Add(value);
        }

        public void Clear()
        {
            _keys.Clear();
            _mapView.Clear();
            _values.Clear();
        }

        public bool ContainsKey(TKey key)
        {
            return _mapView.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            return _mapView.ContainsValue(value);
        }

        public bool Remove(TKey key)
        {
            TValue value;
            if (_mapView.TryGetValue(key, out value))
            {
                _keys.Remove(key);
                _mapView.Remove(key);
                _values.Remove(value);
                return true;
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _mapView.TryGetValue(key, out value);
        }

        public void Foreach(IterationFunc iterationFunc)
        {
            for (int i = 0; i < _values.Count; i++)
            {
                if (!iterationFunc(_keys[i], _values[i]))
                {
                    break;
                }
            }
        }

        public void Foreach(StaticIterationFunc iterationFunc, object userData)
        {
            for (int i = 0; i < _values.Count; i++)
            {
                if (!iterationFunc(_keys[i], _values[i], userData))
                {
                    break;
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _mapView.GetEnumerator();
        }
    }
}
