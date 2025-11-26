using System;
using System.Collections.Generic;

namespace BattleSystem
{
    public class AnyObjectPool<T> : IDisposable where T : class, IObjectPool
    {
        //private static DateTime CacheTimestampStart = new DateTime(1970, 1, 1, 0, 0, 0);

        public object locker = new object();
        public HashSet<T> used = new HashSet<T>();
        public Dictionary<Type, RingBuffer<T>> unused = new Dictionary<Type, RingBuffer<T>>();

        public uint limitMaxUsed = 0;
        public CallbackOut<Type, object[], T> customInstance;

        protected virtual T CreateUnusedInstance(Type t, object[] args)
        {
            var rt = customInstance?.Invoke(t, args) ?? (T)Activator.CreateInstance(t, args);
            return rt;
        }


        protected virtual T GetOrCreateUnsed(Type t = null, object[] args = null)
        {
            //lock (locker)
            {
                T temp = null;
                if (unused.TryGetValue(t, out var ringBuffer) && !ringBuffer.IsEmpty())
                {
                    ringBuffer.Pop(out temp);
                }
                if (temp == null)
                {
                    temp = CreateUnusedInstance(t, args);
                }
                used.Add(temp);
                temp.OnAwake();
                return temp;
            }
        }

        //private long LocalTimestamp => (long)(DateTime.UtcNow - CacheTimestampStart).TotalMilliseconds;

        public virtual T Create(object[] args = null) => GetOrCreateUnsed(typeof(T), args);
        public virtual T1 Create<T1>(object[] args = null) where T1 : T => (T1)GetOrCreateUnsed(typeof(T1), args);
        public virtual T Create(Type t, object[] args = null) => GetOrCreateUnsed(t, args);

        /// <summary>
		/// 是否在使用中
		/// </summary>
		public virtual bool IsContains(T obj)
        {
            //lock (locker)
            {
                return used.Contains(obj);
            }
        }

        /// <summary>
        /// 回收
        /// </summary>
        public virtual void Release(T obj)
        {
            //lock (locker)
            {
                if (used.Remove(obj))
                {
                    unused.AddRingBufferItem(obj.GetType(), obj, (int)limitMaxUsed);
                }
                else
                {
                    throw new Exception(obj.ToString() + " not in object pool");
                }
                obj.Dispose();
            }
        }

        /// <summary>
        /// 是否装满
        /// </summary>
        public virtual void Fill()
        {
            //lock (locker)
            {
                //Debug.Assert(limitMaxUsed == 0, "Fill() must set limit max used");
                //{
                //    throw new Exception("ahhahahahhahahahhahahah!!!!");
                //}
            }
        }

        /// <summary>
        ///
        /// </summary>
        public virtual void Dispose()
        {
            //lock (locker)
            {
                unused.Clear();
                foreach (var item in used)
                {
                    item.Dispose();
                }
                used.Clear();
            }
        }
    }
}

