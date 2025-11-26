#if UNDEF_BATTLE_UNITY_EDITOR
#undef UNITY_EDITOR
#endif

using System;
using System.Collections.Generic;
using System.Text;

namespace BattleSystem
{
    /// <summary>
    /// 战斗者
    /// </summary>
    public class Fighter : BattleObject
    {
        /// <summary>
        /// 战斗者数据
        /// </summary>
        public FighterData Data { get; private set; }

        /// <summary>
        /// 队伍数据
        /// </summary>
        public NormalTeamData TeamData { get; private set; }

        /// <summary>
        /// 缓存的索敌目标
        /// </summary>
        public List<Fighter> CacheTargets { get; set; }

        /// <summary>
        /// 角色血量百分比
        /// </summary>
        public double HpPercent => (double)Data[BattleDef.Property.hp] / Data[BattleDef.Property.max_hp];

        /// <summary>
        /// 损失血量
        /// </summary>
        public double LossHp => Data[BattleDef.Property.max_hp] - Data[BattleDef.Property.hp];

        /// <summary>
        /// 记录扣血(用于血量同步)
        /// </summary>
        public long recordSubHp;

        /// <summary>
        /// 记录造成的伤害（用于伤害同步）
        /// </summary>
        public long recordDamage = 0;

        /// <summary>
        /// 累计造成普攻伤害
        /// </summary>
        public double causeNormalDamage;

        /// <summary>
        /// 累计受到普攻伤害
        /// </summary>
        public double hitNormalDamage;

        /// <summary>
        /// 当前护盾值
        /// </summary>
        private long mShieldValue;

        /// <summary>
        /// 当前特殊护盾值
        /// </summary>
        private long mSpecialShieldValue;

        /// <summary>
        /// 文字处理
        /// </summary>
        private StringBuilder mStringBuilder;

        /// <summary>
        /// 额外伤害系数
        /// </summary>
        private double mDamageFactor = 1;
        /// <summary>
        /// 额外伤害系数，始终生效
        /// </summary>
        private double mDamageFactorAlways = 1;
        /// <summary>
        /// 额外伤害减免
        /// </summary>
        private double mDamageReduce = 0;
        /// <summary>
        /// 每次伤害死血比例
        /// </summary>
        private double mDeadHpRatio;
        /// <summary>
        /// 大回合结束死血比例
        /// </summary>
        private double mBoutDeadHpRatio;

        private LinkedList<long> mTauntedFighters = null;

        private double _comboProbability = 0.0d;
        public double comboProbability
        {
            get => _comboProbability;
            set => _comboProbability = Math.Max(0.0d, value);
        }

        //internal List<BaseEffect> forResistEffectData;

        public class StatisticsData
        {
            public long aliveBout;      //存活回合数
            public long beControlledNum;//被控回合数
            public long skillNum;       //主动技能释放次数
            public long critNum;        //暴击次数
            public long blockNum;       //格挡次数
        }

        /// <summary>
        /// 统计数据
        /// </summary>
        public StatisticsData statisticsData { get; private set; }

        /// <summary>
        /// 记录效果改变的属性
        /// </summary>
        private Dictionary<BattleDef.Property, long> mChangeProperty =
            new Dictionary<BattleDef.Property, long>((int)BattleDef.Property.MaxCount);

        /// <summary>
        /// 该回合内是否被控制
        /// </summary>
        private bool _beControlledThisBout;
        private static readonly State _beControlledState;

        static Fighter()
        {
            //眩晕 冰冻 沉默
            _beControlledState = State.Dizzy | State.Frozen | State.Paralysis;
        }

        public Fighter(NormalBattle battle) : base(battle)
        {
            Battle = battle;
            if (battle != null)
            {
                mDeadHpRatio = battle.BattleData.DeadHpIn * BattleDef.Percent100;
                mBoutDeadHpRatio = battle.BattleData.DeadHpRound * BattleDef.Percent100;
            }
            Data = new FighterData(this);
            statisticsData = new StatisticsData();
        }

        private void SetSomeInitData()
        {
            Data.SetInitProp(BattleDef.Property.heal, (long)BattleDef.TenThousand);
            Data.SetInitProp(BattleDef.Property.be_heal, (long)BattleDef.TenThousand);
            Data.SetInitProp(BattleDef.Property.shield_add_factor, (long)BattleDef.TenThousand);
            Data.SetInitProp(BattleDef.Property.normal_damage_add, (long)BattleDef.TenThousand);
            Data.SetInitProp(BattleDef.Property.normal_damage_reduce, (long)BattleDef.TenThousand);
        }

        public void SetData(fighter_info fighterData, NormalTeamData teamData)
        {
            Uid = fighterData.Uid;
            TeamData = teamData;
            CampType = teamData.CampType;
            Data.SetData(fighterData);
            SetSomeInitData();
            isOnlySelfAddEffect = Data.jobType == BattleDef.EJobType.Pet;
            if (Data[BattleDef.Property.hp] <= 0 && !IsDead)
            {
                //通过标记去让死亡逻辑后面再处理，避免提起清理了效果
                IsDead = true;
                IsDoDeadTag = false;
                ClearState();
                AddState(State.Dead, Uid);
            }
            Battle.AddSceneObject(this);
            Battle.BattleInfo?.ListenObjectInfo(this);
        }

        public override void OnAwake()
        {
        }

        public override void OnStart()
        {
        }
        private void DoNormalSkill()
        {
            if (Data.normalSkill == null) {  return; }
            if (Data.normalSkill.SkillType == BattleDef.ESkillType.Normal && IsState(BattleObject.State.Disarm))
            {
#if UNITY_EDITOR
                Battle.AddInfo("缴械中，不能释放普攻:").AddInfo(Data.normalSkill.Sid).AddInfo("，施法者:").AddInfo(GetBaseDesc(), true);
#endif
                return;
            }
            DoSkill(Data.normalSkill);
            Battle.DispatchEvent(Battle, BaseBattle.Event.SomeoneDoNormalSkill, EventTwoParams<Fighter, BattleSkill>.Create(Battle, this, Data.normalSkill));
        }

        private bool ShenWeiNormalSkillMore()
        {
            var shenWeiNormalSkillLimit = BattleDef.ShenWeiNormalSkillLimit;
            if (shenWeiNormalSkillLimit <= 0 || !IsState(State.ShenWei))
            {
                return false;
            }

            var count = 0;
            var allEffectList = Effect.AllEffectList;
            for (int i = 0, l = allEffectList.Count; i < l; i++)
            {
                var e = allEffectList[i];
                if (e == null)
                {
                    continue;
                }

                if (e.IsType(EffectSys.EffectType.ShenWei))
                {
                    if (e.IsType(EffectSys.EffectType.Overlays))
                    {
                        count += e.overlayCount;
                    }
                    else
                    {
                        count++;
                    }

                    if (count >= shenWeiNormalSkillLimit)
                    {
                        break;
                    }
                }
            }

            count = System.Math.Min(count, shenWeiNormalSkillLimit);
            for (int i = 0; i < count; i++)
            {
                DoNormalSkill();
            }

            return true;
        }

        private void InnerUpdate()
        {
            if (IsCanUseRageSkill())
            {
                if (Data.rageSkill != null && Data[BattleDef.Property.anger] >= Data.rageSkill.Cfg.rage)
                {
                    //满怒放技能
                    DoSkill(Data.rageSkill);
                    Battle.DispatchEvent(Battle, BaseBattle.Event.SomeoneDoRageSkill, EventTwoParams<Fighter, BattleSkill>.Create(Battle, this, Data.rageSkill));
                    return;
                }
            }
#if UNITY_EDITOR
            else
            {
                Battle.AddInfo(GetBaseDesc()).AddInfo("沉默中，使用普攻", true);
            }
#endif
            //普攻
            DoNormalSkill();

            if (_comboProbability > 0.0d)
            {
                if (Battle.Random.RandomValue(0d, 100d) <= _comboProbability)
                {
                    DoNormalSkill();
                }
            }

            ShenWeiNormalSkillMore();
        }

        public override void OnUpdate()
        {
            InnerUpdate();
            if (IsState(State.ActAgain))
            {
                DispatchEvent(Battle, Event.ActAgain, EventParams<Fighter>.Create(Battle, this));
                InnerUpdate();
            }
        }

        /// <summary>
        /// 开场生效，只有一次
        /// </summary>
        public void DoOpenSkills()
        {
            var skills = Data.openingSkills;
            if (skills != null)
            {
                for (int i = 0; i < skills.Count; i++)
                {
                    if (skills[i].SkillType == BattleDef.ESkillType.Opening)
                    {
                        DoSkill(skills[i]);
                    }
                }
            }
        }

        /// <summary>
        /// 被动生效，只有一次
        /// </summary>
        public void DoPassiveSkills()
        {
            var skills = Data.passiveSkills;
            if (skills != null)
            {
                for (int i = 0; i < skills.Count; i++)
                {
                    if (skills[i].SkillType == BattleDef.ESkillType.Passive)
                    {
                        DoSkill(skills[i]);
                    }
                }
            }
        }

        /// <summary>
        /// 使用技能
        /// </summary>
        /// <param name="battleSkill">技能</param>
        /// <param name="specificTargetUid"></param>
        public void DoSkill(BattleSkill battleSkill, long specificTargetUid = 0)
        {
            if (battleSkill == null)
                return;
            battleSkill.Do(specificTargetUid);
        }

        /// <summary>
        /// 使用技能
        /// </summary>
        /// <param name="sid">技能sid</param>
        /// <param name="level"></param>
        /// <param name="specificTargetUid"></param>
        public BattleSkill DoSkill(long sid, int level, long specificTargetUid = 0)
        {
            var skill = new BattleSkill(sid, level, this);
            skill.Do(specificTargetUid);
            return skill;
        }
        #region 回合
        /// <summary>
        /// 回合开始
        /// </summary>
        public void OnBoutStart()
        {
            if (!IsDead)
            {
                statisticsData.aliveBout++;

                if (IsState(_beControlledState))
                {
                    _beControlledThisBout = true;
                    statisticsData.beControlledNum++;
                }
            }

            Effect.OnBoutStart();
        }

        /// <summary>
        /// 回合结束
        /// </summary>
        public void OnBoutEnd()
        {
            if (!IsDead && !_beControlledThisBout && IsState(_beControlledState))
            {
                statisticsData.beControlledNum++;
            }
            _beControlledThisBout = false;

            Effect.OnBoutEnd();

            if (mBoutDeadHpRatio > 0 && !IsDead)
            {
                long maxHp = Data.nowProps[(int)BattleDef.Property.max_hp];
                long hp = Data.nowProps[(int)BattleDef.Property.hp];
                long deadHp = Data.nowProps[(int)BattleDef.Property.dead_hp];
                long hurtHp = maxHp - hp - deadHp;
                if (hurtHp > 0)
                {
                    long addDeadHp = (long)(hurtHp * mBoutDeadHpRatio);
                    if (addDeadHp > 0)
                    {
                        PropParams deadHpParam = Battle.CreateEventParam<PropParams>();
                        deadHpParam.property = BattleDef.Property.dead_hp;
                        deadHpParam.authorUid = Uid;
                        deadHpParam.dataValue = deadHp;
                        deadHpParam.newValue = deadHp + addDeadHp;
                        deadHpParam.IsAutoRelease = false;
                        Data.nowProps[(int)BattleDef.Property.dead_hp] = deadHpParam.newValue;
                        DispatchEvent(Battle, Event.Prop, deadHpParam);
                        //回收
                        deadHpParam.IsAutoRelease = true;
                        Battle.ReleaseEventParam(deadHpParam);
                    }
                }
            }
        }

        /// <summary>
        /// 小回合开始
        /// </summary>
        public void OnRoundStart()
        {
            Effect.OnRoundStart();
        }

        /// <summary>
        /// 小回合结束
        /// </summary>
        public void OnRoundEnd()
        {
            Effect.OnRoundEnd();
        }
        #endregion
        #region 属性变化

        /// <summary>
        /// 修改属性正数为加负数为减
        /// </summary>
        /// <param name="property">属性类型</param>
        /// <param name="addValue">改变值</param>
        /// <param name="authorUid"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual long AddProp(BattleDef.Property property, long addValue, long authorUid = 0,
            PropParams param = null)
        {
            var oldV = Data[property];
            return SetProp(property, oldV + addValue, authorUid, param) - oldV;
        }

        /// <summary>
        /// 修改属性 外部慎用直接修改的新值
        /// </summary>
        /// <param name="property">属性类型</param>
        /// <param name="newValue">改变值</param>
        /// <param name="authorUid"></param>
        /// <param name="param"></param>
        /// <returns>实际新的值</returns>
        public long SetProp(BattleDef.Property property, long newValue, long authorUid = 0, PropParams param = null)
        {
            if (property == BattleDef.Property.max_hp)
            {
#if UNITY_EDITOR
                Log.error($"最大血量不允许修改 {authorUid}");
#endif
                //最大血量不允许修改
                return Data.nowProps[(int)BattleDef.Property.max_hp];
            }

            //属性不能小于0 上面也判断一下，不然可能出现0血后，又扣血(成负值，小于0)，会继续往下面走，DeadPre事件错误的抛多次 by ww
            if (newValue < 0)
            {
                newValue = 0;
            }

            var now = Data[property];
            if (newValue != now)
            {
                if (param == null)
                    param = Battle.CreateEventParam<PropParams>();
                param.property = property;
                param.authorUid = authorUid;
                param.dataValue = Data.nowProps[(int)property];
                param.newValue = newValue;
                param.IsAutoRelease = false;

                //按配置最大值修正属性
                var maxValue = BattleUtils.GetMaxPropertyValue(this, property);
                if (maxValue != 0 && param.newValue > maxValue)
                {
                    param.newValue = maxValue;
                }

                //属性不能小于0
                if (param.newValue < 0)
                {
                    param.newValue = 0;
                }
                //回血
                if (property == BattleDef.Property.hp && param.newValue > param.dataValue)
                {
                    long maxHp = Data.nowProps[(int)BattleDef.Property.max_hp] - Data.nowProps[(int)BattleDef.Property.dead_hp];
                    if (param.newValue > maxHp)
                    {
                        param.newValue = maxHp;
                    }
                }
                else if (property == BattleDef.Property.anger)
                {
                    //无法回怒状态
                    if (IsState(State.UnableAddAnger) && param.newValue > now)
                    {
                        param.newValue = now;
                    }
                }

                if (!DispatchEvent(Battle, Event.PropPre, param).IsBlockEvent)
                {
                    Data.nowProps[(int)property] = param.newValue;

                    DispatchEvent(Battle, Event.Prop, param);

                    if (property == BattleDef.Property.hp)
                    {
                        if (param.newValue < param.dataValue)
                        {
                            var lostHp = param.dataValue - param.newValue;
                            var oldLostHp = Data.nowProps[(int)BattleDef.Property.lost_hp_total];
                            Data.nowProps[(int)BattleDef.Property.lost_hp_total] = oldLostHp + lostHp;
                            Data.nowProps[(int)BattleDef.Property.losthp] = (long)LossHp;
                            PropParams lostHpParam = Battle.CreateEventParam<PropParams>();
                            lostHpParam.property = BattleDef.Property.lost_hp_total;
                            lostHpParam.authorUid = authorUid;
                            lostHpParam.dataValue = oldLostHp;
                            lostHpParam.newValue = oldLostHp + lostHp;
                            lostHpParam.IsAutoRelease = false;
                            DispatchEvent(Battle, Event.Prop, lostHpParam);
                            //回收
                            lostHpParam.IsAutoRelease = true;
                            Battle.ReleaseEventParam(lostHpParam);

                            if (mDeadHpRatio > 0)
                            {
                                long addDeadHp = (long)(lostHp * mDeadHpRatio);
                                if (addDeadHp > 0)
                                {
                                    long deadHp = Data.nowProps[(int)BattleDef.Property.dead_hp];
                                    PropParams deadHpParam = Battle.CreateEventParam<PropParams>();
                                    deadHpParam.property = BattleDef.Property.dead_hp;
                                    deadHpParam.authorUid = authorUid;
                                    deadHpParam.dataValue = deadHp;
                                    deadHpParam.newValue = deadHp + addDeadHp;
                                    deadHpParam.IsAutoRelease = false;
                                    Data.nowProps[(int)BattleDef.Property.dead_hp] = deadHpParam.newValue;
                                    DispatchEvent(Battle, Event.Prop, deadHpParam);
                                    //回收
                                    deadHpParam.IsAutoRelease = true;
                                    Battle.ReleaseEventParam(deadHpParam);
                                }
                            }
                        }

                        if (Data[BattleDef.Property.hp] <= 0 && !IsDead && !IsFakeDead)
                        {
                            Fighter fighter = null;
                            if (authorUid != 0)
                            {
                                fighter = Battle.GetSceneObject<Fighter>(authorUid);
                            }

                            if (!DispatchEvent(Battle, Event.DeadPre, EventTwoParams<Fighter, PropParams>.Create(Battle, fighter, param))
                                    .IsBlockEvent && Data[BattleDef.Property.hp] <= 0)
                            {
                                bool relive = !IsState(State.IgnoreRelive) && (IsState(State.Relive) || IsFakeDead);
                                if (relive && fighter != null && fighter.IsState(State.ZhenShi))
                                {
                                    relive = false;
                                }

                                if (!relive)
                                {
                                    IsFakeDead = false;
                                    //通过标记去让死亡逻辑后面再处理，避免提起清理了效果
                                    IsDead = true;
                                    IsDoDeadTag = false;
                                    ClearState();
                                    AddState(State.Dead, authorUid);
                                }
                                else
                                {
                                    IsFakeDead = true;
                                    DispatchEvent(Battle, Event.FakeDead);
                                    Battle.DispatchEvent(Battle, BaseBattle.Event.AnyDead, EventParams<long>.Create(Battle, Uid));
                                }

                                fighter?.DispatchEvent(Battle, Event.Kill, EventTwoParams<Fighter, PropParams>.Create(Battle, this, param));
                                DispatchEvent(Battle, Event.BeKill, EventTwoParams<Fighter, PropParams>.Create(Battle, fighter, param));
                            }
                        }
                    }
                }

                long v = param.newValue;
                //回收
                param.IsAutoRelease = true;
                Battle.ReleaseEventParam(param);
                return v;
            }

            if (param != null)
            {
                //回收
                Battle.ReleaseEventParam(param);
            }

            return now;
        }

        /// <summary>
        /// 记录属性变化
        /// </summary>
        /// <param name="property">属性</param>
        /// <param name="value">变化</param>
        public void UpdateChangeProperty(BattleDef.Property property, long value)
        {
            mChangeProperty[property] = value;
        }

        /// <summary>
        /// 获取指定属性的变化
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public long GetChangeProperty(BattleDef.Property property)
        {
            mChangeProperty.TryGetValue(property, out var value);
            return value;
        }

        #endregion

        #region 护盾

        /// <summary>
        /// 护盾
        /// </summary>
        /// <returns></returns>
        public long GetShield()
        {
            return mShieldValue;
        }

        /// <summary>
        /// 护盾
        /// </summary>
        /// <returns></returns>
        public long GetSpecialShield()
        {
            return mSpecialShieldValue;
        }

        /// <summary>
        /// 修改护盾值，支持正负
        /// </summary>
        /// <param name="shield">改变的护盾值</param>
        /// <returns>实际改变的护盾值</returns>
        public long AddShield(long shield, bool special = false)
        {
            if (shield == 0)
                return 0;
            if (shield > 0)
            {
                if (!special && IsState(State.UnableAddShield))
                {
                    return 0;
                }

                var factor = Data[BattleDef.Property.shield_add_factor] * BattleDef.Percent100;
                shield = (long)YKMath.Ceiling(shield * factor);
                if (shield <= 0)
                {
                    return 0;
                }
            }

            var oldShieldValue = special ? mSpecialShieldValue : mShieldValue;
            if (mShieldValue == 0 && mSpecialShieldValue == 0 && shield > 0)
            {
                //TODO 最大值限制？
                DispatchEvent(Battle, BattleObject.Event.ShieldAdd);
                AddState(State.Shield, 0);
                AddEvent(BattleObject.Event.BeDamagePre, OnDamagePre, false, 1); //事件等级提高到1，优先处理护盾减免相关逻辑，如果有增伤相关，需要大于1
            }
            long maxShield = (long)YKMath.Ceiling(Data[BattleDef.Property.max_hp] * BattleDef.MaxShieldPerPer);
            if (special)
            {
                mSpecialShieldValue += shield;
                if (mSpecialShieldValue > maxShield)
                {
                    mSpecialShieldValue = maxShield;
                }
            }
            else
            {
                mShieldValue += shield;
                if (mShieldValue > maxShield)
                {
                    mShieldValue = maxShield;
                }
            }

            DispatchEvent(Battle, BattleObject.Event.ShieldChange, EventTwoParams<long, long>.Create(Battle, shield, mShieldValue));
#if UNITY_EDITOR
            if (special)
                Battle.AddInfo(GetBaseDesc()).AddInfo("特殊护盾改变：").AddInfo(oldShieldValue).AddInfo(">>>").AddInfo(mSpecialShieldValue, true);
            else
                Battle.AddInfo(GetBaseDesc()).AddInfo("护盾改变：").AddInfo(oldShieldValue).AddInfo(">>>").AddInfo(mShieldValue, true);
#endif
            if (mShieldValue <= 0 && mSpecialShieldValue <= 0)
            {
                mShieldValue = 0;
                mSpecialShieldValue = 0;
                RemoveEvent(BattleObject.Event.BeDamagePre, OnDamagePre);
                DispatchEvent(Battle, BattleObject.Event.ShieldRemove);
                RemoveState(State.Shield, 0);
            }
            return special ? mSpecialShieldValue - oldShieldValue : mShieldValue - oldShieldValue;
        }

        /// <summary>
        /// 按照百分比减少护盾值
        /// </summary>
        /// <param name="value">百分比值，1-100</param>
        public void RemoveShieldByPercentage(double value)
        {
            if (mShieldValue == 0)
                return;
            var newValue = (long)YKMath.Ceiling(mShieldValue * value * BattleDef.Percent);
            AddShield(-newValue);
        }

        public long ShieldEffect(long damage, long attackUid, long defendUid)
        {
            if (mShieldValue <= 0 && mSpecialShieldValue <= 0 )
            {
                return damage;
            }

            var shieldAbsorption = 0L; //承伤值
            if (mShieldValue > 0) 
            {
                if (mShieldValue >= damage)
                {
                    shieldAbsorption = damage;
                    ShieldAbsorptionParams shieldAbsorptionParams = Battle.CreateEventParam<ShieldAbsorptionParams>();
                    shieldAbsorptionParams.attackUid = attackUid;
                    shieldAbsorptionParams.defendUid = defendUid;
                    shieldAbsorptionParams.shieldAbsorptionValue = shieldAbsorption;
                    shieldAbsorptionParams.IsAutoRelease = false;
                    DispatchEvent(Battle, BattleObject.Event.ShieldAbsorption, shieldAbsorptionParams); //在AddShield之前抛出，因为AddShield可能会走ShieldRemove逻辑
                                                                                                        //回收
                    shieldAbsorptionParams.IsAutoRelease = true;
                    Battle.ReleaseEventParam(shieldAbsorptionParams);
                    AddShield(-damage);
                    damage = 0;
                }
                else
                {
                    shieldAbsorption = mShieldValue;
                    ShieldAbsorptionParams shieldAbsorptionParams = Battle.CreateEventParam<ShieldAbsorptionParams>();
                    shieldAbsorptionParams.attackUid = attackUid;
                    shieldAbsorptionParams.defendUid = defendUid;
                    shieldAbsorptionParams.shieldAbsorptionValue = shieldAbsorption;
                    shieldAbsorptionParams.IsAutoRelease = false;
                    DispatchEvent(Battle, BattleObject.Event.ShieldAbsorption, shieldAbsorptionParams); //在AddShield之前抛出，因为AddShield可能会走ShieldRemove逻辑
                                                                                                        //回收
                    shieldAbsorptionParams.IsAutoRelease = true;
                    Battle.ReleaseEventParam(shieldAbsorptionParams);
                    damage -= mShieldValue;
                    AddShield(-mShieldValue);
                }
            }
            if (mSpecialShieldValue > 0 && damage>0)
            {
                //特殊护盾目前没有需求，暂时不抛事件
                if (mSpecialShieldValue >= damage)
                {
                    AddShield(-damage, true);
                    damage = 0;
                }
                else
                {
                    damage = 0;
                    AddShield(-mSpecialShieldValue, true);
                }
            }

            return damage;
        }

        /// <summary>
        /// 伤害事件之前，修正伤害值
        /// </summary>
        /// <param name="eventParams"></param>
        private void OnDamagePre(EventParams eventParams)
        {
            if (!eventParams.IsBlockEvent && eventParams is DamageParams args)
            {
                args.newValue = ShieldEffect(args.newValue, args.attackUid, args.defendUid);
            }
        }

        #endregion

        #region 额外伤害系数增减

        /// <summary>
        /// 修改额外伤害系数，支持正负
        /// </summary>
        /// <param name="damageFactor">改变的伤害系数</param>
        public void AddDamageFactor(double damageFactor)
        {
            mDamageFactor += damageFactor;
            //if (mDamageFactor < 1)
            //{
            //    mDamageFactor = 1; //设定上不能小于1，更不能等于0，不然会打不出伤害
            //}
        }

        /// <summary>
        /// 获取额外伤害系数
        /// </summary>
        /// <returns></returns>
        public double GetDamageFactor()
        {
            return Math.Max(mDamageFactor, 1);
        }

        #endregion

        #region 额外伤害系数增减，始终生效

        /// <summary>
        /// 修改额外伤害系数，始终生效，支持正负
        /// </summary>
        /// <param name="damageFactor">改变的伤害系数</param>
        public void AddDamageFactorAlways(double damageFactor)
        {
            mDamageFactorAlways += damageFactor;
        }

        /// <summary>
        /// 获取额外伤害系数，始终生效
        /// </summary>
        /// <returns></returns>
        public double GetDamageFactorAlways()
        {
            return Math.Max(mDamageFactorAlways, 1);
        }

        #endregion

        #region 额外伤害减免增减

        /// <summary>
        /// 修改额外伤害减免，支持正负
        /// </summary>
        /// <param name="damageReduce">改变的伤害减免</param>
        public void AddDamageReduce(double damageReduce)
        {
            mDamageReduce += damageReduce;
        }

        /// <summary>
        /// 获取额外伤害减免
        /// </summary>
        /// <returns></returns>
        public double GetDamageReduce()
        {
            return YKMath.Clamp(mDamageReduce, 0.0, 1.0);
        }

        #endregion

        #region 嘲讽
        public LinkedListNode<long> AddTaunt(long taunterUid)
        {
            if (taunterUid == 0)
            {
                return null;
            }

            if (mTauntedFighters == null)
            {
                mTauntedFighters = new LinkedList<long>();
            }

            return mTauntedFighters.AddLast(taunterUid);
        }

        public void RemoveTaunt(LinkedListNode<long> taunter)
        {
            if (taunter == null || mTauntedFighters == null)
            {
                return;
            }

            mTauntedFighters.Remove(taunter);
        }

        public long GetTauntedUid()
        {
            if (mTauntedFighters == null || mTauntedFighters.Count <= 0)
            {
                return 0;
            }

            return mTauntedFighters.Last.Value;
        }
        #endregion

        /// <summary>
        ///
        /// </summary>
        /// <param name="ignoreHp">忽略HP(继血)</param>
        public void ResetInit(bool ignoreHp)
        {
            if (IsDead || IsState(State.Dead))
                return;
            Effect.ClearAll();
            ClearState();
            if (ignoreHp)
            {
                long old = Data[BattleDef.Property.hp];
                Data.ResetProp();
                if (old > Data[BattleDef.Property.max_hp])
                {
                    Data.nowProps[(int)BattleDef.Property.hp] = Data[BattleDef.Property.max_hp];
                }
                else
                {
                    Data.nowProps[(int)BattleDef.Property.hp] = old;
                }
            }
            else
            {
                Data.ResetProp();
            }
            mDamageFactor = 1;
            mDamageFactorAlways = 1;
            mDamageReduce = 0;
            mShieldValue = 0;
            Data.InitSkills();
        }

        public override string ToString()
        {
            if (!BattleDef.DebugLog)
            {
                return base.ToString();
            }

            if (mStringBuilder == null)
            {
                mStringBuilder = new StringBuilder();
            }
            else
            {
                mStringBuilder.Clear();
            }

            mStringBuilder.Append(Data.Cfg?.name).Append(",Uid:").Append(Data.Uid).Append(",Sid:").Append(Data.Sid)
                .Append(",站位:").Append(Data.Pos).Append(",阵营:")
                .Append(CampType == BattleDef.TeamCampType.Attacker ? "进攻方" : "防守方").Append(",普攻:")
                .Append(Data.normalSkill?.Sid).Append(",技能:")
                .Append(Data.rageSkill?.Sid).Append(",开场:{");
            if (Data.openingSkills != null)
            {
                int count = Data.openingSkills.Count;
                for (int i = 0; i < count; i++)
                {
                    if (i < count - 1)
                    {
                        mStringBuilder.Append(Data.openingSkills[i]?.Sid).Append("+");
                    }
                    else
                    {
                        mStringBuilder.Append(Data.openingSkills[i]?.Sid);
                    }
                }
            }

            mStringBuilder.Append("}");
            mStringBuilder.Append(",被动:{");
            if (Data.passiveSkills != null)
            {
                int count = Data.passiveSkills.Count;
                for (int i = 0; i < count; i++)
                {
                    if (i < count - 1)
                    {
                        mStringBuilder.Append(Data.passiveSkills[i]?.Sid).Append("+");
                    }
                    else
                    {
                        mStringBuilder.Append(Data.passiveSkills[i]?.Sid);
                    }
                }
            }

            mStringBuilder.Append("}");

            mStringBuilder.AppendLine();
            mStringBuilder.Append("属性:{");

            var maxCount = (int)BattleDef.Property.MaxCount;
            for (byte i = 0; i < maxCount; i++)
            {
                mStringBuilder.Append(BattleUtils.GetPropertyName(i)).Append(":").Append(Data[i]);
                if (i != maxCount - 1)
                {
                    mStringBuilder.Append(",");
                }
            }

            mStringBuilder.Append("}");

            mStringBuilder.AppendLine();


            mStringBuilder.Append("状态:").Append(BattleUtils.GetEnumMarkDesc<State>(StateMark));
            mStringBuilder.AppendLine();

            var allEffectList = Effect.AllEffectList;
            if (IsState(State.Shield) || mShieldValue > 0 || mSpecialShieldValue>0)
            {
                mStringBuilder.Append("护盾：").Append(mShieldValue);
                mStringBuilder.AppendLine();
                mStringBuilder.Append("特殊护盾：").Append(mSpecialShieldValue);
                mStringBuilder.AppendLine();
            }

            mStringBuilder.Append("效果:");
            if (Effect.AllEffectList.Count > 0)
            {
                mStringBuilder.AppendLine();
                int index = 0;
                foreach (var value in allEffectList)
                {
                    index++;
                    mStringBuilder.Append(index).Append(":").Append(value.GetType().Name).Append("-").Append(value.sid)
                        .Append(",施法者:")
                        .Append(Battle.GetSceneObject<Fighter>(value.authorUid).GetBaseDesc()).Append(",剩余(")
                        .Append(value.GetActiveBout()).Append("/").Append(value.bout).Append(")回合").Append(",叠加(")
                        .Append(value.overlayCount).Append("/").Append(value.addMax).Append(")层");
                    mStringBuilder.AppendLine();
                }
            }
            else
            {
                mStringBuilder.Append("无");
                mStringBuilder.AppendLine();
            }

            return mStringBuilder.ToString();
        }

        public override string GetBaseDesc()
        {
            return $"{(CampType == BattleDef.TeamCampType.Attacker ? "进攻方" : "防守方")}({Data.Pos})号位[{Data.Cfg?.name}]";
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}