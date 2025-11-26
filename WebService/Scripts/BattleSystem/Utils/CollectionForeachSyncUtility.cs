using System;
using System.Collections;
using System.Collections.Generic;

namespace BattleSystem
{
    /// <summary>
    /// 用于foreach的缓存工具, 解决的问题是: 再循环列表或者字典期间, 又对他进行修改, 不会影响这次的循环
    /// </summary>
    public static class CollectionForeachSyncUtility
    {
        /// <summary>
        /// 
        /// </summary>
        public class Collection<T> : IEnumerable<T>, IEnumerator<T>
        {
            public static Collection<T> Inst = new Collection<T>();
            public T[] Arr = new T[16];
            public int mCurIndex, mMaxIndex;

            public T Current
            {
                get
                {
                    var tmp = Arr[mCurIndex];
                    Arr[mCurIndex] = default; // 用于实现让这个工具不对源对象进行引用, 反正都是foreach, 用一次就没用了的
                    return tmp;
                }
            }

            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                mCurIndex++;
                return mCurIndex < mMaxIndex;
            }

            public void Reset()
            {
                mMaxIndex = mCurIndex = -1;
            }

            public IEnumerator<T> GetEnumerator() => this;

            IEnumerator IEnumerable.GetEnumerator() => this;

            public Collection<T> CopyFrom(ICollection<T> collection)
            {
                if (Arr.Length < collection.Count)
                {
                    Array.Resize(ref Arr, FindNextPowerOfTwo(collection.Count));
                }
                collection.CopyTo(Arr, 0);
                mCurIndex = -1;
                mMaxIndex = collection.Count;
                return this;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int FindNextPowerOfTwo(int v)
        {
            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v++;
            return v;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static Collection<T> Foreach<T>(ICollection<T> collection)
        {
            return Collection<T>.Inst.CopyFrom(collection);
        }
    }
}
