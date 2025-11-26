using Google.Protobuf;
using System;
using System.Collections.Generic;
/// <summary>
/// protoBuf解析类
/// 兰阳
/// 2023-04-12
/// </summary>
/// <typeparam name="T"></typeparam>
public static class PbManager<T> where T : IMessage<T>
{
    private static readonly Dictionary<Type, MessageParser<T>> PB_PARSE_DIC =
        new Dictionary<Type, MessageParser<T>>();

    public static T ParseFrom(byte[] bytes)
    {
        lock (PB_PARSE_DIC)
        {
            Type t = typeof(T);
            if (!PB_PARSE_DIC.TryGetValue(t, out var value))
            {
                value = new MessageParser<T>(Activator.CreateInstance<T>);
                PB_PARSE_DIC.Add(t, value);
            }

            try
            {
                return value.ParseFrom(bytes);
            }
            catch (Exception e)
            {
                //网络消息Pb解析报错单独弹窗
                throw new ArgumentException("Pb erro", e);
            }
        }
    }
}
