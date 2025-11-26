using System;

namespace BattleSystem
{
    public interface IObjectPool : IDisposable
    {
        void OnAwake();
    }
}
