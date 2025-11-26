#if UNDEF_BATTLE_UNITY_EDITOR
#undef UNITY_EDITOR
#endif

using System.ComponentModel;

namespace BattleSystem
{
    /// <summary>
    /// 伤害减免效果
    /// </summary>
    public class DamageReduceEffect : BaseEffect
    {
        /// <summary>
        /// 减免数值是否叠加
        /// </summary>
        public bool isOverlay;

        /// <summary>
        /// 系数
        /// </summary>
        public double factor;

        /// <summary>
        /// 改变的值
        /// </summary>
        private double _addValue;

        /// <summary>
        /// 目标
        /// </summary>
        private Fighter _target;

        public override void OnBegin()
        {
            _target = (Fighter)target;
            _addValue = 0;
            base.OnBegin();
        }

        public override void Trigger()
        {
            DamageChange();
        }

        /// <summary>
        /// 改变伤害减免
        /// </summary>
        /// <param name="valueAdd">增加为1，减少为-1，只是相对的当配置负数，增加就会反过来是减少，减少就是增加，不同参照目标的理解</param>
        private void DamageChange(int valueAdd = 1)
        {
            double changeValue = valueAdd * factor; //(long)YKMath.Ceiling(tempTarget.Data[property] * factor * BattleDef.Percent);

            _target.AddDamageReduce(changeValue);

            _addValue += changeValue;

#if UNITY_EDITOR
            battle.AddInfo(_target.GetBaseDesc()).AddInfo("DamageReduceEffect-").AddInfo(sid).AddInfo("生效,")
                .AddInfo("额外伤害减免改变:")
                .AddInfo(_target.GetDamageReduce()).AddInfo(">>")
                .AddInfo(_target.GetDamageReduce() + changeValue, true);
#endif
        }

        public override void OnOverlayAdd()
        {
            if(isOverlay && overlayCount < addMax)
            {
                DamageChange();
            }

            base.OnOverlayAdd();
        }

        public override void OnOverlayReduce()
        {
            base.OnOverlayReduce();
            if(!isEnd && isOverlay && overlayCount >= 0)
            {
                DamageChange(-1);
            }
        }

        public override void OnEnd()
        {
            _target.AddDamageReduce(-_addValue);
            _addValue = 0;
            _target = null;
            base.OnEnd();
        }
    }
}