namespace BattleSystem
{
	/// <summary>
	/// 转移伤害
	/// </summary>
	public class TransferDamageEffect : BaseEffect
	{
		/// <summary>
		/// 转移比例（百分比）
		/// </summary>
		public double value;

		/// <summary>
		/// 伤害从自己转移到目标
		/// </summary>
		public bool transferToTarget;

		/// <summary>
		/// 剩余伤害是否清零
		/// </summary>
		public bool noLeftDamage;

		/// <summary>
		/// 是否特定伤害类型生效
		/// </summary>
		public bool careDamageType;

		/// <summary>
		/// 伤害类型
		/// </summary>
		public BattleDef.DamageType damageType;

		/// <summary>
		/// 是否指定转移后的伤害类型
		/// </summary>
		public bool careTransferredDamageType;

		/// <summary>
		/// 转移后的类型
		/// </summary>
		public BattleDef.DamageType transferredDamageType;

		/// <summary>
		/// 转移给攻击方
		/// </summary>
		public bool transferToAttacker;

		/// <summary>
		/// 转移几次后删除自身
		/// </summary>
		public int transferCount;

		private int _transferCount;

		public override void OnBegin()
		{
			_transferCount = 0;
			if (transferToTarget)
			{
				author.AddEvent(BattleObject.Event.BeDamagePre, OnDamagePre, false, 2); //要在shield的BeDamagePre之前执行，事件是倒序执行，所以要比shield的BeDamagePre的1大才行 by ww
			}
			else
			{
				target.AddEvent(BattleObject.Event.BeDamagePre, OnDamagePre, false, 2); //要在shield的BeDamagePre之前执行，事件是倒序执行，所以要比shield的BeDamagePre的1大才行 by ww
			}
			base.OnBegin();
		}

		private void OnDamagePre(EventParams eventParams)
		{
			if (value <= 0.0d || eventParams.IsBlockEvent || eventParams is not DamageParams args || args.isTransferDamage)
			{
				return;
			}

			if (battle == null)
			{
				return;
			}

			if (careDamageType && args.damageType != damageType)
			{
				return;
			}

			var transferredFighter = transferToTarget ? target : author;
			if (transferToAttacker)
			{
				transferredFighter = battle.GetSceneObject<Fighter>(args.attackUid);
			}

			if (transferredFighter == null || transferredFighter.IsDead)
			{
				return;
			}

			if (transferredFighter.Uid == args.defendUid)
			{
#if UNITY_EDITOR
				battle.AddInfo(target.GetBaseDesc()).AddInfo(" 转移失败，自己转移给自己！ ", true);
#endif
				return;
			}

			var scale = value * BattleDef.Percent;
			var transferDamage = System.Math.Min((long)YKMath.Ceiling(args.newValue * scale), args.newValue);
			args.newValue = noLeftDamage ? 0 : System.Math.Max(args.newValue - transferDamage, 0);

#if UNITY_EDITOR
			battle.AddInfo(transferToTarget ? author.GetBaseDesc() : target.GetBaseDesc()).AddInfo("TransferDamageEffect-").AddInfo(sid).
					AddInfo(" 转移 ").AddInfo(transferDamage).AddInfo(" 伤害给 ").AddInfo(transferredFighter.GetBaseDesc()).AddInfo("，剩余伤害：").
					AddInfo(args.newValue, true);
#endif

			if (transferDamage > 0)
			{
				var fakeDamageEffectCfg = new DamageEffectCfg();
				fakeDamageEffectCfg.id = -999999;
				if (fakeDamageEffectCfg.Create(transferredFighter.Effect.Owner, target.Uid) is DamageEffect beDamageEffect)
				{
					beDamageEffect.damageType = careTransferredDamageType ? transferredDamageType : BattleDef.DamageType.Indirect;
					beDamageEffect.calculatedDamage = transferDamage;
					beDamageEffect.rate = 1.0;
					beDamageEffect.isRealDamage = true;
					beDamageEffect.isTransferDamage = true;
					transferredFighter.Effect.Battle.BattleInfo?.ListenEffectInfo(beDamageEffect);
					transferredFighter.Effect.AddEffect(beDamageEffect);
				}
			}

			if (transferCount > 0 && ++_transferCount >= transferCount)
			{
				if (!isEnd)
				{
					target.Effect.ClearEffect(this);
				}
			}
		}

		public override void Trigger()
		{
		}

		public override void OnEnd()
		{
			if (transferToTarget)
			{
				author.RemoveEvent(BattleObject.Event.BeDamagePre, OnDamagePre);
			}
			else
			{
				target.RemoveEvent(BattleObject.Event.BeDamagePre, OnDamagePre);
			}

			base.OnEnd();
		}
	}
}