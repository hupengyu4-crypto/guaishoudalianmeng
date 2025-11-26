using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BattleSystem
{
    /// <summary>
    ///子类属性必须为long类型的get set
    /// </summary>
    public abstract class BattleCommandHandler
    {
        public long Frame { get; private set; }

        /// <summary>
        /// 指令动作
        /// </summary>
        /// <param name="battle"></param>
        public abstract void Do(BaseBattle battle);

        public virtual void SetFrame(long frame)
        {
            this.Frame = frame;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns></returns>
        public abstract byte[] Serialize();

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="bytes"></param>
        public abstract bool UnSerialize(byte[] bytes);
    }
}
