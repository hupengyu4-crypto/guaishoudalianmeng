using System;
namespace YoukiaSDKSpace
{
    //获取资源类型  Prafab还是GameObject
    public class FastResTypeAttr : Attribute
    {
        private FastResType type;
        public FastResTypeAttr(FastResType type)
        {
            Type = type;
        }

        public FastResType Type
        {
            get => type;
            private set => type = value;
        }
    }
}
