#if UNDEF_BATTLE_UNITY_EDITOR
#undef UNITY_EDITOR
#endif

using RootScript.Config;

namespace BattleSystem
{
    /// <summary>
    /// 属性效果
    /// </summary>
    public class PropertyEffect : BaseEffect
    {
        /// <summary>
        /// 参照目标类型
        /// </summary>
        public BattleDef.TargetType propTarget = BattleDef.TargetType.Self;
        /// <summary>
        /// 修改的属性
        /// </summary>
        public BattleDef.Property property;
        /// <summary>
        /// 修改值(百分比)
        /// </summary>
        public double[] value;
        /// <summary>
        /// 星级加成
        /// </summary>
        public double starValue;
        /// <summary>
        /// 等级加成
        /// </summary>
        public double lvValue;
        /// <summary>
        /// 结束时是否保留
        /// </summary>
        public bool isRetain;
        /// <summary>
        /// 是否乘以当前回合数
        /// </summary>
        public bool isUseBout;
        /// <summary>
        /// 是否数值类型
        /// </summary>
        public bool isNumber;

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
        private long mAdd;


        public override void OnBegin()
        {
            mAuthor = (Fighter)author;
            mTarget = (Fighter)target;
            mAdd = 0;
            base.OnBegin();
        }

        public override void Trigger()
        {
            ChangeProp();
        }

        /// <summary>
        /// 改变属性
        /// </summary>
        /// <param name="valueAdd">增加为1，减少为-1，只是相对的当配置负数，增加就会反过来是减少，减少就是增加，不同参照目标的理解</param>
        private void ChangeProp(int valueAdd = 1)
        {
            long add;
            var changeProperty = property;
            if (_additionData != null && _additionData.propertyName > 0 && _additionData.propertyValue != 0.0d)
            {
                add = _additionData.propertyValue;
                changeProperty = (BattleDef.Property)_additionData.propertyName;
            }
            else
            {
                long addRate = isUseBout ? battle.Bout : 1;
                var isCoe = BattleDef.IsPropertyCoefficient[property];
                var temp = propTarget == BattleDef.TargetType.Self ? mAuthor.Data.initProps : mTarget.Data.initProps;

                var actualValue = 0.0d;
                if (BattleUtils.GetLevelValue(value, effectLevel, out var cfgValue))
                {
                    actualValue = cfgValue;
                }

                if (actualValue < 0 && property == BattleDef.Property.heal)
                {
                    //减疗抵抗
                    var reduceHealingCut = YKMath.Clamp(1.0 - mTarget.Data[BattleDef.Property.reduce_healing_cut] * BattleDef.Percent100, 0.0, 1.0);
                    actualValue *= reduceHealingCut;
                }
                actualValue = actualValue + mAuthor.Data.Star * starValue + mAuthor.Data.Level * lvValue;

                if (isNumber)
                {
                    add = (long)(actualValue * valueAdd * addRate);
                }
                else
                {
                    if (isCoe)
                    {
                        //改变属性系数=(10000 * 百分比)
                        add = (long)((BattleDef.TenThousand * actualValue * BattleDef.Percent)) * valueAdd * addRate;
                    }
                    else
                    {
                        //改变属性值=(基础属性*百分比)
                        add = (long)YKMath.Ceiling((temp[(int)property] * actualValue * BattleDef.Percent) * valueAdd * addRate);
                    }
                }

                if (_additionData != null)
                {
                    add = (long)YKMath.Ceiling(add * (_additionData.stepChangeValue + 1.0));
                }
            }

#if UNITY_EDITOR
            battle.AddInfo(mTarget.GetBaseDesc()).AddInfo("PropertyEffect-").AddInfo(sid).AddInfo("生效,[").AddInfo(changeProperty).AddInfo("]改变:")
                .AddInfo(mTarget.Data[(int)changeProperty]).AddInfo(">>").AddInfo(mTarget.Data[(int)changeProperty] + add, true);
#endif

            var oldProp = mTarget.Data[changeProperty];
            mTarget.AddProp(changeProperty, add, authorUid);

            mAdd += mTarget.Data[changeProperty] - oldProp;//通过新旧属性对比来计算差异，因为可能最大值的限制，加不上

            if (bout > 0 || isRetain)
            {
                mTarget.UpdateChangeProperty(changeProperty, add);
            }
        }

        public override void OnOverlayAdd()
        {
            if (overlayCount < addMax)
            {
                ChangeProp();
            }
            base.OnOverlayAdd();
        }

        public override void OnOverlayReduce()
        {
            base.OnOverlayReduce();
            if (!isEnd && overlayCount >= 0)
            {
                ChangeProp(-1);
            }
        }

        public override void OnEnd()
        {
            if (!isRetain)
            {
#if UNITY_EDITOR
                battle.AddInfo(mTarget.GetBaseDesc()).AddInfo("PropertyEffect-").AddInfo(sid).AddInfo("结束,[").AddInfo(property).AddInfo("]恢复:")
                    .AddInfo(mTarget.Data[(int)property]).AddInfo(">>").AddInfo(mTarget.Data[(int)property] - mAdd, true);
#endif
                mTarget.AddProp(property, -mAdd, authorUid);
                //mTarget.Data.nowProps[(int)property] -= mAdd;
                if (bout > 0)//如果设置了永久，就不用扣除了
                {
                    //属性类debuff消失后，还原到初始属性，不显示属性改变标识
                    mTarget.UpdateChangeProperty(property, 0);
                }
            }

            mAdd = 0;
            mTarget = null;
            mAuthor = null;
            base.OnEnd();
        }
    }
}