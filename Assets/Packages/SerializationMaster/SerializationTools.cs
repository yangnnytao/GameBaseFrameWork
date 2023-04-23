using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;

public static class SerializationTools  {
    public static bool displayDEBUG = false;
    public static string testPath = Application.dataPath + "AssetResources/Table/";

    private static CultureInfo serializationCulInfo;
    public static CultureInfo SerializationCulInfo {
        get {
            if (serializationCulInfo == null) {
                serializationCulInfo = CultureInfo.InvariantCulture;
                //serializationCulInfo = new CultureInfo("de-DE");
            }
            return serializationCulInfo;
        }
    }
    /// <summary>
    /// 将value转换成info需要的 type
    /// </summary>
    /// <param name="info"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static object ChangeValueType(FieldInfo info, object value)
    {
        //判断一下类型 如果为枚举，则强转value为int
        if (info.FieldType.IsEnum)
        {
            return System.Convert.ChangeType(value, typeof(int), SerializationCulInfo);
        }

        if (info.FieldType == typeof(bool) && value is string)
        {
           return System.Convert.ChangeType(int.Parse(value as string), info.FieldType, SerializationCulInfo);
        }
        return System.Convert.ChangeType(value, info.FieldType, SerializationCulInfo);
        //info.FieldType
    }



    public static object ChangeValueType(Type type, object value)
    {
        //判断一下类型 如果为枚举，则强转value为int
        if (type.IsEnum)
        {
            return System.Convert.ChangeType(value, typeof(int), SerializationCulInfo);
        }

        if (type == typeof(bool) && value is string)
        {
            return System.Convert.ChangeType(int.Parse(value as string), type, SerializationCulInfo);
        }
        return System.Convert.ChangeType(value, type, SerializationCulInfo);
        //info.FieldType
    }


    public static T GetAttribute<T>(FieldInfo fieldInfo) where T : Attribute
    {
        var attributes = fieldInfo.GetCustomAttributes(true);
        if (attributes.Length > 0)
        {
            for (int i = 0; i < attributes.Length; i++)
            {
                if (attributes[i] is T)
                {
                    return attributes[i] as T;
                }
            }
        }
        return null;
    }
}
public struct FieldExcelReflectionHandler
{
    public FieldInfo field;
    public CustomSerializableAttribute attribute;
    public int index;

    public FieldExcelReflectionHandler(FieldInfo field, CustomSerializableAttribute attribute, int index)
    {
        this.field = field;
        this.attribute = attribute;
        this.index = index;
    }
}