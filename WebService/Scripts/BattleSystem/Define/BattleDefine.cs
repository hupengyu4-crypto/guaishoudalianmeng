using RootScript.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BattleSystem
{
    public static class BattleDef
    {
        /// <summary>
        /// 百分比属性系数
        /// </summary>
        public const double Percent = 0.01d;
        /// <summary>
        /// 万分比属性系数
        /// </summary>
        public const double Percent100 = 0.0001d;
        /// <summary>
        /// 一万倍
        /// </summary>
        public const double TenThousand = 10000d;
        /// <summary>
        /// 帧数
        /// </summary>
        public static int FrameRate = 30;
        /// <summary>
        /// 单帧耗时
        /// </summary>
        public static double FrameDelta = 1d / FrameRate;

        /// <summary>
        /// 破甲上限
        /// </summary>
        public const double AntiDefMax = 9000d;

        /// <summary>
        /// 暴击伤害最小值
        /// </summary>
        public const double CriticalHurtMin = 2.5d;

        /// <summary>
        /// 暴击伤害最大值
        /// </summary>
        public const double CriticalHurtMax = 4d;

        /// <summary>
        /// pvp队伍总速度基础值
        /// </summary>
        public const long PVPBaseTotalSpeed = 3000;

        /// <summary>
        /// 属性是否放大一万倍
        /// </summary>
        public static Dictionary<Property, bool> IsPropertyCoefficient
        {
            get
            {
                if (mPropertyCoefficient == null)
                {
                    mPropertyCoefficient = new Dictionary<Property, bool>((int)Property.MaxCount);
                    foreach (var value in Enum.GetValues(typeof(Property)))
                    {
                        Property property = (Property)value;
                        mPropertyCoefficient[property] = true;
                    }
                    //值类型
                    mPropertyCoefficient[Property.max_hp] = false;
                    mPropertyCoefficient[Property.hp] = false;
                    mPropertyCoefficient[Property.anger] = false;
                    mPropertyCoefficient[Property.attack] = false;
                    mPropertyCoefficient[Property.defense] = false;
                    mPropertyCoefficient[Property.speed] = false;
                    mPropertyCoefficient[Property.lost_hp_total] = false;
                    mPropertyCoefficient[Property.losthp] = false;
                }
                return mPropertyCoefficient;
            }
        }

        private static Dictionary<Property, bool> mPropertyCoefficient;

        /// <summary>
        /// 公用配置 ID是定死的只能修改value值
        /// </summary>
        public enum DefineCfg
        {
            /// <summary>
            /// 是否输出日志
            /// </summary>
            DebugLog = 2,
            /// <summary>
            /// 普攻加怒气
            /// </summary>
            AttackAddRage = 3,
            /// <summary>
            /// 受击加怒气
            /// </summary>
            BeHitAddRage = 4,
            /// <summary>
            /// 护盾上限为当前最大生命倍数
            /// </summary>
            MaxShieldPer = 5,
            /// <summary>
            /// 火球伤害系数
            /// </summary>
            Fire = 6,
            /// <summary>
            /// 属性上限配置
            /// </summary>
            MaxProps = 7,
            /// <summary>
            /// 战斗场景与类型映射
            /// </summary>
            BattleScene = 8,
            /// <summary>
            /// 大火球额外伤害系数
            /// </summary>
            BigFire = 9,
            /// <summary>
            /// PVE宠物额外增伤倍率
            /// </summary>
            PvePetExtraDamageAddFactor = 10,
            /// <summary>
            /// PVP宠物额外增伤倍率
            /// </summary>
            PvpPetExtraDamageAddFactor = 11,
            /// <summary>
            /// 孤立叠加的最大层数
            /// </summary>
            IsolateMaxOverlayCount = 12,
            /// <summary>
            /// 神威额外普攻上限
            /// </summary>
            ShenWeiNormalSkillLimit = 13,
            /// <summary>
            /// 所有速度之和的次方
            /// </summary>
            TotalSpeedPower = 14,
            /// <summary>
            /// 中毒、灼烧、流血伤害倍率上下限
            /// </summary>
            DebuffDMGLimit = 15,
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        public static bool DebugLog
        {
            get
            {
#if COMBAT_CHECK
                return true;
#else
                if (mDebugLog == -1)
                {
                    mDebugLog = ConfigManagerNew.Instance.Get<BattleDefineCfg>(ConfigHashCodeDefine.BattleDefineCfg, (int)DefineCfg.DebugLog).valueIntArray[0][0];
                }
                return mDebugLog == 1;
#endif

            }
            set => mDebugLog = value ? 1 : -1;
        }
        private static int mDebugLog = -1;

        /// <summary>
        /// 普攻加怒气
        /// </summary>
        public static int AttackAddRage
        {
            get
            {
                if (mAttackAddRage == 0)
                {
                    mAttackAddRage = ConfigManagerNew.Instance.Get<BattleDefineCfg>(ConfigHashCodeDefine.BattleDefineCfg, (int)DefineCfg.AttackAddRage).valueIntArray[0][0];
                }
                return mAttackAddRage;
            }
        }
        private static int mAttackAddRage;

        /// <summary>
        /// 受击加怒气
        /// </summary>
        public static int BeHitAddRage
        {
            get
            {
                if (mBeHitAddRage == 0)
                {
                    mBeHitAddRage = ConfigManagerNew.Instance.Get<BattleDefineCfg>(ConfigHashCodeDefine.BattleDefineCfg, (int)DefineCfg.BeHitAddRage).valueIntArray[0][0];
                }
                return mBeHitAddRage;
            }
        }
        private static int mBeHitAddRage;

        /// <summary>
        /// 护盾上限为当前最大生命倍数
        /// </summary>
        public static double MaxShieldPerPer
        {
            get
            {
                if (mMaxShieldPer == 0)
                {
                    mMaxShieldPer = ConfigManagerNew.Instance.Get<BattleDefineCfg>(ConfigHashCodeDefine.BattleDefineCfg, (int)DefineCfg.MaxShieldPer).valueDoubleArray[0][0];
                }
                return mMaxShieldPer;
            }
        }
        private static double mMaxShieldPer;

        /// <summary>
        /// 火球伤害系数
        /// </summary>
        public static double[][] Fire
        {
            get
            {
                if (mFire == null)
                {
                    mFire = ConfigManagerNew.Instance.Get<BattleDefineCfg>(ConfigHashCodeDefine.BattleDefineCfg, (int)DefineCfg.Fire).valueDoubleArray;
                }
                return mFire;
            }
        }
        private static double[][] mFire;

        /// <summary>
        /// 属性上限配置
        /// </summary>
        public static Dictionary<byte, double[]> MaxProps
        {
            get
            {
                if (mMaxProps == null)
                {
                    mMaxProps = new Dictionary<byte, double[]>();
                    var cfg = ConfigManagerNew.Instance.Get<BattleDefineCfg>(ConfigHashCodeDefine.BattleDefineCfg, (int)DefineCfg.MaxProps).valueDoubleArray;
                    foreach (var item in cfg)
                    {
                        mMaxProps.Add((byte)item[0], new double[] { item[1], item[2] });
                    }
                }
                return mMaxProps;
            }
        }
        private static Dictionary<byte, double[]> mMaxProps;

        /// <summary>
        /// 战斗类型映射战斗场景
        /// </summary>
        public static Dictionary<string, long> BattleSceneInfo
        {
            get
            {
                if (mBattleSceneInfo == null)
                {
                    mBattleSceneInfo = new Dictionary<string, long>();
                    var cfg = ConfigManagerNew.Instance.Get<BattleDefineCfg>(ConfigHashCodeDefine.BattleDefineCfg, (int)DefineCfg.BattleScene).valueStringArray;
                    foreach (var item in cfg)
                    {
                        mBattleSceneInfo.Add(item[0], item[1].ToLong());
                    }
                }
                return mBattleSceneInfo;
            }
        }
        private static Dictionary<string, long> mBattleSceneInfo;


        /// <summary>
        /// 大火球额外伤害系数
        /// </summary>
        public static double BigFire
        {
            get
            {
                if (mBigFire == 0)
                {
                    mBigFire = ConfigManagerNew.Instance.Get<BattleDefineCfg>(ConfigHashCodeDefine.BattleDefineCfg, (int)DefineCfg.BigFire).valueDoubleArray[0][0];
                }
                return mBigFire;
            }
        }
        private static double mBigFire;

        private static double mPvePetExtraDamageAddFactor;
        /// <summary>
        /// PVE额外增伤倍率
        /// </summary>
        public static double PvePetExtraDamageAddFactor
        {
            get
            {
                if (mPvePetExtraDamageAddFactor == 0)
                {
                    mPvePetExtraDamageAddFactor = ConfigManagerNew.Instance.Get<BattleDefineCfg>(ConfigHashCodeDefine.BattleDefineCfg, (int)DefineCfg.PvePetExtraDamageAddFactor).valueDoubleArray[0][0];
                }
                return mPvePetExtraDamageAddFactor;
            }
        }

        private static double mPvpPetExtraDamageAddFactor;
        /// <summary>
        /// PVp额外增伤倍率
        /// </summary>
        public static double PvpPetExtraDamageAddFactor
        {
            get
            {
                if (mPvpPetExtraDamageAddFactor == 0)
                {
                    mPvpPetExtraDamageAddFactor = ConfigManagerNew.Instance.Get<BattleDefineCfg>(ConfigHashCodeDefine.BattleDefineCfg, (int)DefineCfg.PvpPetExtraDamageAddFactor).valueDoubleArray[0][0];
                }
                return mPvpPetExtraDamageAddFactor;
            }
        }

        private static int mIsolateMaxOverlayCount;
        /// <summary>
        /// 孤立叠加的最大层数
        /// </summary>
        public static int IsolateMaxOverlayCount
        {
            get
            {
                if (mIsolateMaxOverlayCount == 0)
                {
                    mIsolateMaxOverlayCount = ConfigManagerNew.Instance.Get<BattleDefineCfg>(ConfigHashCodeDefine.BattleDefineCfg, (int)DefineCfg.IsolateMaxOverlayCount).valueIntArray[0][0];
                }
                return mIsolateMaxOverlayCount;
            }
        }

        private static int mShenWeiNormalSkillLimit;
        /// <summary>
        /// 神威额外普攻上限
        /// </summary>
        public static int ShenWeiNormalSkillLimit
        {
            get
            {
                if (mShenWeiNormalSkillLimit == 0)
                {
                    mShenWeiNormalSkillLimit = ConfigManagerNew.Instance.Get<BattleDefineCfg>(ConfigHashCodeDefine.BattleDefineCfg, (int)DefineCfg.ShenWeiNormalSkillLimit).valueIntArray[0][0];
                }
                return mShenWeiNormalSkillLimit;
            }
        }

        private static double mTotalSpeedPower;
        /// <summary>
        /// 所有速度之和的次方
        /// </summary>
        public static double TotalSpeedPower
        {
            get
            {
                if (mTotalSpeedPower == 0.0f)
                {
                    var cfg = ConfigManagerNew.Instance.Get<BattleDefineCfg>(ConfigHashCodeDefine.BattleDefineCfg, (int)DefineCfg.TotalSpeedPower);
                    mTotalSpeedPower = cfg == null ? 1.0d : cfg.valueDoubleArray[0][0];
                }
                return mTotalSpeedPower;
            }
        }


        private static int m_minDebuffDMGMultiple;
        /// <summary>
        /// debuff最小伤害倍率
        /// </summary>
        public static int MinDebuffDMGMultiple
        {
            get
            {
                if (m_minDebuffDMGMultiple == 0)
                {
                    m_minDebuffDMGMultiple = ConfigManagerNew.Instance.Get<BattleDefineCfg>(ConfigHashCodeDefine.BattleDefineCfg, (int)DefineCfg.DebuffDMGLimit).valueIntArray[1][0];
                }
                return m_minDebuffDMGMultiple;
            }
        }


        private static int m_MaxDebuffDMGMultiple;
        /// <summary>
        /// debuff最大伤害倍率 
        /// </summary>
        public static int MaxDebuffDMGMultiple
        {
            get
            {
                if (m_MaxDebuffDMGMultiple == 0)
                {
                    m_MaxDebuffDMGMultiple = ConfigManagerNew.Instance.Get<BattleDefineCfg>(ConfigHashCodeDefine.BattleDefineCfg, (int)DefineCfg.DebuffDMGLimit).valueIntArray[0][0];
                }
                return m_MaxDebuffDMGMultiple;
            }
        }

        /// <summary>
        /// 战斗类型
        /// </summary>
        public enum BattleType : byte
        {
            /// <summary>
            /// 缺省
            /// </summary>
            Default = 0,
            /// <summary>
            /// 普通
            /// </summary>
            Normal = 1,
            /// <summary>
            /// 集结
            /// </summary>
            Mass = 2,
            /// <summary>
            /// Boss
            /// </summary>
            Boss = 3,
            /// <summary>
            /// 火把战
            /// </summary>
            Fire = 4,
            /// <summary>
            /// PVP
            /// </summary>
            PVP = 5,
            /// <summary>
            /// KOF
            /// </summary>
            KOF = 6,
            /// <summary>
            /// Bo3
            /// </summary>
            Bo3 = 7,
        }

        /// <summary>
        /// 战斗结果 所有战斗类型相对于攻击方
        /// </summary>
        public enum BattleResult : byte
        {
            /// <summary>
            /// 平局
            /// </summary>
            Tie = 0,
            /// <summary>
            /// 胜利
            /// </summary>
            Win = 1,
            /// <summary>
            /// 失败
            /// </summary>
            Fail = 2,
        }

        /// <summary>
        /// 队伍阵营类型
        /// </summary>
        public enum TeamCampType : byte
        {
            /// <summary>
            /// 进攻方
            /// </summary>
            Attacker = 0,
            /// <summary>
            /// 防守方
            /// </summary>
            Defender = 1,
        }

        /// <summary>
        /// 队伍拥有者类型
        /// </summary>
        public enum TeamOwnerType : byte
        {
            Player,
            Npc
        }

        /// <summary>
        /// 取值目标类型
        /// </summary>
        public enum TargetType : byte
        {
            /// <summary>
            /// 自己
            /// </summary>
            [Description("自己")]
            Self,
            /// <summary>
            /// 目标
            /// </summary>
            [Description("目标")]
            Target
        }

        /// <summary>
        /// 伤害类型
        /// </summary>
        public enum DamageType : byte
        {
            /// <summary>
            /// 普攻
            /// </summary>
            [Description("普攻")]
            Normal,
            /// <summary>
            /// 技能
            /// </summary>
            [Description("技能")]
            Skill,
            /// <summary>
            /// 持续伤害
            /// </summary>
            [Description("持续伤害")]
            DeBuff,
            /// <summary>
            /// 持续伤害(毒)
            /// </summary>
            [Description("持续伤害(毒)")]
            PoisonBuff,
            /// <summary>
            /// 持续伤害(灼烧)
            /// </summary>
            [Description("持续伤害(灼烧)")]
            FiringBuff,
            /// <summary>
            /// 持续伤害(流血)
            /// </summary>
            [Description("持续伤害(流血)")]
            BleedBuff,
            /// <summary>
            /// 间接伤害
            /// </summary>
            [Description("间接伤害")]
            Indirect,
            /// <summary>
            /// 指令伤害
            /// </summary>
            [Description("指令伤害")]
            Cmd,
            /// <summary>
            /// 小兵伤害
            /// </summary>
            [Description("小兵伤害")]
            Creep,
        }

        /// <summary>
        /// 战斗者属性，同业务端的 PropertyDefine.cs中的 PropertyType
        /// </summary>
        public enum Property
        {
            /// <summary>
            /// 缺省
            /// </summary>
            None = 0,

            /// <summary>
            /// 最大生命
            /// </summary>
            [Description("最大生命")]
            max_hp,

            /// <summary>
            /// 怒气
            /// </summary>
            [Description("怒气")]
            anger,

            /// <summary>
            /// 治疗强度
            /// </summary>
            [Description("治疗强度")]
            heal,

            /// <summary>
            ///  攻击
            /// </summary>
            [Description("攻击")]
            attack,

            /// <summary>
            /// 防御
            /// </summary>
            [Description("防御")]
            defense,

            /// <summary>
            /// 血量
            /// </summary>
            [Description("血量")]
            hp,


            /// <summary>
            /// 速度
            /// </summary>
            [Description("速度")]
            speed,

            /// <summary>
            /// 增伤
            /// </summary>
            [Description("增伤")]
            damage_add,

            /// <summary>
            /// 减伤
            /// </summary>
            [Description("减伤")]
            damage_reduce,

            /// <summary>
            /// VIP增伤
            /// </summary>
            [Description("VIP增伤")]
            vip_damage_add,

            /// <summary>
            /// VIP减伤
            /// </summary>
            [Description("VIP减伤")]
            vip_damage_reduce,

            /// <summary>
            /// 技能伤害
            /// </summary>
            [Description("技能伤害")]
            ability_dmg_add,

            /// <summary>
            /// 技能减伤
            /// </summary>
            [Description("技能减伤")]
            ability_dmg_reduce,

            /// <summary>
            /// 破甲
            /// </summary>
            [Description("破甲")]
            broken_add,

            /// <summary>
            /// 破甲抵抗
            /// </summary>
            [Description("破甲抵抗")]
            broken_reduce,

            /// <summary>
            /// 暴击
            /// </summary>
            [Description("暴击")]
            crit,

            /// <summary>
            /// 暴击抵抗
            /// </summary>
            [Description("暴击抵抗")]
            crit_reduce,

            /// <summary>
            /// 暴伤
            /// </summary>
            [Description("暴伤")]
            crit_hurt_add,

            /// <summary>
            /// 暴伤抵抗
            /// </summary>
            [Description("暴伤抵抗")]
            crit_hurt_reduce,

            /// <summary>
            /// 控制命中
            /// </summary>
            [Description("控制命中")]
            control_add,

            /// <summary>
            /// 免控
            /// </summary>
            [Description("免控")]
            control_cut,

            /// <summary>
            /// 抵抗战士职业伤害
            /// </summary>
            [Description("抵抗战士职业伤害")]
            soldier_hurt_cut,

            /// <summary>
            /// 抵抗法师职业伤害
            /// </summary>
            [Description("抵抗法师职业伤害")]
            master_hurt_cut,

            /// <summary>
            /// 抵抗刺客职业伤害
            /// </summary>
            [Description("抵抗刺客职业伤害")]
            assassin_hurt_cut,

            /// <summary>
            /// 抵抗辅助职业伤害
            /// </summary>
            [Description("抵抗辅助职业伤害")]
            support_hurt_cut,

            /// <summary>
            /// 抵抗肉盾职业伤害
            /// </summary>
            [Description("抵抗肉盾职业伤害")]
            tank_hurt_cut,

            /// <summary>
            /// 抵抗控制职业伤害
            /// </summary>
            [Description("抵抗控制职业伤害")]
            control_hurt_cut,

            /// <summary>
            /// 冰冻抵抗
            /// </summary>
            [Description("冰冻抵抗")]
            freeze_cut,

            /// <summary>
            /// 中毒抵抗
            /// </summary>
            [Description("中毒抵抗")]
            poison_cut,

            /// <summary>
            ///麻痹抵抗
            /// </summary>
            [Description("麻痹抵抗")]
            palsy_cut,

            /// <summary>
            /// 灼烧抵抗
            /// </summary>
            [Description("灼烧抵抗")]
            burn_cut,

            /// <summary>
            /// 沉默抵抗
            /// </summary>
            [Description("沉默抵抗")]
            silent_cut,

            /// <summary>
            /// 流血抵抗
            /// </summary>
            [Description("流血抵抗")]
            bleed_cut,

            /// <summary>
            /// 眩晕抵抗
            /// </summary>
            [Description("眩晕抵抗")]
            vertigo_cut,

            /// <summary>
            /// 减疗抵抗
            /// </summary>
            [Description("减疗抵抗")]
            reduce_healing_cut,

            /// <summary>
            /// 格挡
            /// </summary>
            [Description("格挡")]
            block,

            /// <summary>
            /// 精准
            /// </summary>
            [Description("精准")]
            accurate,

            /// <summary>
            /// 死血
            /// </summary>
            [Description("死血")]
            dead_hp,

            /// <summary>
            /// 战功压制
            /// </summary>
            [Description("战功压制")]
            service_pressing,

            /// <summary>
            /// 华夏伤害加成
            /// </summary>
            [Description("华夏伤害加成")]
            chinese__dmg_add,

            /// <summary>
            /// 华夏伤害抵抗
            /// </summary>
            [Description("华夏伤害抵抗")]
            chinese__dmg_reduce,

            /// <summary>
            /// 北欧伤害加成
            /// </summary>
            [Description("北欧伤害加成")]
            nordic__dmg_add,

            /// <summary>
            /// 北欧伤害抵抗
            /// </summary>
            [Description("北欧伤害抵抗")]
            nordic__dmg_reduce,

            /// <summary>
            /// 希腊伤害加成
            /// </summary>
            [Description("希腊伤害加成")]
            greece__dmg_add,

            /// <summary>
            /// 希腊伤害抵抗
            /// </summary>
            [Description("希腊伤害抵抗")]
            greece__dmg_reduce,

            /// <summary>
            /// 群雄伤害增加
            /// </summary>
            [Description("群雄伤害增加")]
            qunxiong_dmg_add,

            /// <summary>
            /// 群雄伤害抵抗
            /// </summary>
            [Description("群雄伤害抵抗")]
            qunxiong_dmg_reduce,

            /// <summary>
            /// 华夏攻击
            /// </summary>
            [Description("华夏攻击")]
            chinese_attack,


            /// <summary>
            /// 华夏血量
            /// </summary>
            [Description("华夏血量")]
            chinese_hp,

            /// <summary>
            /// 北欧攻击
            /// </summary>
            [Description("北欧攻击")]
            nordic_attack,
            /// <summary>
            /// 北欧血量
            /// </summary>
            [Description("北欧血量")]
            nordic_hp,
            /// <summary>
            /// 北欧血量
            /// </summary>
            [Description("希腊攻击")]
            greece_attack,

            /// <summary>
            /// 希腊血量
            /// </summary>
            [Description("希腊血量")]
            greece_hp,

            /// <summary>
            /// 希腊血量
            /// </summary>
            [Description("所有采集")]
            resource_speed,

            /// <summary>
            /// 增加对战士职业伤害
            /// </summary>
            [Description("增加对战士职业伤害")]
            soldier_hurt_add,

            /// <summary>
            /// 增加对法师职业伤害
            /// </summary>
            [Description("增加对法师职业伤害")]
            master_hurt_add,

            /// <summary>
            /// 增加对刺客职业伤害
            /// </summary>
            [Description("增加对刺客职业伤害")]
            assassin_hurt_add,

            /// <summary>
            /// 增加对辅助职业伤害
            /// </summary>
            [Description("增加对辅助职业伤害")]
            support_hurt_add,

            /// <summary>
            /// 增加对肉盾职业伤害
            /// </summary>
            [Description("增加对肉盾职业伤害")]
            tank_hurt_add,

            /// <summary>
            /// 增加对控制职业伤害
            /// </summary>
            [Description("增加对控制职业伤害")]
            control_hurt_add,

            /// <summary>
            /// 受治疗效果，数值0的时候表示100%
            /// </summary>
            [Description("受治疗效果")]
            be_heal,

            /// <summary>
            /// 吸血
            /// </summary>
            [Description("吸血")]
            life_steal,

            /// <summary>
            /// 第四种族伤害加成
            /// </summary>
            [Description("群伤害加成")]
            fourth__dmg_add,

            /// <summary>
            /// 第四种族伤害抵抗
            /// </summary>
            [Description("群伤害抵抗")]
            fourth__dmg_reduce,

            /// <summary>
            /// 对第一种族伤害加成
            /// </summary>
            [Description("对魏伤害加成")]
            to_first_dmg_add,

            /// <summary>
            /// 对第二种族伤害加成
            /// </summary>
            [Description("对蜀伤害加成")]
            to_second_dmg_add,

            /// <summary>
            /// 对第三种族伤害加成
            /// </summary>
            [Description("对吴伤害加成")]
            to_third_dmg_add,

            /// <summary>
            /// 对第四种族伤害加成
            /// </summary>
            [Description("对群伤害加成")]
            to_fourth_dmg_add,

            /// <summary>
            /// 添加护盾比例
            /// </summary>
            [Description("添加护盾比例")]
            shield_add_factor,

            /// <summary>
            /// 受到第一种族伤害加成
            /// </summary>
            [Description("受到魏伤害加成")]
            be_first_hit_damage_add,

            /// <summary>
            /// 受到第二种族伤害加成
            /// </summary>
            [Description("受到蜀伤害加成")]
            be_second_hit_damage_add,

            /// <summary>
            /// 受到第三种族伤害加成
            /// </summary>
            [Description("受到吴伤害加成")]
            be_third_hit_damage_add,

            /// <summary>
            /// 受到第四种族伤害加成
            /// </summary>
            [Description("受到群伤害加成")]
            be_forth_hit_damage_add,

            /// <summary>
            /// 受到伤害加成
            /// </summary>
            [Description("受到伤害加成")]
            be_hit_damage_add,

            /// <summary>
            /// 普攻伤害提升
            /// </summary>
            [Description("普攻伤害提升")]
            normal_damage_add,

            /// <summary>
            /// 普攻伤害减免
            /// </summary>
            [Description("普攻伤害减免")]
            normal_damage_reduce,

            /// <summary>
            /// 灼烧命中
            /// </summary>
            [Description("灼烧命中")]
            burn_hit_add,

            /// <summary>
            /// 中毒命中
            /// </summary>
            [Description("中毒命中")]
            poison_hit_add,

            /// <summary>
            /// 流血命中
            /// </summary>
            [Description("流血命中")]
            bleed_hit_add,

            /// <summary>
            /// 受到普攻伤害加成
            /// </summary>
            [Description("受到普攻伤害加成")]
            be_hit_normal_damage_add,

            /// <summary>
            /// 星级属性
            /// </summary>
            [Description("星级属性")]
            role_equip_star,

            /// <summary>
            /// 星级减伤
            /// </summary>
            [Description("星级减伤")]
            anti_role_equip_star,

            /// <summary>
            /// 冰冻命中
            /// </summary>
            [Description("冰冻命中")]
            frozen_destined,
            /// <summary>
            /// 眩晕命中
            /// </summary>
            [Description("眩晕命中")]
            dizziness_destined,
            /// <summary>
            /// 沉默命中
            /// </summary>
            [Description("沉默命中")]
            silence_destined,

            /// <summary>
            /// 最终受到伤害减免
            /// </summary>
            [Description("最终受到伤害减免")]
            final_damage_reduce,

            /// <summary>
            /// 最终造成伤害减免
            /// </summary>
            [Description("最终造成伤害减免")]
            final_attack_damage_reduce,

            /// <summary>
            /// 累计损失血量
            /// </summary>
            [Description("累计损失血量")]
            lost_hp_total,

            /// <summary>
            /// 损失血量
            /// </summary>
            [Description("损失血量")]
            losthp,

            /// <summary>
            /// debuff减伤
            /// </summary>
            [Description("debuff减伤")]
            debuff_damage_reduce,

            /// <summary>
            /// 流血减伤
            /// </summary>
            [Description("流血减伤")]
            bleed_damage_reduce,

            /// <summary>
            /// 毒减伤
            /// </summary>
            [Description("毒减伤")]
            poison_damage_reduce,

            /// <summary>
            /// 灼烧减伤
            /// </summary>
            [Description("灼烧减伤")]
            burn_damage_reduce,

            /// <summary>
            /// debuff增伤
            /// </summary>
            [Description("debuff增伤")]
            debuff_damage_add,

            /// <summary>
            /// 流血增伤
            /// </summary>
            [Description("流血增伤")]
            bleed_damage_add,

            /// <summary>
            /// 毒增伤
            /// </summary>
            [Description("毒增伤")]
            poison_damage_add,

            /// <summary>
            /// 灼烧增伤
            /// </summary>
            [Description("灼烧增伤")]
            burn_damage_add,

            /// <summary>
            /// 有效属性总数，添加移除都要修改
            /// </summary>
            MaxCount,
        }

        /// <summary>
        /// 查找范围类型
        /// </summary>
        public enum EFindType : byte
        {
            /// <summary>
            /// 当前对位目标
            /// </summary>
            [Description("当前对位目标")]
            Target,
            /// <summary>
            /// 自身
            /// </summary>
            [Description("自身")]
            Self,
            /// <summary>
            /// 我方出战者
            /// </summary>
            [Description("我方出战者")]
            TargetSelf,
            /// <summary>
            /// 我方全体
            /// </summary>
            [Description("我方全体")]
            AllSelf,
            /// <summary>
            /// 我方全体,排除自己
            /// </summary>
            [Description("我方全体,排除自己")]
            AllSelfExMe,
            /// <summary>
            /// 随机我方
            /// </summary>
            [Description("随机我方")]
            RandomSelf,
            /// <summary>
            /// 我方前排
            /// </summary>
            [Description("我方前排")]
            FrontRowSelf,
            /// <summary>
            /// 我方后排
            /// </summary>
            [Description("我方后排")]
            BackRowSelf,
            /// <summary>
            /// 敌方全体
            /// </summary>
            [Description("敌方全体")]
            AllEnemy,
            /// <summary>
            /// 随机敌方
            /// </summary>
            [Description("随机敌方")]
            RandomEnemy,
            /// <summary>
            /// 敌方前排
            /// </summary>
            [Description("敌方前排")]
            FrontRowEnemy,
            /// <summary>
            /// 敌方后排
            /// </summary>
            [Description("敌方后排")]
            BackRowEnemy,
            /// <summary>
            /// 我方同排
            /// </summary>
            [Description("我方同排")]
            SameRowSelf,
            /// <summary>
            /// 我方异排
            /// </summary>
            [Description("我方异排")]
            OtherRowSelf,
            /// <summary>
            /// 敌方前排
            /// </summary>
            [Description("敌方同排")]
            SameRowEnemy,
            /// <summary>
            /// 敌方后排
            /// </summary>
            [Description("敌方异排")]
            OtherRowEnemy,
        }

        /// <summary>
        /// 种族
        /// </summary>
        public enum ERaceType
        {
            /// <summary>
            /// 全体
            /// </summary>
            [Description("全体")]
            All = 0,
            /// <summary>
            /// 希腊
            /// </summary>
            [Description("希腊")]
            XiLa = 1,
            /// <summary>
            /// 北欧
            /// </summary>
            [Description("北欧")]
            BeiOu = 2,
            /// <summary>
            /// 华夏
            /// </summary>
            [Description("华夏")]
            HuaXia = 3,
            /// <summary>
            /// 魏
            /// </summary>
            [Description("魏")]
            Wei = 1,
            /// <summary>
            /// 蜀
            /// </summary>
            [Description("蜀")]
            Shu = 2,
            /// <summary>
            /// 吴
            /// </summary>
            [Description("吴")]
            Wu = 3,
            /// <summary>
            /// 群
            /// </summary>
            [Description("群")]
            Qun = 4,
            /// <summary>
            /// 罗马
            /// </summary>
            [Description("罗马")]
            LuoMa = 5,
            /// <summary>
            /// 中亚
            /// </summary>
            [Description("中亚")]
            ZhongYa = 6,
            /// <summary>
            /// 欧洲
            /// </summary>
            [Description("欧洲")]
            OuZhou = 7,
        }

        /// <summary>
        /// 卡牌职业
        /// </summary>
        public enum EJobType
        {
            /// <summary>
            /// 全体
            /// </summary>
            [Description("全体")]
            All = 0,
            /// <summary>
            /// 战士
            /// </summary>
            [Description("战士")]
            Soldier = 1,
            /// <summary>
            /// 法师
            /// </summary>
            [Description("法师")]
            Sorcerer = 2,
            /// <summary>
            /// 刺客
            /// </summary>
            [Description("刺客")]
            Assassin = 3,
            /// <summary>
            /// 射手
            /// </summary>
            [Description("射手")]
            Controller = 4,
            /// <summary>
            /// 肉盾
            /// </summary>
            [Description("肉盾")]
            Meatshield = 5,
            /// <summary>
            /// 辅助
            /// </summary>
            [Description("辅助")]
            Supporter = 6,
            /// <summary>
            /// 大地图Boss
            /// </summary>
            [Description("大地图Boss")]
            MapBoss = 7,
            /// <summary>
            /// 怪物
            /// </summary>
            [Description("怪物")]
            Monster = 8,
            /// <summary>
            /// 宠物 无法锁定 不参与死亡计算
            /// </summary>
            [Description("宠物 无法锁定 不参与死亡计算")]
            Pet = 9,
        }

        /// <summary>
        /// 技能类型
        /// </summary>
        public enum ESkillType : byte
        {
            [Description("空类型")]
            None,
            /// <summary>
            /// 普攻技
            /// </summary>
            [Description("普攻技")]
            Normal = 1,
            /// <summary>
            /// 怒气技
            /// </summary>
            [Description("怒气技")]
            Rage = 2,
            /// <summary>
            /// 开场技
            /// </summary>
            [Description("开场技")]
            Opening = 3,
            /// <summary>
            /// 被动技(不主动释放的为被动技)
            /// </summary>
            [Description("被动技(不主动释放都为被动技)")]
            Passive = 4,
        }

        /// <summary>
        /// 检查类型
        /// </summary>
        public enum CheckType : byte
        {
            /// <summary>
            /// 空
            /// </summary>
            [Description("空")]
            Null,
            /// <summary>
            /// 检查属性，值1:目标
            /// </summary>
            [Description("检查属性，值1:目标")]
            Property,
            [Description("目标异常状态种类数量增加伤害基础百分比")]
            DebuffTypeNumAddFactors,
            [Description("目标异常状态数量增加伤害基础百分比")]
            DebuffNumAddFactors,
            [Description("随机添加一个效果")]
            RandomOneEffect,
            [Description("指定大回合")]
            DesignatedBout,
            [Description("检测状态层数")]
            StateOverlayCount,
        }

        /// <summary>
        /// 比较类型
        /// </summary>
        public enum CompareType : byte
        {
            /// <summary>
            /// 小于
            /// </summary>
            [Description("小于")]
            Small,
            /// <summary>
            /// 等于
            /// </summary>
            [Description("等于")]
            Equal,
            /// <summary>
            /// 大于
            /// </summary>
            [Description("大于")]
            Big,
            /// <summary>
            /// 小于自己
            /// </summary>
            [Description("小于自己")]
            LessThanMe,
            /// <summary>
            /// 等于自己
            /// </summary>
            [Description("等于自己")]
            EqualToMe,
            /// <summary>
            /// 大于自己
            /// </summary>
            [Description("大于自己")]
            GreaterThanMe,
            /// <summary>
            /// 小于等于
            /// </summary>
            [Description("小于等于")]
            SmallAndEqual,
            /// <summary>
            /// 大于等于
            /// </summary>
            [Description("大于等于")]
            BigAndEqual,
        }

        /// <summary>
        /// 简单比较类型
        /// </summary>
        public enum SimpleCompareType : byte
        {
            /// <summary>
            /// 等于
            /// </summary>
            [Description("等于")]
            Equal,
            /// <summary>
            /// 小于
            /// </summary>
            [Description("小于")]
            Small,
            /// <summary>
            /// 大于
            /// </summary>
            [Description("大于")]
            Big,
            /// <summary>
            /// 小于等于
            /// </summary>
            [Description("小于等于")]
            SmallEqual,
            /// <summary>
            /// 大于等于
            /// </summary>
            [Description("大于等于")]
            BigEqual,
        }

        public enum CompareGradeType : byte
        {
            /// <summary>
            /// 最高
            /// </summary>
            [Description("最高")]
            Highest,
            /// <summary>
            /// 最低
            /// </summary>
            [Description("最低")]
            Lowest,
        }

        public enum OtherType : byte
        {
            /// <summary>
            /// 空
            /// </summary>
            [Description("空")]
            Null,
            /// <summary>
            /// 有负面效果（包括控制，减益，中毒）
            /// </summary>
            [Description("有负面效果")]
            NegativeEffect,
            /// <summary>
            /// 血量百分比
            /// </summary>
            [Description("血量百分比")]
            HpPercent,
            /// <summary>
            /// 状态层数
            /// </summary>
            [Description("状态层数")]
            StateOverlayCount,
        }
        public static Dictionary<Type, int> cmdType = new Dictionary<Type, int>()
        {
            {typeof(MakeDamageCmd),1},
            {typeof(FireKillCommand),2},
            {typeof(AddFighterCmd),3},
            {typeof(UseSkillCmd),4},
        };
        public static BattleCommandHandler NewBattleCommandHandler(long type, byte[] bytes)
        {
            switch (type)
            {
                case 1:
                    MakeDamageCmd damageCmd = new MakeDamageCmd();
                    if (damageCmd.UnSerialize(bytes))
                    {
                        return damageCmd;
                    }

                    return null;
                case 2:
                    FireKillCommand fireKillCmd = new FireKillCommand();
                    if (fireKillCmd.UnSerialize(bytes))
                    {
                        return fireKillCmd;
                    }

                    return null;
                case 3:
                    AddFighterCmd addFightCmd = new AddFighterCmd();
                    if (addFightCmd.UnSerialize(bytes))
                    {
                        return addFightCmd;
                    }

                    return null;
                case 4:
                    UseSkillCmd useSkillCmd = new UseSkillCmd();
                    if (useSkillCmd.UnSerialize(bytes))
                    {
                        return useSkillCmd;
                    }

                    return null;
            }

            return null;
        }
    }

}
