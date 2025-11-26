using System.Collections.Generic;

namespace BattleSystem
{
	/// <summary>
	/// 护盾偷取
	/// </summary>
	public class ShieldStealEffect : BaseEffect
	{
		/// <summary>
		/// 属性系数(百分比)
		/// </summary>
		public double factor;

		/// <summary>
		/// 索敌
		/// </summary>
		public long findEffects;

		/// <summary>
		/// 星级加成（百分比）
		/// </summary>
		public double starValue;

		/// <summary>
		/// 施法者
		/// </summary>
		private Fighter mAuthor;

		/// <summary>
		/// 目标
		/// </summary>
		private Fighter mTarget;

		public override void Trigger()
		{
			mAuthor = (Fighter)author;
			mTarget = (Fighter)target;
			var scale = (factor + mAuthor.Data.Star * starValue) * BattleDef.Percent;
			if (scale <= 0)
			{
				return;
			}

			List<Fighter> allTargets = null;
			target.Effect.AddEffect(findEffects, target.Uid, effectLevel, out var e);
			if (e is FindTargetEffect findTargetEffect)
			{
				allTargets = findTargetEffect.GetFighters();
			}
			target.Effect.ClearEffect(e);

			if (allTargets == null)
			{
				return;
			}

			var stealShield = 0L;
			for (int i = 0, l = allTargets.Count; i < l; i++)
			{
				var t = allTargets[i];
				if (t.IsDead)
				{
					continue;
				}

				var shield = (long)YKMath.Ceiling(t.GetShield() * scale);
				t.AddShield(-shield);
				stealShield += shield;
			}

			if (stealShield > 0)
			{
#if UNITY_EDITOR
				battle.AddInfo($"偷取护盾总值：").AddInfo(stealShield, true);
#endif
				mTarget.AddShield(stealShield);
			}
		}

		public override void OnEnd()
		{
			base.OnEnd();
			mAuthor = null;
			mTarget = null;
		}
	}
}