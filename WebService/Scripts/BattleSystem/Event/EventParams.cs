using System;

namespace BattleSystem
{
    public class EventParams : IEventParams
    {
        /// <summary>
        /// 事件抛出者
        /// </summary>
        public IEvent Target
        {
            get;
            internal set;
        }

        /// <summary>
        /// 是否阻止事件冒泡
        /// </summary>
        public virtual bool IsBlockEvent
        {
            get;
            set;
        }

        /// <summary>
        /// 对象池自动回收,当事件处理函数执行完成的时候
        /// </summary>
        public virtual bool IsAutoRelease
        {
            get;
            set;
        }

        public virtual void OnAwake()
        {
            IsAutoRelease = true;
            IsBlockEvent = false;
        }

        public virtual void Dispose()
        {
            Target = null;
        }
    }

    public class EventParams<T> : EventParams
    {
        public T data;

        public static EventParams<T> Create(BaseBattle battle, T data)
        {
            var rt = battle.CreateEventParam<EventParams<T>>();
            rt.data = data;
            return rt;
        }

        public override void Dispose()
        {
            base.Dispose();
            data = default(T);
        }
    }

    public class EventTwoParams<T1, T2> : EventParams
    {
        public T1 data1;
        public T2 data2;

        public static EventTwoParams<T1, T2> Create(BaseBattle battle, T1 data1, T2 data2)
        {
            var rt = battle.CreateEventParam<EventTwoParams<T1, T2>>();
            rt.data1 = data1;
            rt.data2 = data2;
            return rt;
        }

        public override void Dispose()
        {
            base.Dispose();
            data1 = default(T1);
            data2 = default(T2);
        }
    }
}

