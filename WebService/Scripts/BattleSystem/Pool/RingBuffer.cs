using System;
using System.Collections.Generic;

namespace BattleSystem
{
    public class RingBuffer<T>
    {
        private const int _DEFAULT_LENGTH = 32;
        private readonly List<T> _buffer;
        private readonly int _capacity;
        private int _startIndex;
        private int _endIndex;
        private bool _pushed;

        public RingBuffer() : this(_DEFAULT_LENGTH)
        {
        }

        public RingBuffer(int capacity)
        {
            _capacity = capacity;
            if (_capacity <= 0)
            {
                _capacity = _DEFAULT_LENGTH;
            }
            _buffer = new List<T>(2);
        }

        // public void SetCapacity(int capacity)
        // {
        //     _capacity = capacity;
        // }

        public bool IsFull()
        {
            return _pushed && _startIndex == _endIndex;
        }

        public bool IsEmpty()
        {
            return !_pushed;
        }

        public int Count()
        {
            if (IsEmpty())
            {
                return 0;
            }

            if (IsFull())
            {
                return _capacity;
            }

            var endIndex = _endIndex;
            while (endIndex < _startIndex)
            {
                endIndex += _capacity;
            }

            return endIndex - _startIndex;
        }

        public bool Push(T obj)
        {
            if (IsFull())
            {
                return false;
            }

            _pushed = true;
            var bufferCount = _buffer.Count;
            if (_endIndex >= bufferCount)
            {
                for (int i = bufferCount; i <= _endIndex; i++)
                {
                    _buffer.Add(default(T));
                }
            }
            _buffer[_endIndex] = obj;

            _endIndex += 1;
            while (_endIndex >= _capacity)
            {
                _endIndex -= _capacity;
            }

            return true;
        }

        public bool Pop(out T obj)
        {
            obj = default(T);
            if (IsEmpty())
            {
                return false;
            }

            if (_buffer.Count <= _startIndex)
            {
                return false;
            }

            obj = _buffer[_startIndex];
            _buffer[_startIndex] = default(T);
            _startIndex += 1;
            while (_startIndex >= _capacity)
            {
                _startIndex -= _capacity;
            }

            if (_startIndex == _endIndex)
            {
                Clear();
            }

            return true;
        }

        public void Clear()
        {
            _buffer.Clear();
            _pushed = false;
            _startIndex = 0;
            _endIndex = 0;
        }
    }
}

