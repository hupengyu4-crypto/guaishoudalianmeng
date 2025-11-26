namespace BattleSystem
{
    /// <summary>
    /// 状态事件
    /// </summary>
    public class FighterStateParams : EventParams
    {
        /// <summary>
        /// 改变状态
        /// </summary>
        public BattleObject.State state;
        /// <summary>
        /// 施法者uid
        /// </summary>
        public long authorUid;
        /// <summary>
        /// 目标uid(改变状态的对象)
        /// </summary>
        public long targetUid;
        /// <summary>
        /// 改变状态的对象的阵营
        /// </summary>
        public BattleDef.TeamCampType teamCampType;
        /// <summary>
        /// 来自哪个sid
        /// </summary>
        public long fromSid;
    }
}