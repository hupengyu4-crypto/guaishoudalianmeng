using System.Collections.Generic;

namespace BattleSystem
{
    /// <summary>
    /// 嘲讽
    /// </summary>
    public class TauntEffect : BaseEffect
    {
        /// <summary>
        /// 嘲讽次数
        /// </summary>
        private int _count;

        private Fighter _author;
        private Fighter _target;
        private LinkedListNode<long> _node;
        private int _tauntCount;

        public override void OnBegin()
        {
            _author = (Fighter)author;
            _target = (Fighter)target;
            _author.AddEvent(BattleObject.Event.TaunterBeAttacked, OnTaunterBeAttacked);

            base.OnBegin();
        }

        private void OnTaunterBeAttacked(EventParams eventParams)
        {
            if (eventParams is EventParams<long> args && args.data == _target.Uid)
            {
                _tauntCount++;
                if (_count > 0 && _tauntCount >= _count)
                {
                    target.Effect.ClearEffect(this);
                }
            }
        }

        private void RemoveExist()
        {
            if (_node != null)
            {
                _target.RemoveTaunt(_node);
                _node = null;
            }
        }

        public override void Trigger()
        {
            if (author.Uid != target.Uid)
            {
                RemoveExist();
#if UNITY_EDITOR
                battle.AddInfo($"嘲讽---触发者：{author.GetBaseDesc()}", true);
#endif
                _node = _target.AddTaunt(author.Uid);
            }
        }

        public override void OnEnd()
        {
            _author.RemoveEvent(BattleObject.Event.TaunterBeAttacked, OnTaunterBeAttacked);
            RemoveExist();
            _author = null;
            _target = null;
            _node = null;
            _tauntCount = 0;

            base.OnEnd();
        }

        public void SetData(TauntEffectConfig cfg)
        {
            _count = cfg.count;
        }
    }
}