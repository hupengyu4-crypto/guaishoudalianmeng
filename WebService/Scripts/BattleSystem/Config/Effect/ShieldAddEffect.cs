
namespace BattleSystem
{
    /// <summary>
    /// 护盾添加
    /// <para>护盾最新设计是永久的，对应的叠加、效果结束后移除代码已经被废弃，重新需要找svn记录</para>
    /// </summary>
    public class ShieldAddEffect : BaseEffect
    {
        /// <summary>
        /// 参照目标类型
        /// </summary>
        public BattleDef.TargetType targetType;
        /// <summary>
        /// 参照属性
        /// </summary>
        public BattleDef.Property property;
        /// <summary>
        /// 属性系数(百分比)
        /// </summary>
        public double[] factor;
        /// <summary>
        /// 星级加成
        /// </summary>
        public double starValue;
        /// <summary>
        /// 护盾为特殊护盾
        /// </summary>
        public bool specialShield; 

        // /// <summary>
        // /// 等级加成
        // /// </summary>
        // public double lvValue;

        /// <summary>
        /// 施法者
        /// </summary>
        private Fighter mAuthor;
        /// <summary>
        /// 目标
        /// </summary>
        private Fighter mTarget;
        /// <summary>
        /// 提供护盾值
        /// </summary>
        private long mChangeValue;

        public override void Trigger()
        {
            mAuthor = (Fighter)author;
            mTarget = (Fighter)target;
            var tempTarget = targetType == BattleDef.TargetType.Self ? mAuthor : mTarget;
            var cfgFactor = 0.0d;
            if (BattleUtils.GetLevelValue(factor, effectLevel, out var cfgValue))
            {
                cfgFactor = cfgValue;
            }
            // var starFactor = factor * BattleDef.Percent * mAuthor.Data.Star * starValue;
            // mChangeValue = (long)YKMath.Ceiling(tempTarget.Data[property] * starFactor + mAuthor.Data.Level * lvValue);
            mChangeValue = (long)YKMath.Ceiling(tempTarget.Data[property] * (cfgFactor + mAuthor.Data.Star * starValue) * BattleDef.Percent);
            mTarget.AddShield(mChangeValue, specialShield);
        }

        public override void OnOverlayAdd()
        {
            if (IsType(EffectSys.EffectType.Overlays) && overlayCount < addMax)
            {
                mTarget.AddShield(mChangeValue, specialShield);
            }
            base.OnOverlayAdd();
        }

        public override void OnOverlayReduce()
        {
            base.OnOverlayReduce();
            if (!isEnd && IsType(EffectSys.EffectType.Overlays) && overlayCount >= 0)
            {
                mTarget.AddShield(-mChangeValue, specialShield);
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();
            mAuthor = null;
            mTarget = null;
        }
    }
}