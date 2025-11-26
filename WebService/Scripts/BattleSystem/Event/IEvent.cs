using System;

namespace BattleSystem
{
    public interface IEvent : IDisposable
    {
        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <param name="handler">回调</param>
        /// <param name="isUseOnce">是否一次性，不填默认不是</param>
        /// <param name="level">优先级，默认0</param>
        void AddEvent(Enum type, CallBack<EventParams> handler, bool isUseOnce = false, sbyte level = 0);
        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <param name="handler">回调</param>
        /// <returns></returns>
        int RemoveEvent(Enum type, CallBack<EventParams> handler);
        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <returns></returns>
        int RemoveEvent(Enum type);
        /// <summary>
        /// 清理事件
        /// </summary>
        /// <returns></returns>
        int ClearEvent();

        /// <summary>
        /// 派发事件
        /// </summary>
        /// <param name="battle">战场</param>
        /// <param name="type">枚举类型</param>
        /// <param name="args">参数</param>
        /// <returns>结果</returns>
        EventParams DispatchEvent(BaseBattle battle, Enum type, EventParams args);
        /// <summary>
        /// 匹配事件
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <returns>是/否</returns>
        bool ContainEvent(Enum type);
        /// <summary>
        /// 匹配事件
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <param name="handler">回调</param>
        /// <returns>是/否</returns>
        bool ContainEvent(Enum type, CallBack<EventParams> handler);
    }
}

