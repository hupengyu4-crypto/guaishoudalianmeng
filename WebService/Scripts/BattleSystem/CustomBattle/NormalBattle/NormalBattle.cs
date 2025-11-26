#if UNDEF_BATTLE_UNITY_EDITOR
#undef UNITY_EDITOR
#endif

using System;
using System.Collections.Generic;

namespace BattleSystem
{
    public class NormalBattle : BaseBattle
    {
        /// <summary>
        /// 满员状态数量
        /// </summary>
        public const int Const_MaxCount = 10;
        /// <summary>
        /// 进攻方队伍数据
        /// </summary>
        public NormalTeamData Attackers { get; protected set; }
        /// <summary>
        /// 防守方队伍数据
        /// </summary>
        public NormalTeamData Defenders { get; protected set; }
        /// <summary>
        /// 当前出手顺序
        /// </summary>
        public int FightIndex { get; protected set; }
        /// <summary>
        /// 正在战斗的战斗者
        /// </summary>
        public Fighter FireFighter { get; protected set; }
        /// <summary>
        /// 战斗数据
        /// </summary>
        public battle_field BattleData { get; protected set; }
        /// <summary>
        /// 所有战斗者
        /// </summary>
        public List<Fighter> AllFighter { get; protected set; } = new List<Fighter>(Const_MaxCount);
        /// <summary>
        /// 所有宠物
        /// </summary>
        public List<Fighter> AllPet { get; protected set; } = new List<Fighter>(2);
        /// <summary>
        /// 初始化完成
        /// </summary>
        public bool InitSuc;
        /// <summary>
        /// 第一个大回合
        /// </summary>
        private bool mIsFirstBout;
        /// <summary>
        /// 延迟技能
        /// </summary>
        private struct DelaySkill
        {
            /// <summary>
            /// 施法者
            /// </summary>
            public long authorUid;
            /// <summary>
            /// 技能Sid
            /// </summary>
            public long skillSid;
            /// <summary>
            /// 技能等级
            /// </summary>
            public int skillLevel;
            /// <summary>
            /// 技能类型
            /// </summary>
            public BattleDef.ESkillType skillType;
            /// <summary>
            /// 特定的目标
            /// </summary>
            public long specificTargetUid;
        }
        /// <summary>
        /// 延迟释放技能
        /// </summary>
        private List<DelaySkill> mDelaySkill = new List<DelaySkill> { };

        /// <summary>
        /// AllDead里延迟删除的fighter
        /// </summary>
        private List<long> mDelayRemoveFighters = new();

        private HashSet<long> mActOnceMoreFighterHashSet = new();
        private List<Fighter> mActOnceMoreFighters = new();

        public NormalBattle(long uid, BattleSceneCfg cfg) : base(uid, cfg)
        {
            InitSuc= false;
        }

        public override void Dispose()
        {
            base.Dispose();
            AllFighter.Clear();
            Attackers.Dispose();
            Defenders.Dispose();
            mDelaySkill.Clear();
            mDelayRemoveFighters.Clear();
            AllPet.Clear();
            InitSuc = false;
        }

        public override void InitData(object battleData)
        {
#if UNITY_EDITOR
            AddInfo("初始化战场开始", true);
            AddInfo("战斗中能回血？ ").AddInfo(!CanNotHeal, true);
#endif

            BattleData = (battle_field)battleData;
            int seed = (int)BattleData.Seed;
#if UNITY_EDITOR
            AddInfo("随机种子(转换前):").AddInfo(BattleData.Seed, true);
            AddInfo("随机种子(转换后):").AddInfo(seed, true);
#endif
            Random.SetSeed(seed);

            Bout = 0;
            FightIndex = 0;
            InitTeamData();
            InitFighter();

            DispatchEvent(this, Event.InitEnd);
            InitSuc = true;
        }

        /// <summary>
        /// 初始化队伍
        /// </summary>
        protected virtual void InitTeamData()
        {
            //进攻方
            Attackers = new NormalTeamData(BattleDef.TeamCampType.Attacker, this);
            Attackers.InitBattlers(BattleData.AtkCamp);
            //防守方
            Defenders = new NormalTeamData(BattleDef.TeamCampType.Defender, this);
            Defenders.InitBattlers(BattleData.DfsCamp);
        }

        /// <summary>
        /// 初始化战斗者
        /// </summary>
        protected virtual void InitFighter()
        {
            mIsFirstBout = true;
            IsAllDeadTag = Attackers.AllFighters.Count == 0 || Defenders.AllFighters.Count == 0;

            //添加双方战斗者
            for (var i = 0; i < Attackers.AllFighters.Count; i++)
            {
                AllFighter.Add(Attackers.AllFighters[i]);
            }
            for (var i = 0; i < Defenders.AllFighters.Count; i++)
            {
                AllFighter.Add(Defenders.AllFighters[i]);
            }
            if (Attackers.pet != null)
            {
                AllPet.Add(Attackers.pet);
            }
            if (Defenders.pet != null)
            {
                AllPet.Add(Defenders.pet);
            }

            DispatchEvent(this, Event.OpeningAnimation);

            //初始化完成前第一次排序，为开场技能和被动技能准备
            SortFighter();


            FightIndex = AllFighter.Count + AllPet.Count; //默认下所有人都出手，大回合切换时会重置

            DispatchEvent(this, Event.DoOpenSkills);

            for (int i = 0; i < AllPet.Count; i++)
            {
                //宠物无法死亡不验证
                AllPet[i].DoOpenSkills();
            }

            for (int i = 0; i < AllFighter.Count; i++)
            {
                if (!AllFighter[i].IsDead)
                {
                    AllFighter[i].DoOpenSkills();
                }
            }

            DispatchEvent(this, Event.DoPassiveSkills);

            for (int i = 0; i < AllPet.Count; i++)
            {
                //宠物无法死亡不验证
                AllPet[i].DoPassiveSkills();
            }
            for (int i = 0; i < AllFighter.Count; i++)
            {
                if (!AllFighter[i].IsDead)
                {
                    AllFighter[i].DoPassiveSkills();
                }
            }

        }

        private bool CanPetDoRound(Fighter pet)
        {
            if (pet == null)
            {
                return false;
            }

            var temp = FireFighter.CampType == BattleDef.TeamCampType.Attacker ? Attackers : Defenders;
            var fighters = temp?.AllFighters;
            var count = fighters == null ? 0 : fighters.Count;
            for (int i = 0; i < count; i++)
            {
                var fighter = fighters[i];
                if (fighter != null && !fighter.IsDead)
                {
                    return true;
                }
            }

            return false;
        }

        protected override void OnUpdateLogic()
        {
            if (Attackers.IsAllDead || Defenders.IsAllDead)
                return;
            int petCount = AllPet.Count;
            int AllCount = AllFighter.Count + petCount;
            //所有人都出手了，大回合结束
            if (FightIndex >= AllCount && mActOnceMoreFighters.Count <= 0)
            {
                mActOnceMoreFighterHashSet.Clear();
                //刚开始不存在回合结束
                if (mIsFirstBout)
                {
                    mIsFirstBout = false;
                }
                else
                {
                    DispatchEvent(this, Event.BoutEndShow);
                    DispatchEvent(this, Event.BoutEnd);
                    //先执行宠物  后执行英雄
                    for (int i = 0; i < AllPet.Count; i++)
                    {
                        AllPet[i].OnBoutEnd();
                    }

                    for (int i = 0; i < AllFighter.Count; i++)
                    {
                        AllFighter[i].OnBoutEnd();
                    }

                    DispatchEvent(this, Event.BoutEndShowEnd);
                }

                Bout++;
                FightIndex = 0;
                if (Bout > LimitBout)
                {
#if UNITY_EDITOR
                    AddInfo("超过最大回合，攻击者失败", true);
#endif
                    gameOverState = BattleDef.BattleResult.Fail;
                    IsBattleOverTag = true;
                    return;
                }

                //新回合
                DispatchEvent(this, Event.BoutStartShow);

                SortFighter();

                DispatchEvent(this, Event.BoutStart);
                //先执行宠物  后执行英雄
                for (int i = 0; i < AllPet.Count; i++)
                {
                    AllPet[i].OnBoutStart();
                }

                for (int i = 0; i < AllFighter.Count; i++)
                {
                    AllFighter[i].OnBoutStart();
                }

                DispatchEvent(this, Event.BoutStartShowEnd);
            }
            //延迟技能
            if (mDelaySkill.Count > 0)
            {
                DelaySkill skill = mDelaySkill[0];
                mDelaySkill.RemoveAt(0);
                Fighter f = GetSceneObject<Fighter>(skill.authorUid);
                if (f != null&& !f.IsDead && !f.IsCannotAction())
                {
                    var didSkill = f.DoSkill(skill.skillSid, skill.skillLevel, skill.specificTargetUid);
                    //标记死亡状态的战斗者执行死亡流程
                    foreach (var fighter in AllFighter)
                    {
                        if (fighter.IsState(BattleObject.State.Dead) && !fighter.IsDoDeadTag)
                        {
                            fighter.Dead();
                        }
                    }

                    if (skill.skillType == BattleDef.ESkillType.Normal && !f.IsState(BattleObject.State.Disarm))
                    {
                        DispatchEvent(this, Event.SomeoneDoNormalSkill, EventTwoParams<Fighter, BattleSkill>.Create(this, f, didSkill));
                    }
                    else if (skill.skillType == BattleDef.ESkillType.Rage && !f.IsState(BattleObject.State.Silence))
                    {
                        DispatchEvent(this, Event.SomeoneDoRageSkill, EventTwoParams<Fighter, BattleSkill>.Create(this, f, didSkill));
                    }
                }
                return;
            }

            while (mActOnceMoreFighters.Count > 0)
            {
                var actFighter = mActOnceMoreFighters[0];
                mActOnceMoreFighters.RemoveAt(0);
                if (actFighter == null || actFighter.Data == null)
                {
                    continue;
                }

                var acted = false;
                if (actFighter.Data.jobType == BattleDef.EJobType.Pet)
                {
                    if (CanPetDoRound(actFighter)) //IsAllDead是判断的IsDoDeadTag，IsDoDeadTag是这个调用结束后设置的，不满足这里的需求 by ww
                    {
                        acted = true;
                        DispatchEvent(this, Event.RoundStart);
                        actFighter.OnRoundStart();
                        actFighter.OnUpdate();
                        actFighter.OnRoundEnd();
                        DispatchEvent(this, Event.RoundEnd);
                    }
                }
                else
                {
                    if (!actFighter.IsDead && !actFighter.IsCannotAction())
                    {
                        acted = true;
                        DispatchEvent(this, Event.RoundStart);
                        actFighter.OnRoundStart();
                        actFighter.OnUpdate();
                        actFighter.OnRoundEnd();
                        DispatchEvent(this, Event.RoundEnd);
                    }
                }

                if (!acted)
                {
                    continue;
                }

                //标记死亡状态的战斗者执行死亡流程
                foreach (var fighter in AllFighter)
                {
                    if (fighter.IsState(BattleObject.State.Dead) && !fighter.IsDoDeadTag)
                    {
                        fighter.Dead();
                    }
                }
                return;
            }

            //判断当前小回合出手者
            //先执行宠物  后执行英雄
            if (FightIndex < petCount)
            {
                FireFighter = AllPet[FightIndex];
                //var campAllDead = FireFighter.CampType == BattleDef.TeamCampType.Attacker ? Attackers.IsAllDead : Defenders.IsAllDead;
                if (CanPetDoRound(FireFighter)) //IsAllDead是判断的IsDoDeadTag，IsDoDeadTag是这个调用结束后设置的，不满足这里的需求 by ww
                {
                    DispatchEvent(this, Event.RoundStart);
                    FireFighter.OnRoundStart();
                    FireFighter.OnUpdate();
                    FireFighter.OnRoundEnd();
                    DispatchEvent(this, Event.RoundEnd, EventParams<Fighter>.Create(this, FireFighter));
                }
            }
            else
            {
                FireFighter = AllFighter[FightIndex - petCount];
                if (!FireFighter.IsDead && !FireFighter.IsCannotAction())
                {
                    DispatchEvent(this, Event.RoundStart);
                    FireFighter.OnRoundStart();
                    FireFighter.OnUpdate();
                    FireFighter.OnRoundEnd();
                    DispatchEvent(this, Event.RoundEnd, EventParams<Fighter>.Create(this, FireFighter));
                }
#if UNITY_EDITOR
                else
                {
                    if (!FireFighter.IsDead)
                    {
                        AddInfo(FireFighter.GetBaseDesc()).AddInfo("被控制中，无法行动", true);
                    }
                }
#endif
            }


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

            FightIndex++;
        }

        public override void BeginDead()
        {
            if (IsBattleOver || IsBattleOverTag)
                return;
            bool selfAll = Attackers.IsAllDead;
            if (selfAll || Defenders.IsAllDead)
            {
                gameOverState = selfAll ? BattleDef.BattleResult.Fail : BattleDef.BattleResult.Win;
                IsAllDeadTag = true;
            }
        }

        public override void AllDead()
        {
            //团灭不会游戏结束，而是检查有没有人可以轮换，没有才结束
            var isAttackOver = Attackers.IsTeamDead;
            var isDefendOver = Defenders.IsTeamDead;
            if (isAttackOver || isDefendOver)
            {
                gameOverState = isAttackOver ? BattleDef.BattleResult.Fail : BattleDef.BattleResult.Win;
                IsAllDeadTag = false;
                IsBattleOverTag = true;
                return;
            }
            //继续
            Attackers.UpdateNextFighter(mDelayRemoveFighters);
            Defenders.UpdateNextFighter(mDelayRemoveFighters);

            for (int i = 0, l = mDelayRemoveFighters.Count; i < l; i++)
            {
                var fighterUid = mDelayRemoveFighters[i];
                RemoveSceneObject(fighterUid);
            }
            mDelayRemoveFighters.Clear();

#if UNITY_EDITOR
            AddInfo("重新整理上阵数据完成", true);
#endif

            //清理，重新初始化战斗者
            AllFighter.Clear();
            AllPet.Clear();
            InitFighter();
        }

        public override void OnBattleOver()
        {
            IsBattleOverTag = false;
            IsBattleOver = true;
        }

        protected override void AddBattleInfoOnBattleOver()
        {
            #region 大数据战报
#if UNITY_EDITOR
            AddInfo("统计数据：", true);
            for (int i = 0, l = AllFighter.Count; i < l; i++)
            {
                var fighter = AllFighter[i];
                AddInfo(fighter.GetBaseDesc()).AddInfo(": ", true);
                var statisticsData = fighter.statisticsData;
                AddInfo($"    存活回合数:{statisticsData.aliveBout}, 被控回合数:{statisticsData.beControlledNum}, 主动技能释放次数:{statisticsData.skillNum}, 暴击次数:{statisticsData.critNum}, 格挡次数:{statisticsData.blockNum}", true);
            }
#endif
            #endregion
        }

        public override bool IsPve()
        {
            return BattleData.Type == "fb_common" || BattleData.Type == "fb_fire";
        }

        public override BattleInfoSys AddInfo(string info, bool isNewLine = false)
        {
            return BattleInfo?.AddInfo(info, isNewLine);
        }

        /// <summary>
        /// 重新排出手顺序
        /// </summary>
        protected virtual void SortFighter()
        {
            //站位不决定出手顺序,由双方武将的速度决定,速度越高,出手优先级越高,若双方存在速度相同的英雄,由团队总速度决定出手顺序
            //同队同速度英雄，由战位优先级决定
            //双方存在相同速度英雄，攻先手
            var attackTotalSpeed = Attackers.GetTotalSpeed();
            var defendTotalSpeed = Defenders.GetTotalSpeed();
            var isSameTotalSpeed = Math.Abs(attackTotalSpeed - defendTotalSpeed) < 0.01;
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
                    if (left.CampType != right.CampType)
                    {
                        if (isSameTotalSpeed)
                        {
                            return right.CampType == BattleDef.TeamCampType.Attacker ? 1 : -1;
                        }
                        return attackTotalSpeed < defendTotalSpeed ? 1 : -1;
                    }
                    if (left.Data.Pos != right.Data.Pos)
                    {
                        return left.Data.Pos > right.Data.Pos ? 1 : -1;
                    }
                    return left.Uid < right.Uid ? -1 : 1; //return 0;
                }
                return leftSp < rightSp ? 1 : -1;
            }));

            //宠物以队伍总速度排序
            if (AllPet.Count > 1)
            {
                AllPet.Clear();
                if (attackTotalSpeed > defendTotalSpeed)
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
        /// <summary>
        /// 立即一个战斗者到空闲位置
        /// </summary>
        internal void AddFighterInEmptyPos(BattleDef.TeamCampType camp, fighter_info fighterInfo)
        {
            Fighter f;
            if (camp == BattleDef.TeamCampType.Attacker)
            {
                f = Attackers.AddFighterInEmptyPos(fighterInfo);
            }
            else
            {
                f = Defenders.AddFighterInEmptyPos(fighterInfo);
            }
            if (f == null)
            {
                return;
            }
            AllFighter.Add(f);
            DispatchEvent(this, Event.AddNewFighter, EventParams<Fighter>.Create(this, f));
            f.DoOpenSkills();
            f.DoPassiveSkills();
        }

        /// <summary>
        /// 添加延迟技能 用于再次使用技能防止卡死
        /// </summary>
        /// <param name="authorUid"></param>
        /// <param name="skillSid"></param>
        /// <param name="skillLevel"></param>
        /// <param name="skillType"></param>
        /// <param name="specificTargetUid"></param>
        internal void AddDelaySkill(long authorUid, long skillSid, int skillLevel, BattleDef.ESkillType skillType, long specificTargetUid = 0)
        {
            mDelaySkill.Add(new DelaySkill()
            {
                authorUid = authorUid,
                skillSid = skillSid,
                skillLevel = skillLevel,
                skillType = skillType,
                specificTargetUid = specificTargetUid
            });
        }

        internal virtual bool CanInitBattlerNow(battler battler, int index)
        {
            return true;
        }

        internal bool AddActOnceMoreFighter(Fighter fighter)
        {
            if (fighter == null || !mActOnceMoreFighterHashSet.Add(fighter.Uid))
            {
                return false;
            }

            mActOnceMoreFighters.Add(fighter);
            return true;
        }

        protected void OnTeamFighterEnd()
        {
            mDelaySkill.Clear();
            mActOnceMoreFighters.Clear();
        }
    }
}
