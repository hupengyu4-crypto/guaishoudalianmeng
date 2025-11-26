using System;
namespace YoukiaSDKSpace
{
    //获取资源路径
    public class FastResPathAttr : Attribute
    {
        private string path;

        public string Path
        {
            get => path;
            private set => path = value;
        }
        public FastResPathAttr(string _path)
        {
            Path = _path;
        }
    }
}

