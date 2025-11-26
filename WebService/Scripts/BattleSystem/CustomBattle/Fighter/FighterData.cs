using RootScript.Config;
using Google.Protobuf.Collections;
using System.Collections.Generic;
using Enum = System.Enum;

namespace BattleSystem
{
    /// <summary>
    /// 战斗者数据
    /// </summary>
    public class FighterData : IObjectPool
    {
        /// <summary>
        /// 持有者
        /// </summary>
        public Fighter Owner { get; private set; }
        /// <summary>
        /// 初始化属性
        /// </summary>
        public long[] initProps = new long[(byte)BattleDef.Property.MaxCount];
        /// <summary>
        /// 当前属性
        /// </summary>
        public long[] nowProps = new long[(byte)BattleDef.Property.MaxCount];
        /// <summary>
        /// 战斗者配置
        /// </summary>
        public BattleFighterCfg Cfg { get; private set; }
        /// <summary>
        /// uid
        /// </summary>
        public long Uid { get; private set; }
        /// <summary>
        /// 模板Sid
        /// </summary>
        public long Sid
        {
            get => mSid;
            set
            {
                if (mSid != value)
                {
                    mSid = value;
                    Cfg = ConfigManagerNew.Instance.Get<BattleFighterCfg>(ConfigHashCodeDefine.BattleFighterCfg, mSid);
                }
            }
        }
        /// <summary>
        /// 站位 1-5
        /// </summary>
        public int Pos { get; private set; }
        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; private set; }
        /// <summary>
        /// 阶级
        /// </summary>
        public int Step { get; private set; }
        /// <summary>
        /// 星级
        /// </summary>
        public int Star { get; private set; }
        /// <summary>
        /// 皮肤
        /// </summary>
        public int Skin { get; private set; }

        //属性快取指向
        public long this[int index] => nowProps[index];

        public long this[BattleDef.Property index] => nowProps[(int)index];

        /// <summary>
        /// 开场技
        /// </summary>
        public List<BattleSkill> openingSkills = new List<BattleSkill>();
        /// <summary>
        /// 被动技
        /// </summary>
        public List<BattleSkill> passiveSkills = new List<BattleSkill>();
        /// <summary>
        /// 普通攻击
        /// </summary>
        public BattleSkill normalSkill;
        /// <summary>
        /// 怒气技
        /// </summary>
        public BattleSkill rageSkill;
        /// <summary>
        /// 种族
        /// </summary>
        public BattleDef.ERaceType raceType;
        /// <summary>
        /// 职业
        /// </summary>
        public BattleDef.EJobType jobType;
        /// <summary>
        /// 神力球Sid
        /// </summary>
        public long weapon_id;

        /// <summary>
        /// 神力球星级
        /// </summary>
        public long weapon_star;

        /// <summary>
        /// 模板Sid
        /// </summary>
        private long mSid;

        public fighter_info mFighterInfo;


        public FighterData(Fighter f)
        {
            Owner = f;
        }

        public void OnAwake()
        {
        }

        public void SetData(fighter_info fighterData)
        {
            mFighterInfo = fighterData;
            Uid = fighterData.Uid;
            Sid = fighterData.Sid;
            Level = (int)fighterData.Level;
            Step = (int)fighterData.Step;
            Star = (int)fighterData.Star;
            Skin = (int)fighterData.Skin;
            Pos = (int)fighterData.Pos;
            weapon_id = (int)fighterData.WeaponId;
            weapon_star = (int)fighterData.WeaponStar;
            raceType = (BattleDef.ERaceType)fighterData.Race;
            jobType = (BattleDef.EJobType)fighterData.Job;
            InitSkills();
            SetInitProp(fighterData.Attr);
        }

        #region 战斗属性

        public void SetInitProp(RepeatedField<attr_item> attr)
        {
            foreach (var item in attr)
            {
                var prop = Enum.Parse(typeof(BattleDef.Property), item.AttrName);
                SetInitProp((BattleDef.Property)prop, item.Value);
            }
        }
        public void SetInitProp(RepeatedField<attr_item> attr,HashSet<BattleDef.Property> ignore)
        {
            foreach (var item in attr)
            {
                var prop = (BattleDef.Property)Enum.Parse(typeof(BattleDef.Property), item.AttrName);
                if (!ignore.Contains(prop))
                {
                    SetInitProp(prop, item.Value);
                }
            }
        }
        public void SetInitProp(BattleDef.Property eFightProperty, long v)
        {
            SetInitProp((int)eFightProperty, v);
        }

        public void SetInitProp(Dictionary<string, long> propDic)
        {
            foreach (var item in propDic)
            {
                var prop = Enum.Parse(typeof(BattleDef.Property), item.Key);
                SetInitProp((int)prop, item.Value);
            }
        }

        public void SetInitProp(int index, long v)
        {
            initProps[index] = v;
            nowProps[index] = v;
        }

        /// <summary>
        /// 重置属性
        /// </summary>
        public void ResetProp()
        {
            for (int i = 0; i < nowProps.Length; i++)
            {
                initProps[i] = 0;
                nowProps[i] = 0;
            }
            SetInitProp(mFighterInfo.Attr);
        }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public long GetProp(BattleDef.Property prop)
        {
            return GetProp((int)prop);
        }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public long GetProp(int index)
        {
            if (index < 0)
                index = 0;
            else if (index >= (int)BattleDef.Property.MaxCount)
                index = (int)BattleDef.Property.MaxCount - 1;
            return nowProps[index];
        }

        #endregion

        #region 技能

        /// <summary>
        /// 替换技能
        /// </summary>
        /// <param name="targetSid">目标技能Sid</param>
        /// <param name="replaceSid">替换技能SSid</param>
        /// <returns></returns>
        public bool ReplaceSkill(long targetSid, long replaceSid)
        {
            if (normalSkill != null && normalSkill.Sid == targetSid)
            {
                normalSkill = new BattleSkill(replaceSid, normalSkill.skillLevel, Owner);
                Owner.DispatchEvent(Owner.Battle, BattleObject.Event.Replace, EventParams<long[]>.Create(Owner.Battle, new long[2] { targetSid, replaceSid }));
                return true;
            }
            if (rageSkill != null && rageSkill.Sid == targetSid)
            {
                rageSkill = new BattleSkill(replaceSid, rageSkill.skillLevel, Owner);
                Owner.DispatchEvent(Owner.Battle, BattleObject.Event.Replace, EventParams<long[]>.Create(Owner.Battle, new long[2] { targetSid, replaceSid }));
                return true;
            }

            //开场技能不可能被替换了，只检查被动技能
            var count = passiveSkills.Count;
            for (int i = 0; i < count; i++)
            {
                var skill = passiveSkills[i];
                if (skill.Sid == targetSid)
                {
                    passiveSkills.RemoveAt(i);
                    passiveSkills.Add(new BattleSkill(replaceSid, skill.skillLevel, Owner));
                    Owner.DispatchEvent(Owner.Battle, BattleObject.Event.Replace, EventParams<long[]>.Create(Owner.Battle, new long[2] { targetSid, replaceSid }));
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 替换技能效果
        /// </summary>
        /// <param name="targetSid">目标技能Sid</param>
        /// <param name="effectSids">替换技能效果Sid数组</param>
        /// <param name="selfEffectSids">替换技能效果Sid数组</param>
        /// <returns></returns>
        public bool ReplaceSkillEffect(long targetSid, long[] effectSids, long[] selfEffectSids, long[] selfEndEffectSids)
        {
            if (normalSkill != null && normalSkill.Sid == targetSid)
            {
                normalSkill.ReplaceEffects(effectSids, selfEffectSids, selfEndEffectSids);
                return true;
            }
            else if (rageSkill != null && rageSkill.Sid == targetSid)
            {
                rageSkill.ReplaceEffects(effectSids, selfEffectSids, selfEndEffectSids);
                return true;
            }
            else
            {
                //开场技能不可能被替换了，只检查被动技能
                var count = passiveSkills.Count;
                for (int i = 0; i < count; i++)
                {
                    var skill = passiveSkills[i];
                    if (skill.Sid == targetSid)
                    {
                        skill.ReplaceEffects(effectSids, selfEffectSids, selfEndEffectSids);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 还原技能默认效果
        /// </summary>
        /// <param name="targetSid">目标技能Sid</param>
        public void ReductionEffects(long targetSid)
        {
            if (normalSkill != null && normalSkill.Sid == targetSid)
            {
                normalSkill.ReductionEffects();
            }
            else if (rageSkill != null && rageSkill.Sid == targetSid)
            {
                rageSkill.ReductionEffects();
            }
            else
            {
                //开场技能不可能被替换了，只检查被动技能
                var count = passiveSkills.Count;
                for (int i = 0; i < count; i++)
                {
                    var skill = passiveSkills[i];
                    if (skill.Sid == targetSid)
                    {
                        skill.ReductionEffects();
                    }
                }
            }
        }

        /// <summary>
        /// 初始化技能
        /// </summary>
        public void InitSkills()
        {
            normalSkill = null;
            rageSkill = null;
            openingSkills.Clear();
            passiveSkills.Clear();
            var sids = mFighterInfo.Skills;
            foreach (var pair in sids)
            {
                var sid = pair.K;
                var level = (int)pair.V;
                var skill = new BattleSkill(sid, level, Owner);
                switch (skill.SkillType)
                {
                    case BattleDef.ESkillType.Normal:
                        normalSkill = skill;
                        continue;
                    case BattleDef.ESkillType.Rage:
                        rageSkill = skill;
                        continue;
                    case BattleDef.ESkillType.Opening:
                        openingSkills.Add(skill);
                        continue;
                    case BattleDef.ESkillType.Passive:
                        passiveSkills.Add(skill);
                        continue;
                    case BattleDef.ESkillType.None:
                    default:
                        Log.error($"技能{sid},缺省类型:{skill.SkillType}");
                        continue;
                }
            }
        }

        #endregion


        public void Dispose()
        {
            mSid = 0u;
            Uid = 0u;
            Cfg = null;
            openingSkills = null;
            passiveSkills = null;
            normalSkill = null;
            rageSkill = null;
            Owner = null;

            for (int i = 0; i < nowProps.Length; i++)
            {
                initProps[i] = 0;
                nowProps[i] = 0;
            }
        }
    }
}
