using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Helper
{
    /// <summary>接口转换组件方法 </summary>
    public static T[] ConvertToArray<T>(IList list)
    {
        T[] ret = new T[list.Count];
        list.CopyTo(ret, 0);
        return ret;
    }

    /// <summary> www清理</summary>
    public static void Clear(this WWW www, bool asset = false)
    {
        if (www != null)
        {
            if (asset)
                www.assetBundle.Unload(true);
            www.Dispose();
            www = null;
        }
    }


    /// <summary>数组添加唯一</summary>
    public static void AddOnly<T>(this List<T> list, T temp)
    {
        if (!list.Contains(temp))
            list.Add(temp);
    }

    /// <summary>字典添加唯一</summary>
    public static void AddOnly<T1, T2>(this Dictionary<T1, T2> dis, T1 key, T2 value)
    {
        if (dis.ContainsKey(key))
            dis[key] = value;
        else
            dis.Add(key, value);
    }

    public static string GetString<T1,T2>(this Dictionary<T1, T2> dis)
    {
        if (dis == null) return "";
        string r = "[ ";
        foreach(KeyValuePair<T1,T2> kv in dis)
        {
            r += kv.Key + ":" + kv.Value+" , ";
        }
        return r+" ]";
    }


    /// <summary>字典添加叠加</summary>
    public static void AddUp<T1>(this Dictionary<T1, float> dis, T1 key, float value)
    {
        if (dis.ContainsKey(key))
            dis[key] += value;
        else
            dis.Add(key, value);
    }

    /// <summary>字典添加叠加</summary>
    public static void AddUp<T1>(this Dictionary<T1, int> dis, T1 key, int value)
    {
        if (dis.ContainsKey(key))
            dis[key] += value;
        else
            dis.Add(key, value);
    }
}