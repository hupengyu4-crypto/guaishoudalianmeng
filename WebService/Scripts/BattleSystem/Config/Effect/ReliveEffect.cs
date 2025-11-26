#if UNDEF_BATTLE_UNITY_EDITOR
#undef UNITY_EDITOR
#endif

using System.Collections.Generic;
namespace BattleSystem
{
    public class ReliveEffect : BaseEffect
    {
        /// <summary>
        /// 回血系数(百分比)
        /// </summary>
        public long hpValue;

        /// <summary>
        /// 新复活
        /// </summary>
        public bool newReliveSystem;

        #region newReliveSystem
        /// <summary>
        /// 新复活的索敌
        /// </summary>
        public long findEffects;

        /// <summary>
        /// 新复活的扣血系数（百分比）
        /// </summary>
        public double factor;

        /// <summary>
        /// 扣血最大值关联属性
        /// </summary>
        public BattleDef.Property property;

        /// <summary>
        /// 扣血最大值关联属性比例（百分比）
        /// </summary>
        public double propertyFactor;

        /// <summary>
        /// 触发次数
        /// </summary>
        public int checkCount;
        #endregion

        /// <summary>
        /// 目标
        /// </summary>
        private Fighter mTarget;
        private int _checkedCount = 0;

        public override void OnBegin()
        {
            mTarget = (Fighter)target;
            _checkedCount = 0;
            battle.AddEvent(BaseBattle.Event.RoundEnd, TryRelive);
            if (newReliveSystem)
            {
                mTarget.AddEvent(BattleObject.Event.DeadPre, OnDeadPre, false, -100); //-100,最后执行
            }
            base.OnBegin();
        }

        private void TryRelive(EventParams arg0)
        {
            if (target.IsState(BattleObject.State.IgnoreRelive))
            {
                //target.Effect.ClearEffect(this); //不删effect，保持以前的逻辑 by ww
                ChangeToRealDead();
                return;
            }

            if (target.IsFakeDead)
            {
                target.IsFakeDead = false;
                mTarget.DispatchEvent(battle, BattleObject.Event.ResurgenceEffectPre,
                    EventParams<Fighter>.Create(battle, mTarget));
                var tData = mTarget.Data;
                var recoverPercent = newReliveSystem ? 100 : hpValue;
                var addHp =
                    System.Math.Max(1,
                                    (long)YKMath.Ceiling((tData[BattleDef.Property.max_hp] - tData[BattleDef.Property.dead_hp]) *
                                                         (recoverPercent * BattleDef.Percent)));
                mTarget.AddProp(BattleDef.Property.hp, addHp, target.Uid);
#if UNITY_EDITOR
                battle.AddInfo(mTarget.GetBaseDesc()).AddInfo("ReliveEffect-").AddInfo(sid).AddInfo("生效,恢复:").AddInfo(addHp, true);
#endif
                mTarget.DispatchEvent(battle, BattleObject.Event.ResurgenceEffect,
                    EventParams<Fighter>.Create(battle, mTarget));
            }
        }

        private void OnDeadPre(EventParams arg0)
        {
            if (target.IsState(BattleObject.State.IgnoreRelive))
            {
                //target.Effect.ClearEffect(this); //不删effect，保持以前的逻辑 by ww
                ChangeToRealDead();
                return;
            }

            if (!newReliveSystem || factor <= 0.0)
            {
                return;
            }

            if (checkCount > 0 && _checkedCount >= checkCount)
            {
                return;
            }

            ++_checkedCount;
            var maxValue = (long)YKMath.Ceiling(mTarget.Data[property] * (propertyFactor * BattleDef.Percent));
            if (maxValue <= 0)
            {
                return;
            }

            List<Fighter> allTargets = null;
            target.Effect.AddEffect(findEffects, target.Uid, effectLevel, out var e);
            if (e is FindTargetEffect findTargetEffect)
            {
                allTargets = findTargetEffect.GetFighters();
            }
            target.Effect.ClearEffect(e);

            if (allTargets == null)
            {
                return;
            }

            var allDamage = 0L;
            var scale = factor * BattleDef.Percent;
            for (int i = 0, l = allTargets.Count; i < l; i++)
            {
                var t = allTargets[i];
                var d = System.Math.Min((long)YKMath.Ceiling((t.Data[BattleDef.Property.max_hp] - t.Data[BattleDef.Property.dead_hp]) * scale),
                                        maxValue);
                if (d > 0)
                {
                    allDamage += d;
                    var dmgArg = BattleUtils.CreateSimpleDamageParams(battle, sid, target.Uid, t.Uid, d, BattleDef.DamageType.Normal, true, true);
                    t.DispatchEvent(battle, BattleObject.Event.BeDamageWithoutDamageEffect, dmgArg);
                    t.AddProp(BattleDef.Property.hp, -d, authorUid);
                }
            }

            var targetMaxHp = mTarget.Data[BattleDef.Property.max_hp] - mTarget.Data[BattleDef.Property.dead_hp];
            if (allDamage >= targetMaxHp)
            {
                target.IsFakeDead = true;
            }
        }

        public override void Trigger()
        {
        }

        private void ChangeToRealDead()
        {
            if (target.IsFakeDead)
            {
                target.IsFakeDead = false;
                //通过标记去让死亡逻辑后面再处理，避免提起清理了效果
                target.IsDead = true;
                target.IsDoDeadTag = false;
                target.ClearState();
                target.AddState(BattleObject.State.Dead, authorUid);
            }
        }

        public override void OnEnd()
        {
            battle.RemoveEvent(BaseBattle.Event.RoundEnd, TryRelive);
            if (newReliveSystem)
            {
                mTarget.RemoveEvent(BattleObject.Event.DeadPre, OnDeadPre);
            }
            mTarget = null;

            ChangeToRealDead();

            base.OnEnd();
        }

    }
}
