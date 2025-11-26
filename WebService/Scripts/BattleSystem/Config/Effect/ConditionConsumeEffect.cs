namespace BattleSystem
{
	/// <summary>
	/// 满足条件时消耗效果
	/// </summary>
	public class ConditionConsumeEffect : BaseEffect
	{
		/// <summary>
		/// 消耗类型
		/// </summary>
		public BattleObject.Event consumeType;

		/// <summary>
		/// 目标触发效果
		/// </summary>
		public long[] targetEffects;

		/// <summary>
		/// 可消耗次数
		/// </summary>
		public long consumeNum;

		/// <summary>
		/// 是否是自己消耗
		/// </summary>
		public bool authorConsume;

		/// <summary>
		/// 本效果结束时也结束后续效果
		/// </summary>
		public bool endFollowUpEffects;

		private long _consumeNum;

		private BaseEffect[] _effects;
		private bool[] _effectEndStatus; //BaseEffect有缓存，所以另外用一个数据记录end状态
		private int _effectLength = 0;

		private void AddEffects()
		{
			var effectLength = targetEffects.Length;
			if (targetEffects != null && effectLength > 0)
			{
				target.Effect.AddEffects(targetEffects, authorUid, effectLevel);

				if (_effects == null)
				{
					_effectLength = effectLength;
					_effects = new BaseEffect[effectLength];
					_effectEndStatus = new bool[effectLength];
				}

				for (int i = 0; i < effectLength; i++)
				{
					var effect = target.Effect.GetEffect(authorUid, targetEffects[i]);
					if (effect != null && !effect.isEnd)
					{
						_effects[i] = effect;
						_effectEndStatus[i] = false;
					}
					else
					{
						_effects[i] = null;
						_effectEndStatus[i] = true;
					}
				}
			}
		}

		public override void Trigger()
		{
			AddEffects();
		}

		public override void OnOverlayAdd()
		{
			if (overlayCount < addMax)
			{
				AddEffects();
			}

			base.OnOverlayAdd();
		}

		public override void OnOverlayReduce()
		{
			if (overlayCount > 0)
			{
				for (int i = 0; i < _effectLength; i++)
				{
					var effect = _effects[i];
					if (_effectEndStatus[i] || effect == null || effect.isEnd)
					{
						continue;
					}

					if (effect.IsType(EffectSys.EffectType.Overlays))
					{
						effect.OnOverlayReduce();
					}
					_effectEndStatus[i] = effect.isEnd;
				}
			}

			base.OnOverlayReduce();
		}

		public override void OnBegin()
		{
			_effectLength = 0;
			_consumeNum = consumeNum > 0 ? consumeNum : 1;
			if (consumeType != BattleObject.Event.None)
			{
				var consumeTarget = authorConsume ? author : target;
				consumeTarget.AddEvent(consumeType, OnCondition, false, 1);
			}

			base.OnBegin();
		}

		public override void OnEnd()
		{
			if (consumeType != BattleObject.Event.None)
			{
				var consumeTarget = authorConsume ? author : target;
				consumeTarget.RemoveEvent(consumeType, OnCondition);
			}

			if (endFollowUpEffects)
			{
				for (int i = 0; i < _effectLength; i++)
				{
					var effect = _effects[i];
					if (_effectEndStatus[i] || effect == null || effect.isEnd)
					{
						continue;
					}

					target.Effect.ClearEffect(effect);
				}
			}

			_effectLength = 0;
			_consumeNum = 0;
			_effects = null;
			_effectEndStatus = null;
			base.OnEnd();
		}

		private void OnCondition(EventParams arg0)
		{
			if (--_consumeNum > 0)
			{
				return;
			}

			var hasEffect = false;
			for (int i = 0; i < _effectLength; i++)
			{
				var effect = _effects[i];
				if (_effectEndStatus[i] || effect == null || effect.isEnd)
				{
					continue;
				}

				if (effect.IsType(EffectSys.EffectType.Overlays))
				{
					effect.OnOverlayReduce();
				}
				else
				{
					target.Effect.ClearEffect(effect);
				}

				_effectEndStatus[i] = effect.isEnd;
				if (!effect.isEnd)
				{
					hasEffect = true;
				}
			}

			if (!hasEffect)
			{
				target.Effect.ClearEffect(this);
			}
		}
	}
}