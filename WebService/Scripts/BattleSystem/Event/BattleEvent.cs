using System;
using System.Collections.Generic;

namespace BattleSystem
{
    public class BattleEvent : IEvent
    {
        protected struct EventHandlerInfo
        {
            public CallBack<EventParams> Call;
            public sbyte Level;
            public bool IsUseOnce;
        }
        /// <summary>
        /// 事件派发处理程序异常回调
        /// </summary>
        public CallBack<Exception> EventHandlerErrorCallback;
        /// <summary>
        /// 事件集合
        /// </summary>
        protected Dictionary<Enum, List<EventHandlerInfo>> mEventDic;
        /// <summary>
        /// 是否已销毁
        /// </summary>
        public virtual bool IsDisposed { get; protected set; }

        public BattleEvent()
        {
            IsDisposed = false;
            mEventDic = new Dictionary<Enum, List<EventHandlerInfo>>();
        }

        /// <summary>
        /// 回收事件参数
        /// </summary>
        /// <param name="args"></param>
        protected virtual void DisposeArgs(BaseBattle battle, EventParams args)
        {
            if (args != null && args.IsAutoRelease)
            {
                battle.ReleaseEventParam(args);
            }
        }

        /// <summary>
        /// 执行事件错误
        /// </summary>
        protected virtual void EventHandlerError(Exception error, ref EventHandlerInfo eventHandlerInfo)
        {
            if (EventHandlerErrorCallback != null)
                EventHandlerErrorCallback(error);
            else
                throw new Exception("\nEvent Handler Error : " + error + "\n");
        }


        public virtual EventParams InternalDispatchEvent(Enum type, EventParams args)
        {
            args.Target = this;
            if (mEventDic.TryGetValue(type, out var listInfo))
            {
                for (int i = listInfo.Count - 1; i >= 0; i--)
                {
                    if (listInfo.Count == 0)
                        break;//这样写主要是因为配置了事件循环触发，导致事件清理异常，在并行中被移除了
                    if (i > listInfo.Count - 1)
                    {
                        i = listInfo.Count - 1;
                    }
                    EventHandlerInfo info = listInfo[i];
                    if (info.IsUseOnce)
                    {
                        int removeCount = RemoveEvent(type, info.Call);
                        if (removeCount > 1)
                            i -= removeCount - 1;
                    }
                    info.Call(args);
                    if (args.IsBlockEvent)
                    {
                        break;
                    }
                }
            }
            return args;
        }

        /// <summary>
        /// 添加事件
        /// </summary>
        public virtual void AddEvent(Enum type, CallBack<EventParams> handler, bool isUseOnce = false, sbyte level = 0)
        {
            if (!mEventDic.TryGetValue(type, out var listHandler))
            {
                mEventDic[type] = new List<EventHandlerInfo>();
            }
            EventHandlerInfo info = new EventHandlerInfo
            {
                Call = handler,
                Level = level,
                IsUseOnce = isUseOnce
            };
            for (int i = mEventDic[type].Count - 1; i >= 0; i--)
            {
                if (mEventDic[type][i].Level > level)
                {
                    mEventDic[type].Insert(i, info);
                    return;
                }
            }

            mEventDic[type].Add(info);
        }

        /// <summary>
        /// 移除指定事件
        /// </summary>
        public virtual int RemoveEvent(Enum type, CallBack<EventParams> handler)
        {
            if (mEventDic.TryGetValue(type, out var listHandler))
            {
                var rt = listHandler.RemoveAll(a => a.Call == handler);
                if (listHandler.Count == 0)
                {
                    mEventDic.Remove(type);
                }
                return rt;
            }
            return 0;
        }

        /// <summary>
        /// 移除指定类型所有事件
        /// </summary>
        /// <param name="type">类型</param>
        public virtual int RemoveEvent(Enum type)
        {
            if (mEventDic.ContainsKey(type))
            {
                mEventDic.Remove(type);
            }
            return -1;
        }

        /// <summary>
        /// 移除所有事件
        /// </summary>
        public virtual int ClearEvent()
        {
            mEventDic.Clear();
            return -1;
        }

        /// <summary>
        /// 同步派发指定的类型目标事件
        /// </summary>
        /// <param name="args">参数</param>
        public virtual EventParams DispatchEvent(BaseBattle battle, Enum type, EventParams args = null)
        {
            if (args == null)
            {
                args = battle.CreateEventParam();
            }
            EventParams rt = null;
            try
            {
                rt = InternalDispatchEvent(type, args);
            }
            finally
            {
                DisposeArgs(battle, args);
            }

            return rt;
        }

        /// <summary>
        /// 检查是否注册有指定类型事件
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>是/否</returns>
        public virtual bool ContainEvent(Enum type)
        {
            return mEventDic.ContainsKey(type);
        }

        /// <summary>
        /// 检查是否注册有指定类型事件回调
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="handler">回调</param>
        /// <returns>是/否</returns>
        public virtual bool ContainEvent(Enum type, CallBack<EventParams> handler)
        {
            List<EventHandlerInfo> infoList = mEventDic.ContainsKey(type) ? mEventDic[type] : null;
            if (infoList != null)
            {
                for (int i = infoList.Count - 1; i >= 0; i--)
                {
                    if (infoList[i].Call == handler)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public virtual void Dispose()
        {
            if (IsDisposed)
            {
                throw new Exception(this + " Has been destroyed");
            }

            IsDisposed = true;
            EventHandlerErrorCallback = null;
            mEventDic.Clear();
        }
    }
}
