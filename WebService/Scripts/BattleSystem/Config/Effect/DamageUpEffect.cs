#if UNDEF_BATTLE_UNITY_EDITOR
#undef UNITY_EDITOR
#endif

using System.ComponentModel;

namespace BattleSystem
{
    /// <summary>
    /// 伤害提升效果
    /// </summary>
    public class DamageUpEffect : BaseEffect
    {
        /// <summary>
        /// 参照目标类型
        /// </summary>
        public BattleDef.TargetType targetType;

        /// <summary>
        /// 伤害提升类型
        /// </summary>
        public DamageUpEffect.UpType upType;

        /// <summary>
        /// 系数
        /// </summary>
        public double factor;

        /// <summary>
        /// 结束时是否保留
        /// </summary>
        public bool isRetain;

        /// <summary>
        /// 影响始终生效的额外伤害系数
        /// </summary>
        public bool always;

        /// <summary>
        /// 施法者
        /// </summary>
        private Fighter mAuthor;

        /// <summary>
        /// 目标
        /// </summary>
        private Fighter mTarget;

        /// <summary>
        /// 改变的值
        /// </summary>
        private double mAdd;

        public override void OnBegin()
        {
            mAuthor = (Fighter)author;
            mTarget = (Fighter)target;
            mAdd = 0;
            base.OnBegin();
        }

        public override void Trigger()
        {
            DamageChange();
        }

        /// <summary>
        /// 改变伤害加成
        /// </summary>
        /// <param name="valueAdd">增加为1，减少为-1，只是相对的当配置负数，增加就会反过来是减少，减少就是增加，不同参照目标的理解</param>
        private void DamageChange(int valueAdd = 1)
        {
            var tempTarget = targetType == BattleDef.TargetType.Self ? mAuthor : mTarget;
            double changeValue = 0; //(long)YKMath.Ceiling(tempTarget.Data[property] * factor * BattleDef.Percent);
            switch (upType)
            {
                case UpType.Star:
                    changeValue = tempTarget.Data.Star * factor;
                    break;
                default:
                    changeValue = factor;
                    //Log.error($"DamageUpEffect({sid})中配置了{upType},但未实现对应的增加逻辑,联系配置和程序");
                    break;
            }

            changeValue *= valueAdd;
            if (always)
            {
                mTarget.AddDamageFactorAlways(changeValue);
            }
            else
            {
                mTarget.AddDamageFactor(changeValue);
            }

            mAdd += changeValue;

#if UNITY_EDITOR
            battle.AddInfo(mTarget.GetBaseDesc()).AddInfo("DamageUpEffect-").AddInfo(sid).AddInfo("生效,[")
                .AddInfo(upType).AddInfo("]额外伤害系数改变:")
                .AddInfo(mTarget.GetDamageFactor()).AddInfo(">>")
                .AddInfo(mTarget.GetDamageFactor() + changeValue, true);
#endif
        }

        public override void OnOverlayAdd()
        {
            if (overlayCount < addMax)
            {
                DamageChange();
            }

            base.OnOverlayAdd();
        }

        public override void OnOverlayReduce()
        {
            base.OnOverlayReduce();
            if (!isEnd && overlayCount >= 0)
            {
                DamageChange(-1);
            }
        }

        public override void OnEnd()
        {
            if (!isRetain)
            {
                if(always)
                {
                    mTarget.AddDamageFactorAlways(-mAdd);
                }
                else
                {
                    mTarget.AddDamageFactor(-mAdd);
                }
            }

            mAdd = 0;
            mTarget = null;
            mAuthor = null;
            base.OnEnd();
        }

        /// <summary>
        /// 提升类型
        /// </summary>
        public enum UpType
        {
            None = 0,

            /// <summary>
            /// 星级
            /// </summary>
            [Description("星级")] Star,
        }
    }
}