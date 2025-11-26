#if UNDEF_BATTLE_UNITY_EDITOR
#undef UNITY_EDITOR
#endif

using System;
using BattleSystem;

/// <summary>
/// 检测属性效果
/// </summary>
public class CheckPropertyEffect : BaseEffect
{
	/// <summary>
	/// 检测的属性
	/// </summary>
	public BattleDef.Property property;
	/// <summary>
	/// 检测的类型
	/// </summary>
	public BattleDef.CompareType checkType;
	/// <summary>
	/// 属性值(百分比)
	/// </summary>
	public double value;
	/// <summary>
	/// 通过给自己加效果
	/// </summary>
	public long[] passedSelfEffects;
	/// <summary>
	/// 通过给目标加效果
	/// </summary>
	public long[] passedTargetEffects;
	/// <summary>
	/// 通过给造成事件者加效果
	/// </summary>
	public long[] passedEventMakerEffects;
	/// <summary>
	/// 未通过给自己加效果
	/// </summary>
	public long[] failedSelfEffects;
	/// <summary>
	/// 未通过给目标加效果
	/// </summary>
	public long[] failedTargetEffects;
	/// <summary>
	/// 未通过给造成事件者加效果
	/// </summary>
	public long[] failedEventMakerEffects;
	/// <summary>
	/// 通过立即清理事件
	/// </summary>
	public bool passClearEvent;
	/// <summary>
	/// 差异的百分之多少算一份（百分比）
	/// </summary>
	public double step;
	/// <summary>
	/// 一份改变多少值
	/// </summary>
	public double stepChangePerValue;
	/// <summary>
	/// 通过一次后百分比累加
	/// </summary>
	public bool accumulation;
	/// <summary>
	/// 与受到的总普攻伤害比较
	/// </summary>
	public bool compareWithAllDmg;
	/// <summary>
	/// 与受到的伤害比较
	/// </summary>
	public bool compareWithDmg;

	/// <summary>
	/// 目标
	/// </summary>
	private Fighter mTarget;
	private double _curValue;

	public override void OnBegin()
	{
		mTarget = (Fighter)target;
		_curValue = value;
		if (property != BattleDef.Property.None)
		{
			target.AddEvent(BattleObject.Event.Prop, PropHandler);
		}
		//base.OnBeginAddEvent();
		CheckProp(0);
	}

	private void PropHandler(EventParams arg0)
	{
		if (arg0 is PropParams param)
		{
			if (param.property == property)
			{
				CheckProp(param.authorUid);
			}
		}
	}

	private void CheckProp(long uid)
	{
		var additionValue = -1.0;
		bool checkState = false;
		if (checkType == BattleDef.CompareType.Small || checkType == BattleDef.CompareType.Equal || checkType == BattleDef.CompareType.Big || checkType == BattleDef.CompareType.SmallAndEqual || checkType == BattleDef.CompareType.BigAndEqual)
		{
			double curPropValue = mTarget.Data.nowProps[(int)property];
			double maxPropValue = 0;
			if (property == BattleDef.Property.hp || property == BattleDef.Property.max_hp || property == BattleDef.Property.lost_hp_total)
			{
				maxPropValue = mTarget.Data.nowProps[(int)BattleDef.Property.max_hp];
			}
			else
			{
				maxPropValue = Math.Max(mTarget.Data.initProps[(int)property], BattleUtils.GetMaxPropertyValue(mTarget, property));
			}
			double result = 0;
			if (compareWithAllDmg)
			{
				result = mTarget.hitNormalDamage / (curPropValue * 1.0d);
			}
			else if (compareWithDmg && _additionData != null)
			{
				result = _additionData.causedDamage / (curPropValue * 1.0d);
			}
			else
			{
				result = curPropValue * 1.0d / maxPropValue;
			}
			switch (checkType)
			{
				case BattleDef.CompareType.Small:
					checkState = result < _curValue * BattleDef.Percent;
					break;
				case BattleDef.CompareType.SmallAndEqual:
					checkState = result <= _curValue * BattleDef.Percent;
					break;
				case BattleDef.CompareType.Equal:
					checkState = Math.Abs(result - _curValue * BattleDef.Percent) < BattleDef.Percent;
					break;
				case BattleDef.CompareType.Big:
					checkState = result > _curValue * BattleDef.Percent;
					break;
				case BattleDef.CompareType.BigAndEqual:
					checkState = result >= _curValue * BattleDef.Percent;
					break;
			}

			if (step > 0.0 && stepChangePerValue > 0.0)
			{
				var diff = (int)Math.Floor(Math.Abs(result) / (step * BattleDef.Percent));
				if (diff > 1)
				{
					additionValue = (diff - 1) * stepChangePerValue;
				}
			}

#if UNITY_EDITOR
			battle.AddInfo(GetType().Name).AddInfo("-").AddInfo(sid).AddInfo(",属性检测是否通过:").AddInfo(checkState).AddInfo(",当前[").AddInfo(property).AddInfo("]:").AddInfo(curPropValue).AddInfo("/").AddInfo(maxPropValue).AddInfo(",百分比:").AddInfo(result * 100d).AddInfo("%", true);
#endif
		}
		else
		{
			var self = (Fighter)author;
			var selfValue = self.Data.GetProp(property);
			var otherValue = mTarget.Data.GetProp(property);
			switch (checkType)
			{
				case BattleDef.CompareType.LessThanMe:
					checkState = otherValue < selfValue;
					break;
				case BattleDef.CompareType.EqualToMe:
					checkState = Math.Abs(otherValue - selfValue) < BattleDef.Percent;
					break;
				case BattleDef.CompareType.GreaterThanMe:
					checkState = otherValue > selfValue;
					break;
			}

			if (step > 0.0 && stepChangePerValue > 0.0)
			{
				var diff = (int)Math.Floor(((double)Math.Abs(otherValue - selfValue) / selfValue) / (step * BattleDef.Percent));
				if (diff > 1)
				{
					additionValue = (diff - 1) * stepChangePerValue;
				}
			}

#if UNITY_EDITOR
			battle.AddInfo(GetType().Name).AddInfo("-").AddInfo(sid).AddInfo(",属性检测是否通过:").AddInfo(checkState).AddInfo(",当前[").AddInfo(property).AddInfo("]:").AddInfo(otherValue).AddInfo("-").AddInfo(selfValue).AddInfo(",百分比:").AddInfo("", true);
#endif
		}

		Fighter makeEventTarget = null;
		if (uid > 0)
		{
			makeEventTarget = battle.GetSceneObject<Fighter>(uid);
		}

		BaseBattle.EffectAdditionData tempAdditionData = null;
		if (additionValue > 0)
		{
			tempAdditionData = AllocAdditionData();
			tempAdditionData.stepChangeValue = additionValue;
		}

		long aUid = authorUid;
		if (checkState)
		{
			if (passClearEvent && property != BattleDef.Property.None)
			{
				target.RemoveEvent(BattleObject.Event.Prop, PropHandler);
			}

			if (accumulation)
			{
				_curValue += value;
			}

			author.Effect.AddEffects(passedSelfEffects, aUid, effectLevel, tempAdditionData);
			target.Effect.AddEffects(passedTargetEffects, aUid, effectLevel, tempAdditionData);
			makeEventTarget?.Effect.AddEffects(passedEventMakerEffects, aUid, effectLevel, tempAdditionData);
		}
		else
		{
			author.Effect.AddEffects(failedSelfEffects, aUid, effectLevel, tempAdditionData);
			target.Effect.AddEffects(failedTargetEffects, aUid, effectLevel, tempAdditionData);
			makeEventTarget?.Effect.AddEffects(failedEventMakerEffects, aUid, effectLevel, tempAdditionData);
		}

		//触发次数提供
		TriggerBegin();
	}

	public override void Trigger()
	{
	}

	public override void OnEnd()
	{
		if (property != BattleDef.Property.None)
		{
			target.RemoveEvent(BattleObject.Event.Prop, PropHandler);
		}
		mTarget = null;
		base.OnEnd();
	}
}