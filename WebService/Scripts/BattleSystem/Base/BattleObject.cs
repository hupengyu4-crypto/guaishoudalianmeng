#if UNDEF_BATTLE_UNITY_EDITOR
#undef UNITY_EDITOR
#endif

using System;
using System.ComponentModel;

namespace BattleSystem
{
    public abstract class BattleObject : BattleEvent, IObjectPool
    {
        /// <summary>
        /// 战场
        /// </summary>
        public BaseBattle Battle { get; protected set; }
        /// <summary>
        /// UID
        /// </summary>
        public long Uid { get; protected set; }
        /// <summary>
        /// 是否执行死亡逻辑的标记
        /// </summary>
        public bool IsDoDeadTag { get; set; }
        /// <summary>
        /// 是否死亡
        /// </summary>
        public bool IsDead { get; set; }
        /// <summary>
        /// 是否假死（等待复活）
        /// </summary>
        public bool IsFakeDead { get; set; }
        /// <summary>
        /// 队伍类型
        /// </summary>
        public BattleDef.TeamCampType CampType { get; set; }
        /// <summary>
        /// 当前状态
        /// </summary>
        public State StateMark { get; private set; }
        /// <summary>
        /// 效果控制器
        /// </summary>
        public EffectSys Effect { get; }
        /// <summary>
        /// 是否只能自己给自己添加效果（例如宠物）
        /// </summary>
        public bool isOnlySelfAddEffect = false;
        protected BattleObject(BaseBattle battle)
        {
            Effect = new EffectSys(battle, this);
        }

        public abstract void OnAwake();

        public abstract void OnStart();

        public abstract void OnUpdate();

        /// <summary>
        /// 是否处于不能行动状态
        /// </summary>
        /// <returns></returns>
        public bool IsCannotAction()
        {
            //TODO  有新加控制状态这里也得添加
            return IsState(State.Dizzy) || IsState(State.Frozen) || IsState(State.Paralysis);
        }

        /// <summary>
        /// 是否能使用主动技能
        /// </summary>
        /// <returns></returns>
        public bool IsCanUseRageSkill()
        {
            return !IsState(State.Silence);
        }

        /// <summary>
        /// 添加一个状态
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="author">施法者</param>
        public void AddState(State state, long author,long sid=0)
        {
            if (IsState(state)) return;
            var t = Battle.CreateEventParam<FighterStateParams>();
            t.authorUid = author;
            t.state = state;
            t.targetUid = Uid;
            t.teamCampType = CampType;
            t.IsAutoRelease = false;
            t.fromSid = sid;
            if (!Battle.DispatchEvent(Battle, BaseBattle.Event.AnyFighterAddState, t).IsBlockEvent)
            {
                if (!DispatchEvent(Battle, Event.AddState, t).IsBlockEvent)
                {
                    t.IsAutoRelease = true;
                    StateMark |= state;
#if UNITY_EDITOR
                    Battle.AddInfo(GetBaseDesc()).AddInfo("新增状态：").AddInfo(BattleUtils.GetEnumName<State>(state), true);
#endif

                    Battle.DispatchEvent(Battle, BaseBattle.Event.AnyFighterAddStateEnd, t);
                }
#if UNITY_EDITOR
                else
                {
                    Battle.AddInfo(GetBaseDesc()).AddInfo("新增状态失败：").AddInfo(BattleUtils.GetEnumName<State>(state), true);
                }
#endif
            }
#if UNITY_EDITOR
            else
            {
                Battle.AddInfo(GetBaseDesc()).AddInfo("新增状态失败：").AddInfo(BattleUtils.GetEnumName<State>(state), true);
            }
#endif
        }

        /// <summary>
        /// 删除一个状态
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="author">施法者</param>
        public void RemoveState(State state, long author)
        {
            if (IsState(state))
            {
                var t = Battle.CreateEventParam<FighterStateParams>();
                t.authorUid = author;
                t.state = state;
                t.targetUid = Uid;
                t.teamCampType = CampType;
                t.IsAutoRelease = false;
                if (!Battle.DispatchEvent(Battle, BaseBattle.Event.AnyFighterRemoveState, t).IsBlockEvent)
                {
                    if (!DispatchEvent(Battle, Event.RemoveState, t).IsBlockEvent)
                    {
                        t.IsAutoRelease = true;
                        StateMark &= ~state;
#if UNITY_EDITOR
                        Battle.AddInfo(GetBaseDesc()).AddInfo("删除状态：").AddInfo(BattleUtils.GetEnumName<State>(state), true);
#endif

                        Battle.DispatchEvent(Battle, BaseBattle.Event.AnyFighterRemoveStateEnd, t);
                    }
#if UNITY_EDITOR
                    else
                    {
                        Battle.AddInfo(GetBaseDesc()).AddInfo("删除状态失败：").AddInfo(BattleUtils.GetEnumName<State>(state), true);
                    }
#endif
                }
#if UNITY_EDITOR
                else
                {
                    Battle.AddInfo(GetBaseDesc()).AddInfo("删除状态失败：").AddInfo(BattleUtils.GetEnumName<State>(state), true);
                }
#endif
            }
        }
        /// <summary>
        /// 是否存在某个状态
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool IsState(State state)
        {
            return (StateMark & state) != 0;
        }

        /// <summary>
        /// 设置状态标签
        /// </summary>
        /// <param name="stateMark"></param>
        public void SetStateMark(State stateMark)
        {
            StateMark = stateMark;
        }

        /// <summary>
        /// 清理状态
        /// </summary>
        public void ClearState()
        {
            StateMark = State.Normal;
        }

        /// <summary>
        /// 死亡
        /// </summary>
        public void Dead()
        {
            IsDoDeadTag = true;
            Effect.ClearAll(EffectSys.EffectType.DieRetain);
            Battle.BeginDead();
            DispatchEvent(Battle, BattleObject.Event.Dead);
            Battle.DispatchEvent(Battle, BaseBattle.Event.AnyDead, EventParams<long>.Create(Battle, Uid));
        }

        public virtual string GetBaseDesc()
        {
            return $"{GetType()}-{Uid}";
        }

        /// <summary>
        /// 重置战斗者
        /// </summary>
        public virtual void ResetInit()
        {
            if (IsDead || IsState(State.Dead))
                return;
            Effect.ClearAll();
            ClearState();
        }

        public override void Dispose()
        {
            base.Dispose();
            Uid = 0;
        }

        /// <summary>
        /// 战场对象事件
        /// </summary>
        public enum Event
        {
            /// <summary>
            /// 无，给配置用的
            /// </summary>
            [Description("无，给配置用的")]
            None,
            /// <summary>
            /// 使用技能(全部)
            /// </summary>
            [Description("使用技能(全部)")]
            DoSkill,
            /// <summary>
            /// 使用普攻技能
            /// </summary>
            [Description("使用普攻技能")]
            DoNormalSkill,
            /// <summary>
            /// 使用怒气技能
            /// </summary>
            [Description("使用怒气技能")]
            DoRageSkill,
            /// <summary>
            /// 使用普攻技能完毕
            /// </summary>
            [Description("使用普攻技能完毕")]
            DoNormalSkillEnd,
            /// <summary>
            /// 使用怒气技能完毕
            /// </summary>
            [Description("使用怒气技能完毕")]
            DoRageSkillEnd,
            /// <summary>
            /// 使用普攻技能完毕
            /// </summary>
            [Description("被普攻技能攻击完毕")]
            BeNormalSkillAttackEnd,
            /// <summary>
            /// 使用怒气技能完毕
            /// </summary>
            [Description("被怒气技能攻击完毕")]
            BeRageSkillAttackEnd,
            /// <summary>
            /// 使用技能完毕(全部)
            /// </summary>
            [Description("使用技能完毕(全部)")]
            DoSkillEnd,
            /// <summary>
            /// 属性改变
            /// </summary>
            [Description("属性改变之前")]
            PropPre,
            /// <summary>
            /// 属性改变
            /// </summary>
            [Description("属性改变")]
            Prop,
            /// <summary>
            /// 状态增加
            /// </summary>
            [Description("状态增加")]
            AddState,
            /// <summary>
            /// 状态删除
            /// </summary>
            [Description("状态删除")]
            RemoveState,
            /// <summary>
            /// 治疗效果之前
            /// </summary>
            [Description("受到治疗之前")]
            HealPre,
            /// <summary>
            /// 治疗效果之前
            /// </summary>
            [Description("受到治疗")]
            Heal,
            /// <summary>
            /// 死亡之前
            /// </summary>
            [Description("死亡之前")]
            DeadPre,
            /// <summary>
            /// 死亡
            /// </summary>
            [Description("死亡")]
            Dead,
            /// <summary>
            /// 假死
            /// </summary>
            [Description("假死")]
            FakeDead,
            /// <summary>
            /// 复活前
            /// </summary>
            [Description("复活前")]
            ResurgenceEffectPre,
            /// <summary>
            /// 复活
            /// </summary>
            [Description("复活")]
            ResurgenceEffect,
            /// <summary>
            /// 造成暴击
            /// </summary>
            [Description("造成暴击")]
            Crit,
            /// <summary>
            /// 被暴击
            /// </summary>
            [Description("被暴击")]
            BeCrit,
            /// <summary>
            /// 格挡
            /// </summary>
            [Description("格挡")]
            Block,
            /// <summary>
            /// 格挡
            /// </summary>
            [Description("被格挡")]
            BeBlock,
            /// <summary>
            /// 攻击造成伤害之前(可监听修改伤害)
            /// 只包含 普通攻击和技能伤害
            /// </summary>
            [Description("攻击造成伤害之前(可监听修改伤害)")]
            AttackDamagePre,
            /// <summary>
            /// 攻击造成伤害之前(可监听修改伤害)
            /// 不包含包含 普通攻击和技能伤害
            /// </summary>
            [Description("攻击造成伤害之前(可监听修改伤害)")]
            OtherAttackDamagePre,
            // /// <summary>
            // /// 攻击造成间接伤害之前(不可监听)
            // /// 不包含包含 普通攻击和技能伤害
            // /// </summary>
            // [Description("攻击造成间接伤害之前(可监听修改伤害)")]
            // IndirectDamagePre,
            /// <summary>
            /// 攻击造成伤害(全类型)
            /// </summary>
            [Description("攻击造成伤害(全类型)")]
            AttackDamage,
            /// <summary>
            /// 受到伤害效果之前(可监听修改伤害)
            /// 只包含 普通攻击和技能伤害
            /// </summary>
            [Description("受到伤害效果之前(可监听修改伤害)")]
            BeDamagePre,
            /// <summary>
            /// 受到伤害效果之前(可监听修改伤害)
            /// 不包含 普通攻击和技能伤害
            /// </summary>
            [Description("受到伤害效果之前(可监听修改伤害)")]
            OtherBeDamagePre,
            // /// <summary>
            // /// 受到间接伤害之前(不可监听)
            // /// 不包含包含 普通攻击和技能伤害
            // /// </summary>
            // [Description("受到间接伤害之前(可监听修改伤害)")]
            // BeIndirectDamagePre,
            /// <summary>
            /// 受到伤害效果(全类型)
            /// </summary>
            [Description("受到伤害效果(全类型)")]
            BeDamage,
            /// <summary>
            /// 普通攻击造成伤害
            /// </summary>
            [Description("普通攻击造成伤害")]
            NormalDamage,
            /// <summary>
            /// 普通攻击造成暴击伤害
            /// </summary>
            [Description("普通攻击造成暴击伤害")]
            NormalCritDamage,
            /// <summary>
            /// 攻击造成间接伤害
            /// </summary>
            [Description("攻击造成间接伤害")]
            IndirectDamage,
            /// <summary>
            /// 受到普攻伤害
            /// </summary>
            [Description("受到普攻伤害")]
            BeNormalDamage,
            /// <summary>
            /// 受到普攻暴击伤害
            /// </summary>
            [Description("受到普攻暴击伤害")]
            BeNormalCritDamage,
            /// <summary>
            /// 受到间接伤害
            /// </summary>
            [Description("受到间接伤害")]
            BeIndirectDamage,
            /// <summary>
            /// 技能攻击造成伤害
            /// </summary>
            [Description("技能攻击造成伤害")]
            SkillDamage,
            /// <summary>
            /// 技能攻击造成暴击伤害
            /// </summary>
            [Description("技能攻击造成暴击伤害")]
            SkillCritDamage,
            /// <summary>
            /// 受到技能伤害
            /// </summary>
            [Description("受到技能伤害")]
            BeSkillDamage,
            /// <summary>
            /// 受到技能暴击伤害
            /// </summary>
            [Description("受到技能暴击伤害")]
            BeSkillCritDamage,
            /// <summary>
            /// 造成持续伤害
            /// </summary>
            [Description("造成持续伤害")]
            DeBuffDamage,
            /// <summary>
            /// 受到持续伤害效果
            /// </summary>
            [Description("受到持续伤害")]
            BeDeBuffDamage,
            /// <summary>
            /// 护盾添加
            /// </summary>
            [Description("护盾添加")]
            ShieldAdd,
            /// <summary>
            /// 护盾值改变
            /// </summary>
            [Description("护盾值改变")]
            ShieldChange,
            /// <summary>
            /// 护盾移除
            /// </summary>
            [Description("护盾移除")]
            ShieldRemove,
            /// <summary>
            /// 护盾承受伤害
            /// </summary>
            [Description("护盾承受伤害")]
            ShieldAbsorption,
            /// <summary>
            /// 替换技能
            /// </summary>
            [Description("替换技能")]
            Replace,
            /// <summary>
            /// 击杀目标
            /// </summary>
            [Description("击杀目标")]
            Kill,
            /// <summary>
            /// 被击杀
            /// </summary>
            [Description("被击杀")]
            BeKill,
            /// <summary>
            /// 抵抗
            /// </summary>
            [Description("抵抗")]
            Resistance,
            /// <summary>
            /// 攻击嘲讽者
            /// </summary>
            [Description("攻击嘲讽者")]
            AttackTaunter,
            /// <summary>
            /// 攻击嘲讽者
            /// </summary>
            [Description("嘲讽者被攻击")]
            TaunterBeAttacked,
            /// <summary>
            /// 受到伤害但不是从damage来的，显示层做飘血等
            /// </summary>
            [Description("受到伤害但不是从damage来的，显示层做飘血等")]
            BeDamageWithoutDamageEffect,
            /// <summary>
            /// 触发了免死效果
            /// </summary>
            [Description("触发了免死效果")]
            ImmuneDamageTrigger,
            /// <summary>
            /// 火把致死指令
            /// </summary>
            [Description("火把致死指令")]
            CmdFireKill,
            /// <summary>
            /// 再动
            /// </summary>
            [Description("再动")]
            ActAgain,
            /// <summary>
            /// 斩杀
            /// </summary>
            [Description("斩杀")]
            KillImmuneDamage,
            /// <summary>
            /// 引爆灼烧伤害
            /// </summary>
            [Description("引爆灼烧伤害")]
            TriggerAllBurnDmg,
            /// <summary>
            /// 引爆流血伤害
            /// </summary>
            [Description("引爆流血伤害")]
            TriggerAllBleedDmg,
            /// <summary>
            /// 引爆中毒伤害
            /// </summary>
            [Description("引爆中毒伤害")]
            TriggerAllPoisonDmg,
        }

        /// <summary>
        /// 战场对象状态(注意最多支持到62)
        /// </summary>
        [Flags]
        public enum State : ulong
        {
            /// <summary>
            /// 正常
            /// </summary>
            [Description("正常")]
            Normal = 0,
            /// <summary>
            /// 眩晕
            /// </summary>
            [Description("眩晕")]
            Dizzy = 1UL << 0,
            /// <summary>
            /// 沉默
            /// </summary>
            [Description("沉默")]
            Silence = 1UL << 1,
            /// <summary>
            /// 中毒
            /// </summary>
            [Description("中毒")]
            Poison = 1UL << 2,
            /// <summary>
            /// 冰冻
            /// </summary>
            [Description("冰冻")]
            Frozen = 1UL << 3,
            /// <summary>
            /// 流血
            /// </summary>
            [Description("流血")]
            Bleed = 1UL << 4,
            /// <summary>
            /// 灼烧
            /// </summary>
            [Description("灼烧")]
            Burn = 1UL << 5,
            /// <summary>
            /// 麻痹
            /// </summary>
            [Description("麻痹")]
            Paralysis = 1UL << 6,
            /// <summary>
            /// 死亡
            /// </summary>
            [Description("死亡")]
            Dead = 1UL << 7,
            /// <summary>
            /// 护盾
            /// </summary>
            [Description("护盾")]
            Shield = 1UL << 8,
            /// <summary>
            /// 无法回怒
            /// </summary>
            [Description("无法回怒")]
            UnableAddAnger = 1UL << 9,
            /// <summary>
            /// 复活
            /// </summary>
            [Description("复活")]
            Relive = 1UL << 10,
            /// <summary>
            /// 缴械
            /// </summary>
            [Description("缴械")]
            Disarm = 1UL << 11,
            /// <summary>
            /// 传导
            /// </summary>
            [Description("传导")]
            Conduction = 1UL << 12,
            /// <summary>
            /// 无法获得护盾
            /// </summary>
            [Description("无法获得护盾")]
            UnableAddShield = 1UL << 13,
            /// <summary>
            /// 无视免死
            /// </summary>
            [Description("无视免死")]
            IgnoreImmuneDead = 1UL << 14,
            /// <summary>
            /// 嘲讽
            /// </summary>
            [Description("嘲讽")]
            Taunte = 1UL << 15,
            /// <summary>
            /// 孤立
            /// </summary>
            [Description("孤立")]
            Isolate = 1UL << 16,
            /// <summary>
            /// 再动
            /// </summary>
            [Description("再动")]
            ActAgain = 1UL << 17,
            /// <summary>
            /// 箭矢
            /// </summary>
            [Description("箭矢")]
            Arrow = 1UL << 18,
            /// <summary>
            /// 无法复活
            /// </summary>
            [Description("无法复活")]
            IgnoreRelive = 1UL << 19,
            /// <summary>
            /// 神主
            /// </summary>
            [Description("神主")]
            ShenZhu = 1UL << 20,
            /// <summary>
            /// 神格碎片
            /// </summary>
            [Description("神格碎片")]
            ShenGeSuiPian = 1UL << 21,
            /// <summary>
            /// 神威
            /// </summary>
            [Description("神威")]
            ShenWei = 1UL << 22,
            /// <summary>
            /// 无法被怒气技选中
            /// </summary>
            [Description("无法被怒气技选中")]
            CannotSelectByRageSkill = 1UL << 23,
            /// <summary>
            /// 怒击
            /// </summary>
            [Description("怒击")]
            NuJi = 1UL << 24,
            /// <summary>
            /// 真视
            /// </summary>
            [Description("真视")]
            ZhenShi = 1UL << 25,
            /// <summary>
            /// 余威
            /// </summary>
            [Description("余威")]
            YuWei = 1UL << 26,
            /// <summary>
            /// 免疫伤害
            /// </summary>
            [Description("免疫伤害")]
            ImmuneDamage = 1UL << 27,
            /// <summary>
            /// 弱点
            /// </summary>
            [Description("弱点")]
            Weakness = 1UL << 28,
            /// <summary>
            /// 生死与共
            /// </summary>
            [Description("生死与共")]
            ShengSiYuGong = 1UL << 29,
            /// <summary>
            /// 免疫死亡
            /// </summary>
            [Description("免疫死亡")]
            ImmuneDead = 1UL << 30,
            /// <summary>
            /// 龙子仪轨
            /// </summary>
            [Description("龙子仪轨")]
            LongZiYiGui = 1UL << 31,
            /// <summary>
            /// 裂碑之诅
            /// </summary>
            [Description("裂碑之诅")]
            LieBieZhiZu = 1UL << 32,
        }
    }
}
