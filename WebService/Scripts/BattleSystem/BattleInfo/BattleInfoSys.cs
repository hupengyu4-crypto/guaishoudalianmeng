#if UNDEF_BATTLE_UNITY_EDITOR
#undef UNITY_EDITOR
#endif

using System;
using System.Text;

namespace BattleSystem
{
    /// <summary>
    /// 战报系统
    /// </summary>
    public class BattleInfoSys : IDisposable
    {
        /// <summary>
        /// 战报
        /// </summary>
        public StringBuilder BattleLogSb { get; private set; }

        public BaseBattle Battle { get; }

        public BattleInfoSys(BaseBattle battle)
        {
            Battle = battle;
            BattleLogSb = new StringBuilder();
            InitEvents();
        }

        private void InitEvents()
        {
#if UNITY_EDITOR
            //战场状态
            Battle.AddEvent(BaseBattle.Event.InitEnd, delegate (EventParams arg0)
            {
                AddInfo("初始化战场结束", true);
                AddInfo("", true);
            });
            Battle.AddEvent(BaseBattle.Event.BoutStartShow, delegate (EventParams arg0)
            {
                AddInfo("===第<");
                AddInfo(Battle.Bout.ToString());
                AddInfo(">回合开始===", true);
                AddInfo("", true);
            });
            Battle.AddEvent(BaseBattle.Event.BoutEndShow, delegate (EventParams arg0)
            {
                AddInfo("===第<");
                AddInfo(Battle.Bout.ToString());
                AddInfo(">回合结束===", true);
                AddInfo("", true);
            });
            Battle.AddEvent(BaseBattle.Event.RoundStart, delegate (EventParams arg0)
            {
                AddInfo("=小回合开始=", true);
                AddInfo("", true);
                var battle = (NormalBattle)Battle;
                if (battle.Attackers.pet != null)
                {
                    AddInfo($"{battle.Attackers.pet}", true);
                }
                if (battle.Defenders.pet != null)
                {
                    AddInfo($"{battle.Defenders.pet}", true);
                }
                foreach (var t in battle.AllFighter)
                {
                    AddInfo($"{t}", true);
                }
            });
            Battle.AddEvent(BaseBattle.Event.RoundEnd, delegate (EventParams arg0)
            {
                AddInfo("=小回合结束=", true);
                AddInfo("", true);
            });
            Battle.AddEvent(BaseBattle.Event.BattleOver, delegate (EventParams arg0)
            {
                AddInfo("", true);
                var battle = (NormalBattle)Battle;
                if (battle.Attackers.IsAllDead)
                {
                    AddInfo("攻击方全阵亡", true);
                }
                else if (battle.Defenders.IsAllDead)
                {
                    AddInfo("防守方全阵亡", true);
                }

                foreach (var t in battle.AllFighter)
                {
                    AddInfo($"{t}", true);
                }

                AddInfo("", true);
                AddInfo("===战斗结束===", true);

                AddInfo("", true);

                AddInfo("逻辑帧:").AddInfo(battle.Frame, true);
                AddInfo("回合数:").AddInfo(battle.Bout, true);
            });
#endif
        }

        /// <summary>
        /// 监听战场对象信息
        /// </summary>
        /// <param name="obj"></param>
        public void ListenObjectInfo(BattleObject obj)
        {
#if UNITY_EDITOR
            obj.AddEvent(BattleObject.Event.DoSkill, delegate (EventParams arg0)
            {
                if (arg0 is EventParams<BattleSkill> skil)
                {
                    GetFighterBaseInfo(obj, out var f).AddInfo("使用:[").AddInfo(skil.data.Sid.ToString())
                        .AddInfo("-").AddInfo(skil.data.Cfg.describe).AddInfo("]", true);
                }
            });
            obj.AddEvent(BattleObject.Event.Prop, delegate (EventParams arg0)
            {
                if (arg0 is PropParams param)
                {
                    GetFighterBaseInfo(obj, out var f).AddInfo("的[")
                        .AddInfo(BattleUtils.GetPropertyName(param.property)).AddInfo("]改变:")
                        .AddInfo(param.dataValue).AddInfo(">>>").AddInfo(param.newValue, true);
                }
            });
            //obj.AddEvent(BattleObject.Event.AttackDamagePre, delegate (EventParams arg0)
            //{
            //    GetFighterBaseInfo(obj, out var f).AddInfo("开始攻击", true);
            //});
            obj.AddEvent(BattleObject.Event.BeDamage, delegate (EventParams arg0)
            {
                if (arg0 is DamageParams param)
                {
                    GetFighterBaseInfo(obj, out var f).AddInfo("受到[").AddInfo(param.damageType).AddInfo(param.effectSid).AddInfo("]伤害:").AddInfo(param.newValue, true);
                }
            });
            obj.AddEvent(BattleObject.Event.BeDeBuffDamage, delegate (EventParams arg0)
            {
                if (arg0 is DamageParams param)
                {
                    GetFighterBaseInfo(obj, out var f).AddInfo("受到[").AddInfo(param.damageType).AddInfo(param.effectSid).AddInfo("]伤害:").AddInfo(param.newValue, true);
                }
            });
            obj.AddEvent(BattleObject.Event.DeadPre, delegate (EventParams arg0)
            {
                GetFighterBaseInfo(obj, out var f).AddInfo("死亡:").AddInfo(f.ToString(), true);
            });
            obj.AddEvent(BattleObject.Event.HealPre, delegate (EventParams arg0)
            {
                if (arg0 is HealParams param)
                {
                    GetFighterBaseInfo(obj, out var f).AddInfo("受到治疗:").AddInfo(param.newValue, true);
                }
            });
            //效果
            obj.AddEvent(EffectSys.Event.TriggerPre, delegate (EventParams arg0)
            {
                if (arg0 is EventParams<BaseEffect> effect)
                {
                    GetFighterBaseInfo(obj, out var f).AddInfo("触发效果:").AddInfo(effect.data.GetType().Name).AddInfo("-").AddInfo(effect.data.sid).
                        AddInfo(",回合:").AddInfo(effect.data.GetActiveBout()).AddInfo("/").AddInfo(effect.data.bout).AddInfo(",生效数:").AddInfo(effect.data.triggerNum).
                        AddInfo("/").AddInfo(effect.data.triggerCount, true);
                }
            });
            obj.AddEvent(EffectSys.Event.EndPre, delegate (EventParams arg0)
            {
                if (arg0 is EventParams<BaseEffect> effect)
                {
                    GetFighterBaseInfo(obj, out var f).AddInfo("清理效果:").AddInfo(effect.data.GetType().Name).AddInfo("-").AddInfo(effect.data.sid).
                        AddInfo(",回合:").AddInfo(effect.data.GetActiveBout()).AddInfo("/").AddInfo(effect.data.bout).AddInfo(",生效数:").AddInfo(effect.data.triggerNum).
                        AddInfo("/").AddInfo(effect.data.triggerCount, true);
                }
            });
#endif
        }

        public void ListenSkillInfo(BattleSkill skill)
        {

        }


        public void ListenEffectInfo(BaseEffect effect)
        {

        }

        public BattleInfoSys AddInfo(object info, bool isNewLine = false)
        {
            return AddInfo(info.ToString(), isNewLine);
        }

        /// <summary>
        /// 添加战报
        /// </summary>
        /// <param name="info">内容</param>
        /// <param name="isNewLine">添加后是否换行</param>
        public BattleInfoSys AddInfo(string info, bool isNewLine = false)
        {
            if (!BattleDef.DebugLog)
                return this;
            BattleLogSb.Append(info);
            if (isNewLine)
            {
                BattleLogSb.AppendLine();
            }
            return this;
        }

        public override string ToString()
        {
            return BattleLogSb?.ToString() ?? string.Empty;
        }

        public void Dispose()
        {
            BattleLogSb.Clear();
        }

        /// <summary>
        /// 战场对象基础描述
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        private BattleInfoSys GetFighterBaseInfo(BattleObject obj, out Fighter f)
        {
            f = (Fighter)obj;
            return f.Battle.AddInfo(f.CampType == BattleDef.TeamCampType.Attacker ? "进攻方" : "防守方")
                .AddInfo("(").AddInfo((f.Data.Pos).ToString(), false).AddInfo(")号位(")
                .AddInfo(f.Data.Cfg?.name).AddInfo(")");
        }
    }
}