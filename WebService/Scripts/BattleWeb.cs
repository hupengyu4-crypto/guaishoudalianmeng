using RootScript.Config;
using BattleSystem;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using static BattleSystem.BattleDef;

namespace WebService
{
    public class BattleWeb
    {
        public BaseBattle Battle { get; private set; }

        private static readonly object _syncLock = new object();
        private static bool _isConfigInit = false;

        private readonly web_req mWebReq;

        public static string ConfigBasePath;

        public static void ConfigInitCheck(_Default def)
        {
            if (!_isConfigInit)
            {
                lock (_syncLock)
                {
                    if (!_isConfigInit)
                    {
                        InitConfigManagerNew(def);
                        _isConfigInit = true;
                    }
                }
            }
        }

        public static void ReloadConfig(_Default def)
        {
            _isConfigInit = false;
            ConfigManagerNew.Instance.Release();
            ConfigInitCheck(def);
        }

        public BattleWeb(byte[] dataBytes)
        {
            lock (_syncLock)
            {
                //处理多线程问题
                var init1 = BattleDef.IsPropertyCoefficient;
                var init2 = BattleDef.MaxProps;
                var init3 = BattleDef.BattleSceneInfo;
                BattleUtils.InitPropertyName();
            }

            mWebReq = PbManager<web_req>.ParseFrom(dataBytes);
            Battle = BattleUtils.CreateBattle(mWebReq.Field);
            Battle.InitData(mWebReq.Field);
            Battle.Cmd.SetCmd(mWebReq.Args.ToByteArray());
        }

        private static void InitConfigManagerNew(_Default def)
        {
            // 加载失败日志事件
            ConfigManagerNew.Instance.loadRowFailure = (error) => { def.Response.Write($"配置表加载失败 {error}"); };

            ConfigManagerNew.Instance.Init(ConfigBytesDataLoadMgr.Instance, (impl =>
            {
                new GameConfigRegister().Initialize(impl);

                ConfigManagerNew.Instance.LoadAll();
            }));

            //InitConfigs(def);
        }

        private static void InitConfigs(_Default def)
        {
            try
            {
                Assembly asm = typeof(BaseBattle).Assembly;
                string rootPath = string.IsNullOrEmpty(ConfigBasePath)
                    ? AppDomain.CurrentDomain.BaseDirectory
                    : ConfigBasePath;
                DirectoryInfo root = new DirectoryInfo(rootPath + "Config");
                var files = root.GetFiles("*.txt", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    string name = Path.GetFileNameWithoutExtension(files[i].Name);
                    if (string.IsNullOrEmpty(name)) continue;
                    Console.WriteLine($"配置:{name}");
                    ConfigManagerNew.Instance.Get(name);
                    //if (name == "ConfigLevelVar")
                    //{
                    //    Type type = typeof(ConfigLevelVar);
                    //    string value = File.ReadAllText(files[i].FullName);
                    //    ConfigManager.Instance.ImportStrInfo(type, value);
                    //}
                    //else
                    //{
                    //    Type curType = asm.GetType(name, false, false);
                    //    if (curType != null)
                    //    {
                    //        Type baseType = curType.BaseType;
                    //        string value = File.ReadAllText(files[i].FullName);
                    //        if (baseType == typeof(Config))
                    //        {
                    //            ConfigManager.Instance.ImportStrInfo(curType, value);
                    //        }
                    //        else
                    //        {
                    //            for (int limit = 0; limit < 5; limit++)
                    //            {
                    //                if (baseType == null || baseType.IsAbstract) break;
                    //                limit++;
                    //                baseType = baseType.BaseType;
                    //            }
                    //            if (baseType == null)
                    //                ConfigManager.Instance.ImportStrInfo(curType, value);
                    //            else
                    //                ConfigManager.Instance.ImportStrInfo(baseType, value, curType);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        Log.Error("Config error:" + name);
                    //    }
                    //}
                }
            }
            catch (Exception e)
            {
                def.Response.Write(e.Message);
                throw;
            }
        }

        private Dictionary<long, attack_card> mAllCards = new Dictionary<long, attack_card>();

        /// <summary>
        /// 进攻方战斗信息
        /// </summary>
        private RepeatedField<attack_card> mAttackCards = new RepeatedField<attack_card>();

        /// <summary>
        /// 防守方战斗信息
        /// </summary>
        private RepeatedField<attack_card> mDefendCards = new RepeatedField<attack_card>();

        private RepeatedField<web_reply> mWebReplys = new RepeatedField<web_reply>();
        private uint mFrame = 0;

        private void AddDamage(EventParams eventParams)
        {
            DamageParams damage = (DamageParams)eventParams;
            mAllCards[damage.attackUid].Hurt += damage.newValue;
            mAllCards[damage.defendUid].Injury += damage.newValue;
        }

        private void AddHeal(EventParams eventParams)
        {
            HealParams heal = (HealParams)eventParams;
            mAllCards[heal.casterkUid].Treat += heal.newValue;
            mAllCards[heal.targetUid].BeTreated += heal.newValue;
        }

        private void AddKill(EventParams eventParams)
        {
            if (eventParams != null && eventParams is EventTwoParams<Fighter, PropParams> killer)
            {
                if (killer.data1 != null)
                {
                    mAllCards[killer.data1.Uid].KillNum += 1;
                }
            }
        }

        private void AddShieldAbsorption(EventParams eventParams)
        {
            if (eventParams is ShieldAbsorptionParams shieldAbsorptionParams)
            {
                if (shieldAbsorptionParams.attackUid != 0)
                {
                    mAllCards[shieldAbsorptionParams.attackUid].Hurt += shieldAbsorptionParams.shieldAbsorptionValue;
                }

                if (shieldAbsorptionParams.defendUid != 0)
                {
                    mAllCards[shieldAbsorptionParams.defendUid].Injury += shieldAbsorptionParams.shieldAbsorptionValue;
                }
            }
        }

        void OnAddFighter(Fighter f)
        {
            if (!mAllCards.ContainsKey(f.Uid))
            {
                var data = f.Data;
                attack_card it = new attack_card
                {
                    Uid = f.Uid,
                    Sid = data.Sid,
                    MaxHp = data[BattleDef.Property.max_hp],
                    Hp = 0,
                    Hurt = 0,
                    Injury = 0,
                    Treat = 0,
                    BeTreated = 0,
                    KillNum = 0,
                    AliveRound = 0,
                    ControlledRound = 0,
                    UseSkillNum = 0,
                    CritNum = 0,
                    BlockNum = 0,
                    InitHp = data[BattleDef.Property.hp],
                    InitMaxHp = data[BattleDef.Property.max_hp],
                    InitDeadHp = data[BattleDef.Property.dead_hp],
                };
                if (f.CampType == BattleDef.TeamCampType.Attacker)
                {
                    mAttackCards.Add(it);
                }
                else
                {
                    mDefendCards.Add(it);
                }

                mAllCards.Add(f.Uid, it);
                f.AddEvent(BattleObject.Event.NormalDamage, AddDamage);
                f.AddEvent(BattleObject.Event.SkillDamage, AddDamage);
                f.AddEvent(BattleObject.Event.DeBuffDamage, AddDamage);
                f.AddEvent(BattleObject.Event.IndirectDamage, AddDamage);
                f.AddEvent(BattleObject.Event.Heal, AddHeal);
                f.AddEvent(BattleObject.Event.BeKill, AddKill);
                f.AddEvent(BattleObject.Event.ShieldAbsorption, AddShieldAbsorption);
            }
        }
        
        private void AddToAllCards(RepeatedField<attack_card> cards, FastIterationDictionary<long, BattleObject> allSceneObjects)
        {
            for (int i = 0, l = cards.Count; i < l; i++)
            {
                var card = cards[i].Clone();
                mAllCards.Add(card.Uid, card);
                cards[i] = card;
                card.Uid = card.Uid;
                card.Sid = card.Sid;
                card.MaxHp = card.MaxHp;
                card.Hp = 0;
                card.Hurt = 0;
                card.Injury = 0;
                card.Treat = 0;
                card.BeTreated = 0;
                card.KillNum = 0;
                card.AliveRound = 0;
                card.ControlledRound = 0;
                card.UseSkillNum = 0;
                card.CritNum = 0;
                card.BlockNum = 0;

                card.InitHp = card.MaxHp;
                card.InitMaxHp = card.MaxHp;
                card.InitDeadHp = 0;
                allSceneObjects.TryGetValue(card.Uid, out var battleObject);
                if (battleObject is Fighter fighter)
                {
                    var fData = fighter.Data;
                    card.InitHp = fData[BattleDef.Property.hp];
                    card.InitMaxHp = fData[BattleDef.Property.max_hp];
                    card.InitDeadHp = fData[BattleDef.Property.dead_hp];
                }
            }
        }

        private void DealBattleOver(BaseBattle battle, bool allOver)
        {
            foreach (var value in battle.allSceneObjects.Values)
            {
                if (value is Fighter f)
                {
                    var fData = f.Data;
                    var cardInfo = mAllCards[f.Uid];
                    cardInfo.Hp = fData[BattleDef.Property.hp];
                    cardInfo.MaxHp = fData[BattleDef.Property.max_hp];
                    cardInfo.DeadHp = fData[BattleDef.Property.dead_hp];
                    var statisticsData = f.statisticsData;
                    cardInfo.AliveRound = statisticsData.aliveBout;
                    cardInfo.ControlledRound = statisticsData.beControlledNum;
                    cardInfo.UseSkillNum = statisticsData.skillNum;
                    cardInfo.CritNum = statisticsData.critNum;
                    cardInfo.BlockNum = statisticsData.blockNum;
                }
            }

            var clearAttack = true;
            var clearDefend = true; //现在都是新创建的fighter
            //if (battle is KOFBattle kofBattle)
            //{
            //    if (!allOver)
            //    {
            //        clearDefend = battle.gameOverState == BattleDef.BattleResult.Win;
            //        clearAttack = !clearDefend;
            //    }
            //}

            battler atkBattler = null;
            battler defBattler = null;
            if (battle is NormalBattle normalBattle)
            {
                atkBattler = normalBattle.Attackers.TeamData.Count > 0 ? normalBattle.Attackers.TeamData[0] : null;
                defBattler = normalBattle.Defenders.TeamData.Count > 0 ? normalBattle.Defenders.TeamData[0] : null;
            }

            var atkUid = 0L;
            var atkArrayId = 0L;
            if (atkBattler != null)
            {
                atkUid = atkBattler.Uid;
                atkArrayId = atkBattler.ArrayId;
            }

            var defUid = 0L;
            var defArrayId = 0L;
            if (defBattler != null)
            {
                defUid = defBattler.Uid;
                defArrayId = defBattler.ArrayId;
            }

            web_reply data = new web_reply
            {
                Round = battle.Bout,
                Frame = battle.Frame - mFrame,
                Win = (int)battle.gameOverState,
                AtkCard = { mAttackCards },
                DfsCard = { mDefendCards },
                Verson = 1,
                AtkCamp = new i_reply_camp
                {
                    Uid = atkUid,
                    ArrayId = atkArrayId,
                },
                DefCamp = new i_reply_camp
                {
                    Uid = defUid,
                    ArrayId = defArrayId,
                },
            };
            mWebReplys.Add(data);
            mFrame = battle.Frame;

            mAllCards.Clear();
            if (clearAttack)
            {
                mAttackCards.Clear();
            }
            else
            {
                AddToAllCards(mAttackCards, battle.allSceneObjects);
            }

            if (clearDefend)
            {
                mDefendCards.Clear();
            }
            else
            {
                AddToAllCards(mDefendCards, battle.allSceneObjects);
            }
        }

        /// <summary>
        /// 开始战斗
        /// </summary>
        public void BattleBegin()
        {
            mAllCards.Clear();
            mAttackCards.Clear();
            mDefendCards.Clear();


            foreach (var value in Battle.allSceneObjects.Values)
            {
                if (value != null && value is Fighter f)
                {
                    OnAddFighter(f);
                }
            }

            Battle.AddEvent(BaseBattle.Event.AddSceneObject, (arg0) =>
            {
                if (arg0 is EventParams<BattleObject> param)
                {
                    if (param.data != null && param.data is Fighter f)
                    {
                        OnAddFighter(f);
                    }
                }
            });
            Battle.AddEvent(BaseBattle.Event.BattleOver, args =>
            {
                DealBattleOver(Battle, true);
            });
            Battle.AddEvent(BaseBattle.Event.BattleTeamOver, args =>
            {
                DealBattleOver(Battle, false);
            });


            int index = 0;
            while (!Battle.IsBattleOver)
            {
                if (index++ > 2000)
                {
                    throw new Exception("战斗死循环！！！！！！");
                }

                Battle.UpdateLogic();
            }
        }

        public web_reply_l BattleWebData()
        {
            web_reply_l data = new web_reply_l
            {
                Win = (int)Battle.gameOverState,
                WebReply = { mWebReplys },
            };
            return data;
        }

        public void Clear()
        {
            Battle.Dispose();
            Battle = null;
            mAttackCards.Clear();
            mDefendCards.Clear();
        }
    }
}