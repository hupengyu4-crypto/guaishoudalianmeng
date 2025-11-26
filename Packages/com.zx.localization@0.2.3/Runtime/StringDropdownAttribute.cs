using System;
using System.Reflection;
using UnityEngine;

public class StringDropdownAttribute : PropertyAttribute
{
    public string[] Options { get; }

    // 通过枚举类型初始化
    public StringDropdownAttribute(Type enumType)
    {
        if (enumType == null || !enumType.IsEnum)
            throw new ArgumentException("EnumType must be an enum type.");
        Options = Enum.GetNames(enumType);
    }

    // 动态选项构造方式
    public StringDropdownAttribute(Type targetType, string methodName)
    {
        Options = GetOptionsReflection(targetType,methodName);
    }

    private string[] GetOptionsReflection(Type targetType,string methodName)
    {
        // 通过反射调用静态方法
        var method = targetType.GetMethod(methodName, 
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        
        if (method != null && method.ReturnType == typeof(string[]))
        {
            return (string[])method.Invoke(null, null);
        }
        return new string[0];
    }

}