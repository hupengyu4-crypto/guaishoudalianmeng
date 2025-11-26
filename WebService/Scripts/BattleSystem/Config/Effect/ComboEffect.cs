namespace BattleSystem
{
    /// <summary>
    /// 斩杀
    /// </summary>
    public class ComboEffect : BaseEffect
    {
        /// <summary>
        /// 概率(0-100)
        /// </summary>
        public double probability;

        private bool _added;

        public override void OnBegin()
        {
            _added = false;
            base.OnBegin();
        }

        public override void OnEnd()
        {
            if (_added && target is Fighter targetFighter)
            {
                targetFighter.comboProbability -= probability;
            }

            base.OnEnd();
        }

        public override void Trigger()
        {
            var targetFighter = (Fighter)target;
            targetFighter.comboProbability += probability;
            _added = true;
        }
    }
}