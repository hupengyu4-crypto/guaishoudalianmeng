namespace BattleSystem
{
    public interface IEventParams : IObjectPool
    {
        /// <summary>
        /// 是否阻塞事件
        /// </summary>
        bool IsBlockEvent
        {
            get;
            set;
        }
    }
}
