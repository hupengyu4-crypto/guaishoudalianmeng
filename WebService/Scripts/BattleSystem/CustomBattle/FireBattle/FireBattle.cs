using System;
using System.Collections.Generic;

namespace BattleSystem
{
    /// <summary>
    /// 火把战斗
    /// 兰阳
    /// 2023-11-16
    /// </summary>
    public class FireBattle : NormalBattle
    {
        public FireBattle(long uid, BattleSceneCfg cfg) : base(uid, cfg)
        {
        }
        protected override void InitTeamData()
        {
            //进攻方 火把战没有进攻方
            Attackers = new FireAtkTeamData(BattleDef.TeamCampType.Attacker, this);
            //防守方
            Defenders = new NormalTeamData(BattleDef.TeamCampType.Defender, this);
            Defenders.InitBattlers(BattleData.DfsCamp);
        }
        protected override void InitFighter()
        {
            IsAllDeadTag = Defenders.AllFighters.Count == 0;

            for (var i = 0; i < Defenders.AllFighters.Count; i++)
            {
                AllFighter.Add(Defenders.AllFighters[i]);
            }

            DispatchEvent(this, Event.OpeningAnimation);

            //初始化完成前第一次排序，为开场技能和被动技能准备
            SortFighter();


            FightIndex = AllFighter.Count; //默认下所有人都出手，大回合切换时会重置

            DispatchEvent(this, Event.DoPassiveSkills);
            for (int i = 0; i < AllFighter.Count; i++)
            {
                if (!AllFighter[i].IsDead)
                {
                    AllFighter[i].DoPassiveSkills();
                }
            }
        }
        protected override void SortFighter()
        {
            //站位不决定出手顺序,由双方武将的速度决定,速度越高,出手优先级越高,若双方存在速度相同的英雄,由团队总速度决定出手顺序
            //同队同速度英雄，由战位优先级决定
            AllFighter.Sort(((left, right) =>
            {
                // 先检查 null 值的情况，按需返回负数、0 或正数
                if (left == null && right == null)
                    return 0;
                if (left == null)
                    return -1;
                if (right == null)
                    return 1;
                var leftSp = left.Data.GetProp(BattleDef.Property.speed);
                var rightSp = right.Data.GetProp(BattleDef.Property.speed);

                if (leftSp == rightSp)
                {
                    if (left.Data.Pos != right.Data.Pos)
                    {
                        return left.Data.Pos > right.Data.Pos ? 1 : -1;
                    }
                    return left.Uid < right.Uid ? -1 : 1; //return 0;
                }
                return leftSp < rightSp ? 1 : -1;
            }));
        }

        public override void UpdateLogic()
        {
            if (IsBattleOverTag)
            {
                BattleOver();
            }
            if (IsAllDeadTag)
            {
                AllDead();
                return;
            }

            if (IsBattleOver)
                return;

            Cmd.OnUpdate(1);

            OnUpdateLogic();
        }

        protected override void OnUpdateLogic()
        {
            if (Defenders.IsAllDead)
                return;

            //标记死亡状态的战斗者执行死亡流程
            int count = AllFighter.Count;
            for (int i = 0; i < count; i++)
            {
                var fighter = AllFighter[i];
                if (fighter.IsState(BattleObject.State.Dead) && !fighter.IsDoDeadTag)
                {
                    fighter.Dead();
                }
            }
        }
        public override void BeginDead()
        {
            if (IsBattleOver || IsBattleOverTag)
                return;
            if (Defenders.IsAllDead)
            {
                gameOverState = BattleDef.BattleResult.Win;
                IsAllDeadTag = true;
            }
        }

        public override void AllDead()
        {
            var isDefendOver = Defenders.IsTeamDead;
            if (isDefendOver)
            {
                gameOverState = BattleDef.BattleResult.Win;
                IsAllDeadTag = false;
                IsBattleOverTag = true;
                return;
            }
            //清理，重新初始化战斗者
            AllFighter.Clear();
            AllPet.Clear();
        }
    }
}
