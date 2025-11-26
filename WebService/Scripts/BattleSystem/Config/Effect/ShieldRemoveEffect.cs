namespace BattleSystem
{
    /// <summary>
    /// 护盾移除
    /// </summary>
    public class ShieldRemoveEffect : BaseEffect
    {
        /// <summary>
        /// 移除护盾百分比(1-100)
        /// </summary>
        public double value;
        /// <summary>
        /// 星级加成
        /// </summary>
        public double starValue;
        /// <summary>
        /// 等级加成
        /// </summary>
        public double lvValue;

        /// <summary>
        /// 施法者
        /// </summary>
        private Fighter mAuthor;


        public override void OnBegin()
        {
            mAuthor = (Fighter)author;
            base.OnBegin();
        }

        public override void Trigger()
        {
            var fighter = (Fighter)target;
            fighter.RemoveShieldByPercentage(value + mAuthor.Data.Star * starValue + mAuthor.Data.Level * lvValue);
        }

        public override void OnEnd()
        {
            mAuthor = null;
            base.OnEnd();
        }
    }
}