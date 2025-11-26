using System;
using System.ComponentModel;

namespace BattleSystem
{
	/// <summary>
	/// 免疫伤害
	/// </summary>
	public class ImmuneDamage : BaseEffect
	{
		/// <summary>
		/// 免疫伤害类型
		/// </summary>
		public ImmuneDamageType checkType;

		/// <summary>
		/// 血量数值
		/// </summary>
		public long hpValue;

		/// <summary>
		/// 触发后下回合移除
		/// </summary>
		public bool triggerNextBoutClear = false;

		/// <summary>
		/// 触发几个小回合后移除（次数，2表示有2个小回合内都免疫）
		/// </summary>
		public int triggerNextRoundClearCount = 0;

		/// <summary>
		/// 触发几次免死后移除(0不生效)
		/// </summary>
		public int triggerCountClear = 0;

		/// <summary>
		/// 当前已触发次数
		/// </summary>
		public int curTriggerCount = 0;

        /// <summary>
        /// 检测的属性
        /// </summary>
        public BattleDef.Property checkPropType;

        /// <summary>
        /// 检测的属性类型
        /// </summary>
        public BattleDef.CompareType compareType;  

        /// <summary>
        /// 属性值(百分比)
        /// </summary>
        public double value;

        /// <summary>
        /// 忽略来自队友的伤害
        /// </summary>
        public bool ingoreTeammate; 

        private Fighter mBind;
		private int mTriggerBout = -1;
		/// <summary>
		/// 是否已经触发
		/// </summary>
		private bool mIsTrigger = false;
		private bool mIsTriggerInThisRound = false;

		public bool triggeredThisDamage = false;
		private int _roundNum = -1;
		private int _roundTriggerCount = -1;
		private int _triggerRoundNum = -1;
		private long _reboundDamage = 0;

		public override void OnBegin()
		{
			triggeredThisDamage = false;
			mIsTrigger = false;
			mTriggerBout = -1;
			_reboundDamage = 0;
			hpValue = Math.Max(hpValue, 1); //强制保留1点，怕策划忘记配
			target.AddEvent(BattleObject.Event.BeDamagePre, OnDamagePre, false,
							0); //要在shield的BeDamagePre之后执行，事件是倒序执行，所以要比shield的BeDamagePre的1小才行 by ww
			target.AddEvent(BattleObject.Event.OtherBeDamagePre, OnDamagePre, false, 0);
			target.AddEvent(BattleObject.Event.BeDamage, OnDamage, false, 0);
			target.AddEvent(BattleObject.Event.BeDeBuffDamage, OnDamage, false, 0);

			if (triggerNextRoundClearCount > 0)
			{
				battle.AddEvent(BaseBattle.Event.RoundStart, OnAnyRoundStart);
				_roundNum = 0;
			}

			mBind = (Fighter)target;
			curTriggerCount = 0;
			_roundTriggerCount = 0;
			base.OnBegin();
		}

		private void OnDamagePre(EventParams eventParams)
		{
			triggeredThisDamage = false;
			var trigger = false;
			if (!eventParams.IsBlockEvent && eventParams is DamageParams args)
            {
                if (!Pass(args.newValue))
                {
                    return;
                }
                var attacker = battle.GetSceneObject<Fighter>(args.attackUid);
				if (ingoreTeammate && attacker.CampType == mBind.CampType) 
				{
					return;
				}
                switch (checkType)
				{
					case ImmuneDamageType.Dead:
					{
						if (args.newValue >= mBind.Data[BattleDef.Property.hp])
						{
							var ignoreImmuneDead = false;
							if (attacker != null && attacker.IsState(BattleObject.State.IgnoreImmuneDead | BattleObject.State.ZhenShi))
							{
								ignoreImmuneDead = true;
#if UNITY_EDITOR
								battle.AddInfo($"ImmuneDamage---Dead：{attacker.Data.Cfg?.name}---无视免死", true);
#endif
							}

							if (!ignoreImmuneDead)
							{
								trigger = true;
								args.newValue = Math.Max(mBind.Data[BattleDef.Property.hp] - hpValue, 0);
							}
						}
						break;
					}
					case ImmuneDamageType.Damage:
					{
						if (attacker != null && attacker.Data.jobType == BattleDef.EJobType.Pet)
						{
							//不能免疫宠物伤害
							break;
						}

						trigger = true;
#if UNITY_EDITOR
						battle.AddInfo("ImmuneDamage---Damage：---免疫伤害", true);
#endif
						args.newValue = 0;
						break;
					}
					case ImmuneDamageType.DeadResist:
					{
						if (args.newValue >= mBind.Data[BattleDef.Property.hp])
						{
							trigger = true;
							_reboundDamage = args.newValue;
							args.newValue = 0;
#if UNITY_EDITOR
							battle.AddInfo("ImmuneDamage---DeadRebound：---致死伤害反弹", true);
#endif
						}
						break;
					}
					default:
						Log.error($"需要实现{GetType()}-{sid}中的免疫伤害类型：{checkType}");
						break;
				}
			}

			if (trigger)
			{
				if (!mIsTrigger)
				{
					mTriggerBout = battle.Bout;
				}

				if (!mIsTriggerInThisRound)
				{
					_triggerRoundNum = _roundNum;
				}

				triggeredThisDamage = true;
				mIsTrigger = true;
				mIsTriggerInThisRound = true;
				//触发次数提供
				TriggerBegin();
				curTriggerCount++;
			}
		}

		private void OnDamage(EventParams eventParams)
        {
            var checkClear = false;

			if (triggeredThisDamage)
			{
				var attackerUid = 0L;
				if (!eventParams.IsBlockEvent && eventParams is DamageParams args)
				{
					switch (checkType)
					{
						case ImmuneDamageType.Dead:
						{
							checkClear = true;
							break;
						}
						case ImmuneDamageType.Damage:
						{
							checkClear = true;
							args.newValue = 0;
							break;
						}
						case ImmuneDamageType.DeadResist:
						{
							checkClear = true;
							args.newValue = 0;
							attackerUid = args.attackUid;
							if (args.isTransferDamage)
							{
								_reboundDamage = 0;
							}
							break;
						}
						default:
							break;
					}
				}
				target.DispatchEvent(battle, BattleObject.Event.ImmuneDamageTrigger, EventParams<ImmuneDamageType>.Create(battle, checkType));

				if (checkType == ImmuneDamageType.DeadResist && _reboundDamage > 0)
				{
					var fakeDamageEffectCfg = new DamageEffectCfg();
					fakeDamageEffectCfg.id = -999999;
					//var rebounder = battle.GetSceneObject<Fighter>(attackerUid);
					var resister = author;
					if (resister != null && !resister.IsDead && resister != target)
					{
						if (fakeDamageEffectCfg.Create(resister.Effect.Owner, target.Uid) is DamageEffect beDamageEffect)
						{
							beDamageEffect.damageType = BattleDef.DamageType.Indirect;
							beDamageEffect.calculatedDamage = _reboundDamage;
							beDamageEffect.rate = 1.0;
							beDamageEffect.isRealDamage = true;
							beDamageEffect.isTransferDamage = true;
							resister.Effect.Battle.BattleInfo?.ListenEffectInfo(beDamageEffect);
							resister.Effect.AddEffect(beDamageEffect);
						}
					}
				}
				_reboundDamage = 0;
			}
			triggeredThisDamage = false;

			if (checkClear)
			{
				// 免死触发一定次数后的移除写在这里。OnDamagePre的时候还没扣血，写在那里有时序问题
				if (triggerCountClear > 0 && curTriggerCount >= triggerCountClear)
				{
					target.Effect.ClearEffect(this);
				}
			}
		}

		private bool Pass(long dmg)
		{
			if (checkPropType == BattleDef.Property.None)
			{
				return true;
			}
			else
			{
				if (target is Fighter mTarget)
				{
					double curValue = mTarget.Data.nowProps[(int)checkPropType] * value * BattleDef.Percent;
					switch (compareType)
					{
						case BattleDef.CompareType.Small:
							return dmg < curValue;
						case BattleDef.CompareType.SmallAndEqual:
							return dmg <= curValue;
						case BattleDef.CompareType.Equal:
							return Math.Abs(dmg - curValue) < BattleDef.Percent;
						case BattleDef.CompareType.Big:
							return dmg > curValue;
						case BattleDef.CompareType.BigAndEqual:
							return dmg >= curValue;
						default:
							return true;
					}
				}
				else
				{
					return true;
				}
			}
		}
		private void OnAnyRoundStart(EventParams eventParams)
		{
			_roundNum++;
			if (mIsTriggerInThisRound && _roundNum > _triggerRoundNum)
			{
				_roundTriggerCount++;

				if (triggerNextRoundClearCount > 0 && _roundTriggerCount >= triggerNextRoundClearCount)
				{
					target.Effect.ClearEffect(this);
				}
			}

			mIsTriggerInThisRound = false;
		}

		public override void OnBoutStart(EventParams arg)
		{
			base.OnBoutStart(arg);
			if (triggerNextBoutClear && mIsTrigger && battle.Bout > mTriggerBout)
			{
				target.Effect.ClearEffect(this);
			}
		}

		public override void Trigger()
		{
		}

		public override void OnEnd()
		{
			target.RemoveEvent(BattleObject.Event.BeDamagePre, OnDamagePre);
			target.RemoveEvent(BattleObject.Event.OtherBeDamagePre, OnDamagePre);
			target.RemoveEvent(BattleObject.Event.BeDamage, OnDamage);
			target.RemoveEvent(BattleObject.Event.BeDeBuffDamage, OnDamage);

			if (triggerNextRoundClearCount > 0)
			{
				battle.RemoveEvent(BaseBattle.Event.RoundStart, OnAnyRoundStart);
			}

			mTriggerBout = -1;
			mIsTrigger = false;
			mIsTriggerInThisRound = false;
			triggeredThisDamage = false;
			_roundNum = -1;
			_roundTriggerCount = -1;
			_triggerRoundNum = -1;

			base.OnEnd();
		}
	}

	/// <summary>
	/// 免疫伤害条件
	/// </summary>
	public enum ImmuneDamageType
	{
		/// <summary>
		/// 免疫致死伤害
		/// </summary>
		[Description("免疫致死伤害(保留1滴血)")]
		Dead,
		/// <summary>
		/// 免疫伤害
		/// </summary>
		[Description("免疫伤害")]
		Damage,
		/// <summary>
		/// 致死伤害抵挡
		/// </summary>
		[Description("致死伤害抵挡")]
		DeadResist,
	}
}