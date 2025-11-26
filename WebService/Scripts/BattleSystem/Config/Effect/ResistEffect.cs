using System.Collections.Generic;

namespace BattleSystem
{
	/// <summary>
	/// 抵抗Effect效果
	/// </summary>
	public class ResistEffect : BaseEffect
	{
		/// <summary>
		/// 受到效果类型
		/// </summary>
		public EffectSys.EffectType[] effectEvent;

		/// <summary>
		/// 自身状态
		/// </summary>
		public BattleObject.State state;

		/// <summary>
		/// 扣除效果类型
		/// </summary>
		public EffectSys.EffectType resistEffectEvent;

		/// <summary>
		/// 扣除次数
		/// </summary>
		public int resistNum;

		private List<BaseEffect> _dealEffectCache; //不能用static，战斗服是多线程 by ww
		private EffectSys.EffectType _effectEvent;

		public override void OnBegin()
		{
			_effectEvent = EffectSys.EffectType.Null;
			for (int i = 0, l = effectEvent.Length; i < l; i++)
			{
				_effectEvent |= effectEvent[i];
			}

			target.AddEvent(EffectSys.Event.BeginPre, OnEffectBeginPre, false, 1);
			target.AddEvent(EffectSys.Event.OverlayAddPre, OnEffectOverlayAddPre, false, 1);

			base.OnBegin();
		}

		public override void OnEnd()
		{
			target.RemoveEvent(EffectSys.Event.BeginPre, OnEffectBeginPre);
			target.RemoveEvent(EffectSys.Event.OverlayAddPre, OnEffectOverlayAddPre);

			base.OnEnd();
		}

		public override void Trigger()
		{
		}

		private void OnTargetEffectWillAdd(EventParams arg0)
		{
			if (!(arg0 is EventParams<BaseEffect> effectArg))
			{
				return;
			}

			var effect = effectArg.data;
			if (resistNum <= 0 || !target.IsState(state))
			{
				if (effect.IsType(_effectEvent))
				{
					effectArg.IsBlockEvent = true;
				}

				return;
			}

			if (effect == null || !effect.IsType(_effectEvent))
			{
				return;
			}

			if (_dealEffectCache == null)
			{
				_dealEffectCache = new();
			}

			_dealEffectCache.Clear();
			var count = 0;
			var allEffectList = target.Effect.AllEffectList;
			for (int i = 0, l = allEffectList.Count; i < l; i++)
			{
				var e = allEffectList[i];
				if (e == null)
				{
					continue;
				}

				if (e.IsType(resistEffectEvent))
				{
					_dealEffectCache.Add(e);
					if (e.IsType(EffectSys.EffectType.Overlays))
					{
						count += e.overlayCount;
					}
					else
					{
						count++;
					}

					if (count >= resistNum)
					{
						break;
					}
				}
			}

			if (count < resistNum)
			{
				_dealEffectCache.Clear();
				return;
			}

			effectArg.IsBlockEvent = true;
			count = resistNum;
			for (int i = 0, l = _dealEffectCache.Count; i < l; i++)
			{
				var dealEffect = _dealEffectCache[i];
				if (dealEffect.IsType(EffectSys.EffectType.Overlays))
				{
					var countReduce = System.Math.Min(count, dealEffect.overlayCount);
					for (int j = 0; j < countReduce; j++)
					{
						dealEffect.OnOverlayReduce();
					}
					count -= countReduce;
				}
				else
				{
					target.Effect.ClearEffect(dealEffect);
					count--;
				}

				if (count <= 0)
				{
					break;
				}
			}
			_dealEffectCache.Clear();
		}

		private void OnEffectBeginPre(EventParams arg0)
		{
			OnTargetEffectWillAdd(arg0);
		}

		private void OnEffectOverlayAddPre(EventParams arg0)
		{
			OnTargetEffectWillAdd(arg0);
		}
	}
}