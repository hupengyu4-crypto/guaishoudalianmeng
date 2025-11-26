#if UNDEF_BATTLE_UNITY_EDITOR
#undef UNITY_EDITOR
#endif

using System;
using System.Collections.Generic;

namespace BattleSystem
{
    /// <summary>
    /// Boss战斗
    /// </summary>
    public class PVPBattle : NormalBattle
    {
        List<Fighter> mAtkFighters = new List<Fighter>();
        List<Fighter> mDefFighters = new List<Fighter>();
        /// <summary>
        /// 是否进攻方优先出手
        /// </summary>
        bool mIsAtkPriority;
        public PVPBattle(long uid, BattleSceneCfg cfg) : base(uid, cfg)
        {
        }
        protected override void InitTeamData()
        {
            base.InitTeamData();
            // long atkSpeed = BattleDef.PVPBaseTotalSpeed;
            // long defSpeed = BattleDef.PVPBaseTotalSpeed;
            long atkSpeed = 0;
            long defSpeed = 0;
            foreach (var fighter in Attackers.AllFighters)
            {
                atkSpeed += fighter.Data.GetProp(BattleDef.Property.speed);
            }
            foreach (var fighter in Defenders.AllFighters)
            {
                defSpeed += fighter.Data.GetProp(BattleDef.Property.speed);
            }
            var speedPower = BattleDef.TotalSpeedPower;
            var adjustAtkSpeed = (long)Math.Ceiling(Math.Pow(atkSpeed, speedPower));
            var adjustDefSpeed = (long)Math.Ceiling(Math.Pow(defSpeed, speedPower));
            float randomValue = Random.RandomValue(0, adjustAtkSpeed + adjustDefSpeed);
            mIsAtkPriority = randomValue <= adjustAtkSpeed;

            //mIsAtkPriority = Random.RandomValue(0, 100) <= 50;
#if UNITY_EDITOR
            // AddInfo($"是否进攻方优先出手 {mIsAtkPriority}, 随机值: {randomValue}, 基底速度：{BattleDef.PVPBaseTotalSpeed}", true);
            // AddInfo($"进攻方速度：{atkSpeed - BattleDef.PVPBaseTotalSpeed}, 进攻方加基底后的速度：{atkSpeed}", true);
            // AddInfo($"防守方速度：{defSpeed - BattleDef.PVPBaseTotalSpeed}, 防守方加基底后的速度：{defSpeed}", true);
            AddInfo($"是否进攻方优先出手 {mIsAtkPriority}, 随机值: {randomValue}, 次方：{speedPower}", true);
            AddInfo($"进攻方速度：{atkSpeed}, 进攻方次方后的速度：{adjustAtkSpeed}", true);
            AddInfo($"防守方速度：{defSpeed}, 防守方次方后的速度：{adjustDefSpeed}", true);
#endif
        }
        protected override void SortFighter()
        {
            /*对战的A方和B方轮流出手,各方内的武将则按速度大小排序轮流出手.
              例如进攻方为A1 - A5  防守方B1 - B5  按照各自阵容的速度排序
              出手顺序为随机一方先出手
              进攻方A1优先出手,接下来B1,A2,B2依次类推*/
            mAtkFighters.Clear();
            mDefFighters.Clear();
            foreach (var fighter in AllFighter)
            {
                if (fighter.CampType == BattleDef.TeamCampType.Attacker)
                {
                    mAtkFighters.Add(fighter);
                }
                else
                {
                    mDefFighters.Add(fighter);
                }
            }
            mAtkFighters.Sort((left, right) =>
            {
                // 先检查 null 值的情况，按需返回负数、0 或正数
                if (left == null && right == null)
                    return 0;
                if (left == null)
                    return -1;
                if (right == null)
                    return 1;
                long leftSp = left.Data.GetProp(BattleDef.Property.speed);
                long rightSp = right.Data.GetProp(BattleDef.Property.speed);
                if (leftSp == rightSp)
                    return left.Uid < right.Uid ? -1 : 1; //return 0;
                return leftSp < rightSp ? 1 : -1;
            });
            mDefFighters.Sort((left, right) =>
            {
                // 先检查 null 值的情况，按需返回负数、0 或正数
                if (left == null && right == null)
                    return 0;
                if (left == null)
                    return -1;
                if (right == null)
                    return 1;
                long leftSp = left.Data.GetProp(BattleDef.Property.speed);
                long rightSp = right.Data.GetProp(BattleDef.Property.speed);
                if (leftSp == rightSp)
                    return left.Uid < right.Uid ? -1 : 1; //return 0;
                return leftSp < rightSp ? 1 : -1;
            });
            AllFighter.Clear();
            int atkCount = mAtkFighters.Count;
            int defCount = mDefFighters.Count;
            int count = atkCount > defCount ? atkCount : defCount;
            for (int i = 0; i < count; i++)
            {
                if (mIsAtkPriority)
                {
                    if (i < atkCount)
                    {
                        AllFighter.Add(mAtkFighters[i]);
                    }
                    if (i < defCount)
                    {
                        AllFighter.Add(mDefFighters[i]);
                    }
                }
                else
                {
                    if (i < defCount)
                    {
                        AllFighter.Add(mDefFighters[i]);
                    }
                    if (i < atkCount)
                    {
                        AllFighter.Add(mAtkFighters[i]);
                    }
                }
            }
            //宠物以队伍第一次随机顺序为准
            if (AllPet.Count > 1)
            {
                AllPet.Clear();
                if (mIsAtkPriority)
                {
                    AllPet.Add(Attackers.pet);
                    AllPet.Add(Defenders.pet);
                }
                else
                {
                    AllPet.Add(Defenders.pet);
                    AllPet.Add(Attackers.pet);
                }
            }
        }
        public override void OnBattleOver()
        {
            base.OnBattleOver();
            if (Bout > LimitBout)
            {
                IsBattleOver = true;
                double atkLossHp = 0;
                double defLossHp = 0;
                foreach (var fighter in AllFighter)
                {
                    if (fighter.CampType == BattleDef.TeamCampType.Attacker)
                    {
                        atkLossHp += fighter.LossHp;
                    }
                    else
                    {
                        defLossHp += fighter.LossHp;
                    }
                }
                gameOverState = atkLossHp < defLossHp ? BattleDef.BattleResult.Win : BattleDef.BattleResult.Fail;
            }
        }
    }
}