using System.Collections.Generic;

namespace BattleSystem
{
    /// <summary>
    /// 特殊共享吸血-自身吸血之后，剩余吸血量共享给其他人
    /// </summary>
    public class ShareVampireEffect : BaseEffect
    {
        /// <summary>
        /// 角色攻击事件(仅限)
        /// </summary>
        public BattleObject.Event fighterEvent;
        /// <summary>
        /// 吸取系数(百分比)
        /// </summary>
        public double factor;

        /// <summary>
        /// 索敌(吸血溢出后共享的目标)
        /// </summary>
        public long findEffects;

        /// <summary>
        /// 索敌目标
        /// </summary>
        private List<Fighter> allTargets = null;

        /// <summary>
        /// 索敌效果
        /// </summary>
        private FindTargetEffect findEffect;

        public override void OnBegin()
        {
            if (fighterEvent != BattleObject.Event.None)
            {
                target.AddEvent(fighterEvent, CheckHandler, false, 1);
            }
            base.OnBegin();
        }

        private void CheckHandler(EventParams arg0)
        {
            if (battle.CanNotHeal)
            {
                return;
            }

            //Fighter mAuthor = (Fighter)author;
            Fighter mTarget = (Fighter)target;

            switch (fighterEvent)
            {
                case BattleObject.Event.Crit:
                case BattleObject.Event.BeCrit:
                case BattleObject.Event.Block:
                case BattleObject.Event.BeBlock:
                case BattleObject.Event.AttackDamagePre:
                case BattleObject.Event.AttackDamage:
                case BattleObject.Event.BeDamagePre:
                case BattleObject.Event.BeDamage:
                case BattleObject.Event.NormalDamage:
                case BattleObject.Event.NormalCritDamage:
                case BattleObject.Event.BeNormalDamage:
                case BattleObject.Event.BeNormalCritDamage:
                case BattleObject.Event.SkillDamage:
                case BattleObject.Event.SkillCritDamage:
                case BattleObject.Event.BeSkillDamage:
                case BattleObject.Event.BeSkillCritDamage:
                case BattleObject.Event.DeBuffDamage:
                case BattleObject.Event.BeDeBuffDamage:
                //case BattleObject.Event.IndirectDamagePre:
                case BattleObject.Event.IndirectDamage:
                //case BattleObject.Event.BeIndirectDamagePre:
                case BattleObject.Event.BeIndirectDamage:
                    if (arg0 is DamageParams { newValue: > 0 } param)
                    {
                        // 剩余吸血量
                        long remainVampireHp = (long)YKMath.Ceiling(param.newValue * factor * BattleDef.Percent);
                        // 先回复自己
                        var hurtHp = mTarget.Data.GetProp(BattleDef.Property.max_hp) - mTarget.Data.GetProp(BattleDef.Property.dead_hp) - mTarget.Data.GetProp(BattleDef.Property.hp);
                        hurtHp = System.Math.Max(hurtHp, 0);
                        var recHp = remainVampireHp < hurtHp ? remainVampireHp : hurtHp;
#if UNITY_EDITOR
                        battle.AddInfo($"特殊回血---触发者：{mTarget.Data.Cfg?.name}---总吸血量：{remainVampireHp}---自身回血量：{recHp}", true);
#endif
                        mTarget.AddProp(BattleDef.Property.hp, recHp, authorUid);
                        remainVampireHp -= recHp;

                        // 再共享剩余吸血量给目标
                        if (remainVampireHp > 0)
                        {
                            // 共享回血的索敌
                            if (findEffects != 0)
                            {
                                target.Effect.AddEffect(findEffects, target.Uid, effectLevel, out var ef);
                                if (ef is FindTargetEffect _findEffect)
                                {
                                    findEffect = _findEffect;
                                }
                                else
                                {
                                    target.Effect.ClearEffect(ef);
                                }
                            }

                            if (findEffect != null)
                            {
                                allTargets = findEffect.GetFighters();
                            }

                            if (allTargets != null && allTargets.Count > 0)
                            {
                                // 移除自己
                                for (int i = allTargets.Count - 1; i >= 0; i--)
                                {
                                    if (allTargets[i].Uid == target.Uid)
                                    {
                                        allTargets.Remove(allTargets[i]);
                                    }
                                }

                                int shareCount = allTargets.Count;
                                //if (shareCount > 1)
                                //{
                                //    allTargets.Sort((left, right) =>
                                //    {
                                //        if (left == null || right == null)
                                //            return 0;
                                //        var leftHp = left.Data[BattleDef.Property.hp];
                                //        var rightHp = right.Data[BattleDef.Property.hp];
                                //        if (leftHp == rightHp)
                                //            return left.Uid < right.Uid ? -1 : 1; //return 0;
                                //        return leftHp > rightHp ? 1 : -1;
                                //    });
                                //}

                                for (int i = 0; i < shareCount; i++)
                                {
                                    var fighter = allTargets[i];
                                    var tmpData = fighter.Data;
                                    var value = tmpData.GetProp(BattleDef.Property.max_hp) - tmpData.GetProp(BattleDef.Property.dead_hp) - tmpData.GetProp(BattleDef.Property.hp);
                                    value = System.Math.Min(System.Math.Max(value, 0), remainVampireHp);
#if UNITY_EDITOR
                                    battle.AddInfo($"特殊吸血-共享吸血 回血者：{tmpData.Cfg?.name}---回血量：{value}", true);
#endif
                                    fighter.AddProp(BattleDef.Property.hp, value, authorUid);
                                    remainVampireHp -= value;
                                    if (remainVampireHp <= 0)
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        if (findEffect != null)
                        {
                            target.Effect.ClearEffect(findEffect);
                            findEffect = null;
                            allTargets.Clear();
                            allTargets = null;
                        }
#if UNITY_EDITOR
                        battle.AddInfo($"", true);
#endif

                        //触发次数提供
                        TriggerBegin();
                    }
                    break;
                default:
                    Log.error($"配置了非伤害事件{fighterEvent}，无法获取有效数据，请检查配置{GetType().Name}-{sid}");
                    break;
            }
        }

        public override void Trigger()
        {

        }

        public override void OnEnd()
        {
            if (fighterEvent != BattleObject.Event.None)
            {
                target.RemoveEvent(fighterEvent, CheckHandler);
                if (findEffect != null)
                {
                    target.Effect.ClearEffect(findEffect);
                    findEffect = null;
                    allTargets.Clear();
                    allTargets = null;
                }
            }
            base.OnEnd();
        }
    }
}