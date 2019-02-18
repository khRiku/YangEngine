using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;


public static class RelectionExtension
{
    /// <summary>
    /// 已反射的方式获取实例中指定名字的字段
    /// </summary>
    public static T GetFieldByReflection<T>(this System.Object pObject, string pName,BindingFlags pBindingFlags = BindingFlags.Default)
    {
        Type tType = pObject.GetType();
        BindingFlags tBindingFlags = BindingFlags.Public
                                     | BindingFlags.NonPublic
                                     | BindingFlags.Static
                                     | BindingFlags.Instance;

        if (pBindingFlags != BindingFlags.Default)
            tBindingFlags = pBindingFlags;

        FieldInfo tFieldInfo = tType.GetField(pName, tBindingFlags);

        T tT =(T) tFieldInfo.GetValue(pObject);

   
        return tT;
    }

}
