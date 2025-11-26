#if UNDEF_BATTLE_UNITY_EDITOR
#undef UNITY_EDITOR
#endif

namespace BattleSystem
{
	/// <summary>
	/// 伤害转换成属性效果
	/// </summary>
	public class DamageToPropertyEffect : BaseEffect
	{
		/// <summary>
		/// 转换比例（百分比）
		/// </summary>
		public double value;

		/// <summary>
		/// 属性
		/// </summary>
		public BattleDef.Property property;

		/// <summary>
		/// 转换成护盾值
		/// </summary>
		public bool toShield;

		public override void OnBegin()
		{
			target.AddEvent(BattleObject.Event.AttackDamage, OnDamage, false, -50);
			base.OnBegin();
		}

		public override void OnEnd()
		{
			target.RemoveEvent(BattleObject.Event.AttackDamage, OnDamage);
			base.OnEnd();
		}

		public override void Trigger()
		{
		}

		private void OnDamage(EventParams eventParams)
		{
			if (target is not Fighter targetFighter)
			{
				return;
			}

			if (value <= 0.0d || eventParams.IsBlockEvent || eventParams is not DamageParams args)
			{
				return;
			}

			if (battle == null)
			{
				return;
			}

			var scale = value * BattleDef.Percent;
			var toProperty = System.Math.Min((long)YKMath.Ceiling(args.newValue * scale), args.newValue);
			if (toProperty == 0)
			{
				return;
			}

			if (toShield)
			{
				targetFighter.AddShield(toProperty);
			}
			else
			{
				targetFighter.AddProp(property, toProperty);
			}
		}
	}
}